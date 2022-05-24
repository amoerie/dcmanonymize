using System;
using System.Collections.Concurrent;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using DcmAnonymize.Names;
using FellowOakDicom;
using KeyedSemaphores;

namespace DcmAnonymize.Patient
{
    public class PatientAnonymizer
    {
        private readonly RandomNameGenerator _randomNameGenerator;
        private readonly NationalNumberGenerator _nationalNumberGenerator;
        private readonly ConcurrentDictionary<string, AnonymizedPatient> _anonymizedPatients = new ConcurrentDictionary<string, AnonymizedPatient>();
        private readonly Random _random = new Random();
        private int _counter = 1;

        public PatientAnonymizer(RandomNameGenerator randomNameGenerator, NationalNumberGenerator nationalNumberGenerator)
        {
            _randomNameGenerator = randomNameGenerator ?? throw new ArgumentNullException(nameof(randomNameGenerator));
            _nationalNumberGenerator = nationalNumberGenerator ?? throw new ArgumentNullException(nameof(nationalNumberGenerator));
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
                        var name = _randomNameGenerator.GenerateRandomName();
                        var birthDate = GenerateRandomBirthdate();
                        var patientId = $"PAT{DateTime.Now:yyyyMMddHHmm}{_counter++}";
                        PatientSex? sex = null;
                        if (dicomDataSet.TryGetString(DicomTag.PatientSex, out string parsedPatientSex))
                        {
                            switch (parsedPatientSex.ToUpperInvariant())
                            {
                                case "m":
                                    sex = PatientSex.Male;
                                    break;
                                case "f":
                                    sex = PatientSex.Female;
                                    break;
                                case "o":
                                    sex = PatientSex.Other;
                                    break;
                            }
                        }
                        var nationalNumber = _nationalNumberGenerator.GenerateRandomNationalNumber(birthDate, sex);

                        anonymizedPatient = new AnonymizedPatient(name, nationalNumber, birthDate, patientId, sex);
                        
                        _anonymizedPatients[originalPatientName] = anonymizedPatient;
                    }
                }
            }

            dicomDataSet.AddOrUpdate(new DicomPersonName(DicomTag.PatientName, anonymizedPatient.Name.LastName, anonymizedPatient.Name.FirstName));
            dicomDataSet.AddOrUpdate(DicomTag.PatientBirthDate, anonymizedPatient.BirthDate.ToString("yyyyMMdd"));
            dicomDataSet.AddOrUpdate(DicomTag.PatientID, anonymizedPatient.PatientId);
            dicomDataSet.Remove(DicomTag.PatientAddress);
            dicomDataSet.Remove(DicomTag.MilitaryRank);
            dicomDataSet.Remove(DicomTag.PatientTelephoneNumbers);
            dicomDataSet.AddOrUpdate(DicomTag.OtherPatientIDsRETIRED, anonymizedPatient.NationalNumber);
            dicomDataSet.AddOrUpdate(new DicomSequence(DicomTag.OtherPatientIDsSequence, 
                new DicomDataset {
                    { DicomTag.PatientID, anonymizedPatient.PatientId },
                    { DicomTag.IssuerOfPatientID, "DcmAnonymize" },
                    { DicomTag.TypeOfPatientID, "PATIENTID" }
                },
                new DicomDataset {
                    { DicomTag.PatientID, anonymizedPatient.NationalNumber },
                    { DicomTag.IssuerOfPatientID, "DcmAnonymize" },
                    { DicomTag.TypeOfPatientID, "NATIONALNUMBER" }
                }
            ));
            dicomDataSet.AddOrUpdate(DicomTag.PatientIdentityRemoved, "YES");
            dicomDataSet.AddOrUpdate(DicomTag.DeidentificationMethod, $"DcmAnonymize {typeof(DicomAnonymizer).Assembly.GetName().Version}");
            dicomDataSet.Remove(DicomTag.DeidentificationMethodCodeSequence);
            dicomDataSet.Remove(DicomTag.ReferencedPatientSequence);
        }
        
        private DateTime GenerateRandomBirthdate()
        {
            // A random age between 18 and 80
            var ageInDays = TimeSpan.FromDays(_random.Next(18 * 365, 80 * 365));
            return DateTime.Today.Add(-ageInDays);
        }
        
    }
}
