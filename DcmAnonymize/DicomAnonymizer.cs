using System;
using System.Threading.Tasks;
using DcmAnonymize.Patient;
using DcmAnonymize.Series;
using DcmAnonymize.Study;
using FellowOakDicom;

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

        public async Task AnonymizeAsync(DicomFileMetaInformation metaInfo, DicomDataset dicomDataset)
        {
            await _patientAnonymizer.AnonymizeAsync(metaInfo, dicomDataset);
            await _studyAnonymizer.AnonymizeAsync(metaInfo, dicomDataset);
            await _seriesAnonymizer.AnonymizeAsync(metaInfo, dicomDataset);
            await _instanceAnonymizer.AnonymizeAsync(metaInfo, dicomDataset);
        }
    }
}
