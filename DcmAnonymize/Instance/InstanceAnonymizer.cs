using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DcmAnonymize.Names;
using Dicom;

namespace DcmAnonymize
{
    public class InstanceAnonymizer
    {
        public async Task AnonymizeAsync(DicomDataset dicomDataSet)
        {
            dicomDataSet.AddOrUpdate(DicomTag.SOPInstanceUID, DicomUIDGenerator.GenerateDerivedFromUUID().UID);
        }
    }
}