using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DcmAnonymize.Instance;
using DcmAnonymize.Patient;
using DcmAnonymize.Series;
using DcmAnonymize.Study;
using FellowOakDicom;
using KeyedSemaphores;

namespace DcmAnonymize;

public class DicomAnonymizer
{
    private readonly PatientAnonymizer _patientAnonymizer;
    private readonly StudyAnonymizer _studyAnonymizer;
    private readonly SeriesAnonymizer _seriesAnonymizer;
    private readonly InstanceAnonymizer _instanceAnonymizer;
    private readonly ConcurrentDictionary<string, DicomUID> _anonymizedUIDs = new ConcurrentDictionary<string, DicomUID>();

    public DicomAnonymizer(PatientAnonymizer patientAnonymizer, StudyAnonymizer studyAnonymizer,
        SeriesAnonymizer seriesAnonymizer, InstanceAnonymizer instanceAnonymizer)
    {
        _patientAnonymizer = patientAnonymizer ?? throw new ArgumentNullException(nameof(patientAnonymizer));
        _studyAnonymizer = studyAnonymizer ?? throw new ArgumentNullException(nameof(studyAnonymizer));
        _seriesAnonymizer = seriesAnonymizer ?? throw new ArgumentNullException(nameof(seriesAnonymizer));
        _instanceAnonymizer = instanceAnonymizer ?? throw new ArgumentNullException(nameof(instanceAnonymizer));
    }

    public Task AnonymizeAsync(DicomFile dicomFile)
    {
        return AnonymizeAsync(dicomFile.FileMetaInfo, dicomFile.Dataset);
    }

    public async Task AnonymizeAsync(DicomFileMetaInformation metaInfo, DicomDataset dicomDataset)
    {
        await _patientAnonymizer.AnonymizeAsync(metaInfo, dicomDataset, _anonymizedUIDs);
        await _studyAnonymizer.AnonymizeAsync(metaInfo, dicomDataset, _anonymizedUIDs);
        await _seriesAnonymizer.AnonymizeAsync(metaInfo, dicomDataset, _anonymizedUIDs);
        await _instanceAnonymizer.AnonymizeAsync(metaInfo, dicomDataset, _anonymizedUIDs);
        
        // Ensure referential integrity of anonymized UIDs
        var stack = new Stack<DicomDataset>();
        stack.Push(dicomDataset);
        while (stack.Count > 0)
        {
            var next = stack.Pop();

            foreach (var item in next.ToList())
            {
                if (item is DicomSequence dicomSequence)
                {
                    foreach (var dicomSequenceItem in dicomSequence)
                    {
                        stack.Push(dicomSequenceItem);
                    }
                    continue;
                }

                if (item.ValueRepresentation != DicomVR.UI)
                {
                    continue;
                }
                
                var originalUID = ((DicomElement)item).Get<string>();

                if (_anonymizedUIDs.TryGetValue(originalUID, out var anonymizedUID) && anonymizedUID.UID != originalUID)
                {
                    next.AddOrUpdate(new DicomUniqueIdentifier(item.Tag, anonymizedUID));
                    continue;
                }
                
                using (await KeyedSemaphore.LockAsync(originalUID))
                {
                    anonymizedUID = _anonymizedUIDs.GetOrAdd(originalUID, _ => DicomUIDGenerator.GenerateDerivedFromUUID());
                    next.AddOrUpdate(new DicomUniqueIdentifier(item.Tag, anonymizedUID));
                }
            }
        }
    }
    
}
