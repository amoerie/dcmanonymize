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
            
            dicomDataSet.AddOrUpdate(DicomTag.SOPInstanceUID, newSopInstanceUID.UID);
        }
    }
}