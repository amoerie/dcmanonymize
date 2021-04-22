using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using DcmAnonymize.Names;
using Dicom;
using KeyedSemaphores;

namespace DcmAnonymize.Study
{
    public class StudyAnonymizer
    {
        private readonly RandomNameGenerator _randomNameGenerator;
        private readonly ConcurrentDictionary<string, AnonymizedStudy> _anonymizedStudies = new ConcurrentDictionary<string, AnonymizedStudy>();
        private readonly Random _random = new Random();
        private int _counter = 1;

        public StudyAnonymizer(RandomNameGenerator randomNameGenerator)
        {
            _randomNameGenerator = randomNameGenerator ?? throw new ArgumentNullException(nameof(randomNameGenerator));
            _random = new Random();
        }

        public async Task AnonymizeAsync(DicomFileMetaInformation metaInfo, DicomDataset dicomDataSet)
        {
            var originalStudyInstanceUID = dicomDataSet.GetSingleValue<string>(DicomTag.StudyInstanceUID);
            var originalModality = dicomDataSet.GetValueOrDefault<string>(DicomTag.Modality, 0, null!);
    
            if (!_anonymizedStudies.TryGetValue(originalStudyInstanceUID, out var anonymizedStudy))
            {
                var key = $"STUDY_{originalStudyInstanceUID}";
                using (await KeyedSemaphore.LockAsync(key))
                {
                    if (!_anonymizedStudies.TryGetValue(originalStudyInstanceUID, out anonymizedStudy))
                    {
                        anonymizedStudy = new AnonymizedStudy();

                        anonymizedStudy.StudyInstanceUID = DicomUIDGenerator.GenerateDerivedFromUUID().UID;
                        anonymizedStudy.Description = "A wonderful study";
                        anonymizedStudy.AccessionNumber = $"{originalModality}{DateTime.Today:yyyyMMddHHmm}{_counter++}";
                        anonymizedStudy.StudyRequestingPhysician = _randomNameGenerator.GenerateRandomName();
                        anonymizedStudy.StudyDateTime = DateTime.Now;

                        anonymizedStudy.StudyID = anonymizedStudy.AccessionNumber;
                        anonymizedStudy.InstitutionName = "Random Hospital " + _random.Next(1, 100);
                        anonymizedStudy.StudyPerformingPhysician = _randomNameGenerator.GenerateRandomName();
                        _anonymizedStudies[originalStudyInstanceUID] = anonymizedStudy;
                    }
                }
            }

            dicomDataSet.AddOrUpdate(DicomTag.StudyInstanceUID, anonymizedStudy.StudyInstanceUID);
            
            dicomDataSet.AddOrUpdate(DicomTag.AccessionNumber, anonymizedStudy.AccessionNumber);
            dicomDataSet.AddOrUpdate(new DicomPersonName(
                DicomTag.RequestingPhysician, 
                anonymizedStudy.StudyRequestingPhysician.LastName,
                anonymizedStudy.StudyRequestingPhysician.FirstName
                )
            );
            dicomDataSet.AddOrUpdate(new DicomPersonName(DicomTag.PerformingPhysicianName, anonymizedStudy.StudyPerformingPhysician.LastName, anonymizedStudy.StudyPerformingPhysician.FirstName));
            dicomDataSet.AddOrUpdate(new DicomPersonName(DicomTag.NameOfPhysiciansReadingStudy, anonymizedStudy.StudyPerformingPhysician.LastName, anonymizedStudy.StudyPerformingPhysician.FirstName));
            
            dicomDataSet.AddOrUpdate(DicomTag.StudyDate, anonymizedStudy.StudyDateTime.ToString("yyyyMMdd"));

            var patientDOB = dicomDataSet.GetSingleValue<DateTime>(DicomTag.PatientBirthDate);
            var age = Math.Floor((anonymizedStudy.StudyDateTime - patientDOB).TotalDays / 365);
            dicomDataSet.AddOrUpdate(DicomTag.PatientAge, $"{age.ToString("000")}Y");

            dicomDataSet.AddOrUpdate(DicomTag.StudyTime, anonymizedStudy.StudyDateTime.ToString("HHmmss"));
            dicomDataSet.AddOrUpdate(DicomTag.StudyID, anonymizedStudy.StudyID);
            dicomDataSet.AddOrUpdate(DicomTag.SourceApplicationEntityTitle, "DobcoAnoSCU");

            //AdditionalTags
            
            dicomDataSet.AddOrUpdate(DicomTag.InstitutionName, anonymizedStudy.InstitutionName);
            dicomDataSet.Remove(DicomTag.InstitutionAddress);
            dicomDataSet.AddOrUpdate(new DicomPersonName(
    DicomTag.ReferringPhysicianName,
    anonymizedStudy.StudyRequestingPhysician.LastName,
    anonymizedStudy.StudyRequestingPhysician.FirstName
    )         
);
            dicomDataSet.Remove(DicomTag.PhysiciansOfRecord);


        }
    }
}