using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FellowOakDicom;
using KeyedSemaphores;

namespace DcmAnonymize.Recursive;

public class RecursiveAnonymizer
{
    public async Task AnonymizeAsync(DicomAnonymizationContext context)
    {
        var dicomDataset = context.Dataset;
        var anonymizedUIDs = context.AnonymizedUIDs;
        var stack = new Stack<DicomDataset>();
        stack.Push(dicomDataset);
        while (stack.Count > 0)
        {
            var next = stack.Pop();
            
            next.Remove(item => KnownDicomTags.TagsToRemove.Contains(item.Tag));
            
            var items = next.ToList();
            for (var i = 0; i < items.Count; i++)
            {
                var item = items[i];
                if (item is DicomSequence dicomSequence)
                {
                    foreach (var dicomSequenceItem in dicomSequence)
                    {
                        stack.Push(dicomSequenceItem);
                    }

                    continue;
                }

                if (item.ValueRepresentation == DicomVR.UI && KnownDicomTags.UIDTagsToAnonymize.Contains(item.Tag))
                {
                    // Ensure referential integrity of anonymized UIDs
                    var originalUIDs = ((DicomUniqueIdentifier)item).Get<DicomUID[]>();
                    var currentAnonymizedUIDs = new DicomUID[originalUIDs.Length];

                    var hasChanged = false;
                    for (var j = 0; j < originalUIDs.Length; j++)
                    {
                        var originalUID = originalUIDs[j];
                        if (anonymizedUIDs.TryGetValue(originalUID.UID, out var anonymizedUID) && originalUID.UID != anonymizedUID.UID)
                        {
                            currentAnonymizedUIDs[j] = anonymizedUID;
                            hasChanged = true;
                            continue;
                        }

                        using (await KeyedSemaphore.LockAsync(originalUID.UID))
                        {
                            anonymizedUID = anonymizedUIDs.GetOrAdd(originalUID.UID, _ => DicomUIDGenerator.GenerateDerivedFromUUID());
                            currentAnonymizedUIDs[j] = anonymizedUID;
                            hasChanged = true;
                        }
                    }

                    if (hasChanged)
                    {
                        next.AddOrUpdate(new DicomUniqueIdentifier(item.Tag, currentAnonymizedUIDs));
                    }
                }
            }
        }
    }

}
