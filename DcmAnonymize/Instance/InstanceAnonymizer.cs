using System;
using System.Globalization;
using System.Threading.Tasks;
using Dicom;

namespace DcmAnonymize
{
    public class InstanceAnonymizer
    {
        public async Task AnonymizeAsync(DicomFileMetaInformation metaInfo, DicomDataset dicomDataSet)
        {
            var newSopInstanceUID = DicomUIDGenerator.GenerateDerivedFromUUID();

            metaInfo.MediaStorageSOPInstanceUID = newSopInstanceUID;
            metaInfo.SourceApplicationEntityTitle = "DcmAnonymize";
            
            dicomDataSet.AddOrUpdate(DicomTag.SOPInstanceUID, newSopInstanceUID.UID);
            dicomDataSet.AddOrUpdate(DicomTag.InstanceCreationDate, DateTime.Now.ToString("yyyyMMdd", CultureInfo.InvariantCulture));
            dicomDataSet.AddOrUpdate(DicomTag.InstanceCreationTime, DateTime.Now.ToString("HHmmss", CultureInfo.InvariantCulture));
        }
    }
}