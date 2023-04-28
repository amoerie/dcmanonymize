using System;
using System.Collections.Concurrent;
using System.Globalization;
using System.Threading.Tasks;
using FellowOakDicom;
using KeyedSemaphores;

namespace DcmAnonymize.Instance;

public class InstanceAnonymizer
{
    private readonly ConcurrentDictionary<string, AnonymizedInstance> _anonymizedInstances;

    public InstanceAnonymizer()
    {
        _anonymizedInstances = new ConcurrentDictionary<string, AnonymizedInstance>();
    }
    
    public async Task AnonymizeAsync(DicomAnonymizationContext context)
    {
        var dicomDataSet = context.Dataset;
        var anonymizedUIDs = context.AnonymizedUIDs;
        var metaInfo = context.MetaInfo;
        
        var originalSopInstanceUID = dicomDataSet.GetSingleValue<string>(DicomTag.SOPInstanceUID);

        if (!_anonymizedInstances.TryGetValue(originalSopInstanceUID, out var anonymizedInstance))
        {
            using (await KeyedSemaphore.LockAsync(originalSopInstanceUID))
            {
                if (!_anonymizedInstances.TryGetValue(originalSopInstanceUID, out anonymizedInstance))
                {
                    var anonymizedSopInstanceUID = anonymizedUIDs.GetOrAdd(originalSopInstanceUID, _ => DicomUIDGenerator.GenerateDerivedFromUUID());
                    anonymizedUIDs[anonymizedSopInstanceUID.UID] = anonymizedSopInstanceUID;
                    var instanceCreationDate = DateTime.Now.ToString("yyyyMMdd", CultureInfo.InvariantCulture);
                    var instanceCreationTime = DateTime.Now.ToString("HHmmss", CultureInfo.InvariantCulture);

                    anonymizedInstance = new AnonymizedInstance(anonymizedSopInstanceUID, instanceCreationDate, instanceCreationTime);

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
