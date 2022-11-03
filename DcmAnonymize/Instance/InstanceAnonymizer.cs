using System;
using System.Collections.Concurrent;
using System.Globalization;
using System.Threading.Tasks;
using FellowOakDicom;
using KeyedSemaphores;

namespace DcmAnonymize.Instance;

public class InstanceAnonymizer
{
    private readonly ConcurrentDictionary<string,AnonymizedInstance> _anonymizedInstances;

    public InstanceAnonymizer()
    {
        _anonymizedInstances = new ConcurrentDictionary<string, AnonymizedInstance>();
    }
    
    public async Task AnonymizeAsync(DicomFileMetaInformation metaInfo, DicomDataset dicomDataSet)
    {
        var originalSopInstanceUID = dicomDataSet.GetSingleValue<string>(DicomTag.SOPInstanceUID);

        if (!_anonymizedInstances.TryGetValue(originalSopInstanceUID, out var anonymizedInstance))
        {
            using (await KeyedSemaphore.LockAsync($"INSTANCE_{originalSopInstanceUID}"))
            {
                if (!_anonymizedInstances.TryGetValue(originalSopInstanceUID, out anonymizedInstance))
                {
                    var newSopInstanceUID = DicomUIDGenerator.GenerateDerivedFromUUID();
                    var instanceCreationDate = DateTime.Now.ToString("yyyyMMdd", CultureInfo.InvariantCulture);
                    var instanceCreationTime = DateTime.Now.ToString("HHmmss", CultureInfo.InvariantCulture);

                    anonymizedInstance = new AnonymizedInstance(newSopInstanceUID, instanceCreationDate, instanceCreationTime);

                    _anonymizedInstances[originalSopInstanceUID] = anonymizedInstance;
                }
            }
        }
            

        metaInfo.MediaStorageSOPInstanceUID = anonymizedInstance.SopInstanceUID;
        metaInfo.SourceApplicationEntityTitle = "DcmAnonymize";
            
        dicomDataSet.AddOrUpdate(DicomTag.SOPInstanceUID, anonymizedInstance.SopInstanceUID.UID);
        dicomDataSet.AddOrUpdate(DicomTag.InstanceCreationDate, anonymizedInstance.InstanceCreationDate);
        dicomDataSet.AddOrUpdate(DicomTag.InstanceCreationTime, anonymizedInstance.InstanceCreationTime);
    }
}
