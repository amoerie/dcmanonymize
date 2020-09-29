using System;
using Dicom;

namespace DcmAnonymize
{
    public class DicomAnonymizer
    {
        private readonly PatientAnonymizer _patientAnonymizer;
        private readonly StudyAnonymizer _studyAnonymizer;
        private readonly SeriesAnonymizer _seriesAnonymizer;
        private readonly InstanceAnonymizer _instanceAnonymizer;

        public DicomAnonymizer(PatientAnonymizer patientAnonymizer, StudyAnonymizer studyAnonymizer,
            SeriesAnonymizer seriesAnonymizer, InstanceAnonymizer instanceAnonymizer)
        {
            _patientAnonymizer = patientAnonymizer ?? throw new ArgumentNullException(nameof(patientAnonymizer));
            _studyAnonymizer = studyAnonymizer ?? throw new ArgumentNullException(nameof(studyAnonymizer));
            _seriesAnonymizer = seriesAnonymizer ?? throw new ArgumentNullException(nameof(seriesAnonymizer));
            _instanceAnonymizer = instanceAnonymizer ?? throw new ArgumentNullException(nameof(instanceAnonymizer));
        }

        public void Anonymize(DicomDataset dicomDataset)
        {
            _patientAnonymizer.Anonymize(dicomDataset);
            _studyAnonymizer.Anonymize(dicomDataset);
            _seriesAnonymizer.Anonymize(dicomDataset);
            _instanceAnonymizer.Anonymize(dicomDataset);
        }
    }
}