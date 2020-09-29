using System;
using System.Collections.Generic;
using DcmAnonymize.Names;
using Dicom;

namespace DcmAnonymize
{
    public class StudyAnonymizer
    {
        private readonly RandomNameGenerator _randomNameGenerator;
        private readonly IDictionary<string, AnonymizedStudy> _anonymizedStudies = new Dictionary<string, AnonymizedStudy>();
        private readonly Random _random = new Random();
        private int _counter = 1;

        public StudyAnonymizer(RandomNameGenerator randomNameGenerator)
        {
            _randomNameGenerator = randomNameGenerator ?? throw new ArgumentNullException(nameof(randomNameGenerator));
        }

        public void Anonymize(DicomDataset dicomDataSet)
        {
            var originalStudyInstanceUID = dicomDataSet.GetSingleValue<string>(DicomTag.StudyInstanceUID);
            var originalModality = dicomDataSet.GetValueOrDefault<string>(DicomTag.Modality, 0, null!);
    
            if (!_anonymizedStudies.TryGetValue(originalStudyInstanceUID, out var anonymizedStudy))
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