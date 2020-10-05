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
        }

        public async Task AnonymizeAsync(DicomDataset dicomDataSet)
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
                        anonymizedStudy.AccessionNumber = $"{originalModality}{DateTime.Today:yyyyMMdd}{_counter++}";
                        anonymizedStudy.StudyRequestingPhysician = _randomNameGenerator.GenerateRandomName();
                        anonymizedStudy.StudyDateTime = DateTime.Now;
                        anonymizedStudy.StudyID = anonymizedStudy.AccessionNumber;

                        _anonymizedStudies[originalStudyInstanceUID] = anonymizedStudy;
                    }
                }
            }

            dicomDataSet.AddOrUpdate(DicomTag.StudyInstanceUID, anonymizedStudy.StudyInstanceUID);
            dicomDataSet.AddOrUpdate(DicomTag.StudyDescription, anonymizedStudy.Description);
            dicomDataSet.AddOrUpdate(DicomTag.AccessionNumber, anonymizedStudy.AccessionNumber);
            dicomDataSet.AddOrUpdate(new DicomPersonName(
                DicomTag.RequestingPhysician, 
                anonymizedStudy.StudyRequestingPhysician.LastName,
                anonymizedStudy.StudyRequestingPhysician.FirstName
                )
            );
            dicomDataSet.AddOrUpdate(DicomTag.StudyDate, anonymizedStudy.StudyDateTime.ToString("yyyyMMdd"));
            dicomDataSet.AddOrUpdate(DicomTag.StudyTime, anonymizedStudy.StudyDateTime.ToString("HHmmss"));
            dicomDataSet.AddOrUpdate(DicomTag.StudyID, anonymizedStudy.StudyID);
        }
    }
}