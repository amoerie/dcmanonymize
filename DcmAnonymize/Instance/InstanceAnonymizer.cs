using System;
using System.Collections.Generic;
using DcmAnonymize.Names;
using Dicom;

namespace DcmAnonymize
{
    public class InstanceAnonymizer
    {
        public void Anonymize(DicomDataset dicomDataSet)
        {
            dicomDataSet.AddOrUpdate(DicomTag.SOPInstanceUID, DicomUIDGenerator.GenerateDerivedFromUUID().UID);
        }
    }
}