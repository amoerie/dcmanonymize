using System;
using System.Collections.Concurrent;
using System.Globalization;
using System.Threading.Tasks;
using DcmAnonymize.Names;
using FellowOakDicom;
using KeyedSemaphores;

namespace DcmAnonymize.Study;

public class StudyAnonymizer
{
    private readonly RandomNameGenerator _randomNameGenerator;
    private readonly ConcurrentDictionary<string, AnonymizedStudy> _anonymizedStudies = new ConcurrentDictionary<string, AnonymizedStudy>();
    private readonly Random _random;
    private int _counter = 1;

    public StudyAnonymizer(RandomNameGenerator randomNameGenerator)
    {
        _randomNameGenerator = randomNameGenerator ?? throw new ArgumentNullException(nameof(randomNameGenerator));
        _random = new Random();
    }

    public async Task AnonymizeAsync(DicomAnonymizationContext context)
    {
        var dicomDataSet = context.Dataset;
        var anonymizedUIDs = context.AnonymizedUIDs;
        var originalStudyInstanceUID = dicomDataSet.GetSingleValueOrDefault(DicomTag.StudyInstanceUID, string.Empty);

        if (string.IsNullOrEmpty(originalStudyInstanceUID))
        {
            originalStudyInstanceUID = DicomUIDGenerator.GenerateDerivedFromUUID().UID;
        }
        
        var originalModality = dicomDataSet.GetValueOrDefault<string>(DicomTag.Modality, 0, null!);

        if (!_anonymizedStudies.TryGetValue(originalStudyInstanceUID, out var anonymizedStudy))
        {
            using (await KeyedSemaphore.LockAsync(originalStudyInstanceUID))
            {
                if (!_anonymizedStudies.TryGetValue(originalStudyInstanceUID, out anonymizedStudy))
                {
                    var anonymizedStudyInstanceUID = anonymizedUIDs.GetOrAdd(originalStudyInstanceUID, _ => DicomUIDGenerator.GenerateDerivedFromUUID());
                    anonymizedUIDs[anonymizedStudyInstanceUID.UID] = anonymizedStudyInstanceUID;
                    var accessionNumber = $"{originalModality}{DateTime.Now:yyyyMMddHHmm}{_counter++}";
                    var requestingPhysician = _randomNameGenerator.GenerateRandomName();
                    var studyDateTime = DateTime.Now;
                    var studyId = accessionNumber;
                    var institutionName = "Random Hospital " + _random.Next(1, 100);
                    var performingPhysician = _randomNameGenerator.GenerateRandomName();

                    anonymizedStudy = new AnonymizedStudy(
                        anonymizedStudyInstanceUID,
                        accessionNumber,
                        studyDateTime,
                        requestingPhysician,
                        performingPhysician,
                        studyId, 
                        institutionName
                    );
                    _anonymizedStudies[originalStudyInstanceUID] = anonymizedStudy;
                }
            }
        }

        dicomDataSet.AddOrUpdate(DicomTag.StudyInstanceUID, anonymizedStudy.StudyInstanceUID);

        dicomDataSet.AddOrUpdate(DicomTag.AccessionNumber, anonymizedStudy.AccessionNumber);
        dicomDataSet.AddOrUpdate(new DicomPersonName(
                DicomTag.RequestingPhysician,
                anonymizedStudy.RequestingPhysician.LastName,
                anonymizedStudy.RequestingPhysician.FirstName
            )
        );
        dicomDataSet.AddOrUpdate(new DicomPersonName(
                DicomTag.PerformingPhysicianName,
                anonymizedStudy.PerformingPhysician.LastName,
                anonymizedStudy.PerformingPhysician.FirstName
            )
        );
        dicomDataSet.AddOrUpdate(new DicomPersonName(
                DicomTag.NameOfPhysiciansReadingStudy, 
                anonymizedStudy.PerformingPhysician.LastName,
                anonymizedStudy.PerformingPhysician.FirstName
            )
        );

        dicomDataSet.AddOrUpdate(DicomTag.StudyDate, anonymizedStudy.StudyDateTime.ToString("yyyyMMdd", CultureInfo.InvariantCulture));
        dicomDataSet.AddOrUpdate(DicomTag.AcquisitionDate, anonymizedStudy.StudyDateTime.ToString("yyyyMMdd", CultureInfo.InvariantCulture));
        dicomDataSet.AddOrUpdate(DicomTag.ContentDate, anonymizedStudy.StudyDateTime.ToString("yyyyMMdd", CultureInfo.InvariantCulture));

        // Assuming the PatientAnonymizer made a pass first, we can be sure to have a patient DOB
        var patientBirthDate = dicomDataSet.GetSingleValue<DateTime>(DicomTag.PatientBirthDate);
        var studyDate = anonymizedStudy.StudyDateTime.Date;
        var patientAge = studyDate.Year - patientBirthDate.Year;
        if (patientBirthDate.Date > studyDate.AddYears(-patientAge))
        {
            patientAge--;
        }
        dicomDataSet.AddOrUpdate(DicomTag.PatientAge, $"{patientAge.ToString("000", CultureInfo.InvariantCulture)}Y");

        dicomDataSet.AddOrUpdate(DicomTag.StudyTime, anonymizedStudy.StudyDateTime.ToString("HHmmss", CultureInfo.InvariantCulture));
        dicomDataSet.AddOrUpdate(DicomTag.AcquisitionTime, anonymizedStudy.StudyDateTime.ToString("HHmmss", CultureInfo.InvariantCulture));
        dicomDataSet.AddOrUpdate(DicomTag.ContentTime, anonymizedStudy.StudyDateTime.ToString("HHmmss", CultureInfo.InvariantCulture));
        dicomDataSet.AddOrUpdate(DicomTag.StudyID, anonymizedStudy.StudyId);

        //AdditionalTags

        dicomDataSet.AddOrUpdate(DicomTag.InstitutionName, anonymizedStudy.InstitutionName);
        dicomDataSet.AddOrUpdate(new DicomPersonName(
                DicomTag.ReferringPhysicianName,
                anonymizedStudy.RequestingPhysician.LastName,
                anonymizedStudy.RequestingPhysician.FirstName
            )
        );
    }
}
