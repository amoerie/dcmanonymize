using System;
using System.Collections.Concurrent;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using DcmAnonymize.Names;
using Dicom;
using KeyedSemaphores;

namespace DcmAnonymize.Patient
{
    public class PatientAnonymizer
    {
        private readonly RandomNameGenerator _randomNameGenerator;
        private readonly ConcurrentDictionary<string, AnonymizedPatient> _anonymizedPatients = new ConcurrentDictionary<string, AnonymizedPatient>();
        private readonly Random _random = new Random();
        private int _counter = 1;

        public PatientAnonymizer(RandomNameGenerator randomNameGenerator)
        {
            _randomNameGenerator = randomNameGenerator ?? throw new ArgumentNullException(nameof(randomNameGenerator));
        }

        public async Task AnonymizeAsync(DicomFileMetaInformation metaInfo, DicomDataset dicomDataSet)
        {
            var originalPatientName = dicomDataSet.GetSingleValue<string>(DicomTag.PatientName).TrimEnd();
    
            if (!_anonymizedPatients.TryGetValue(originalPatientName, out var anonymizedPatient))
            {
                var key = $"PATIENT_{originalPatientName}";
                using (await KeyedSemaphore.LockAsync(key))
                {
                    if (!_anonymizedPatients.TryGetValue(originalPatientName, out anonymizedPatient))
                    {
                        anonymizedPatient = new AnonymizedPatient();

                        anonymizedPatient.Name = _randomNameGenerator.GenerateRandomName();
                        anonymizedPatient.BirthDate = GenerateRandomBirthdate();
                        anonymizedPatient.PatientId = $"PAT{DateTime.Now:yyyyMMddHHmm}{_counter++}";
                        anonymizedPatient.NationalNumber = GenerateRandomNationalNumber(anonymizedPatient.BirthDate);

                        _anonymizedPatients[originalPatientName] = anonymizedPatient;
                    }
                }
            }

            dicomDataSet.AddOrUpdate(new DicomPersonName(DicomTag.PatientName, anonymizedPatient.Name.LastName, anonymizedPatient.Name.FirstName));
            dicomDataSet.AddOrUpdate(DicomTag.PatientBirthDate, anonymizedPatient.BirthDate);
            dicomDataSet.Remove(DicomTag.PatientAddress);
            dicomDataSet.Remove(DicomTag.MilitaryRank);
            dicomDataSet.Remove(DicomTag.PatientTelephoneNumbers);
            dicomDataSet.AddOrUpdate(DicomTag.OtherPatientIDsRETIRED, anonymizedPatient.NationalNumber);
            dicomDataSet.AddOrUpdate(new DicomSequence(DicomTag.OtherPatientIDsSequence, new DicomDataset
            {
                { DicomTag.PatientID, anonymizedPatient.NationalNumber }
            }));
        }
        
        private DateTime GenerateRandomBirthdate()
        {
            var ageInDays = TimeSpan.FromDays(_random.Next(18 * 365, 80 * 365));
            return DateTime.Today.Add(-ageInDays);
        }
        
        private string GenerateRandomNationalNumber(DateTime birthDate)
        {
            var year = birthDate.Year.ToString("0000").Substring(2, 2);
            var month = birthDate.Month.ToString("00");
            var day = birthDate.Day.ToString("00");
            var index = _random.Next(0, 999).ToString("000");
            var number = long.Parse($"{year}{month}{day}{index}");
            var checkSum = (number % 97).ToString("00");

            return $"{number}{checkSum}";
        }
    }
}