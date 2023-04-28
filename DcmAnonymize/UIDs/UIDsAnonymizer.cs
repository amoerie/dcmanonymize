using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FellowOakDicom;
using KeyedSemaphores;

namespace DcmAnonymize.UIDs;

public class UIDsAnonymizer
{
    public async Task AnonymizeAsync(DicomAnonymizationContext context)
    {
        var dicomDataset = context.Dataset;
        var anonymizedUIDs = context.AnonymizedUIDs;
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

                if (item.ValueRepresentation != DicomVR.UI
                    || !UIDTagsToAnonymize.Contains(item.Tag))
                {
                    continue;
                }
                
                var originalUIDs = ((DicomUniqueIdentifier)item).Get<DicomUID[]>();
                var currentAnonymizedUIDs = new DicomUID[originalUIDs.Length];

                var hasChanged = false;
                for (int i = 0; i < originalUIDs.Length; i++)
                {
                    var originalUID = originalUIDs[i];
                    if (anonymizedUIDs.TryGetValue(originalUID.UID, out var anonymizedUID) && originalUID.UID != anonymizedUID.UID)
                    {
                        currentAnonymizedUIDs[i] = anonymizedUID;
                        hasChanged = true;
                        continue;
                    }
                
                    using (await KeyedSemaphore.LockAsync(originalUID.UID))
                    {
                        anonymizedUID = anonymizedUIDs.GetOrAdd(originalUID.UID, _ => DicomUIDGenerator.GenerateDerivedFromUUID());
                        currentAnonymizedUIDs[i] = anonymizedUID;
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

    private static readonly ISet<DicomTag> UIDTagsToAnonymize = new []
    {
        DicomTag.AcquisitionUID,
        DicomTag.AffectedSOPInstanceUID,
        DicomTag.AnnotationGroupUID,
        DicomTag.ConcatenationUID,
        DicomTag.ConceptualVolumeUID,
        DicomTag.ConstituentConceptualVolumeUID,
        DicomTag.DeviceUID,
        DicomTag.DigitalSignatureUID,
        DicomTag.DimensionOrganizationUID,
        DicomTag.DoseReferenceUID,
        DicomTag.DosimetricObjectiveUID,
        DicomTag.FailedSOPInstanceUIDList,
        DicomTag.FiducialUID,
        DicomTag.FrameOfReferenceUID,
        DicomTag.InstanceCreatorUID,
        DicomTag.IrradiationEventUID,
        DicomTag.LargePaletteColorLookupTableUIDRETIRED,
        DicomTag.ManufacturerDeviceClassUID,
        DicomTag.MediaStorageSOPInstanceUID,
        DicomTag.MultiplexGroupUID,
        DicomTag.ObservationSubjectUIDTrialRETIRED,
        DicomTag.ObservationUID,
        DicomTag.PaletteColorLookupTableUID,
        DicomTag.PatientSetupUIDRETIRED,
        DicomTag.PresentationDisplayCollectionUID,
        DicomTag.PresentationSequenceCollectionUID,
        DicomTag.PyramidUID,
        DicomTag.ReferencedConceptualVolumeUID,
        DicomTag.ReferencedDoseReferenceUID,
        DicomTag.ReferencedDosimetricObjectiveUID,
        DicomTag.ReferencedFiducialsUID,
        DicomTag.ReferencedFrameOfReferenceUID,
        DicomTag.ReferencedGeneralPurposeScheduledProcedureStepTransactionUIDRETIRED,
        DicomTag.ReferencedObservationUIDTrialRETIRED,
        DicomTag.ReferencedSOPInstanceUID,
        DicomTag.ReferencedSOPInstanceUIDInFile,
        DicomTag.ReferencedTreatmentPositionGroupUID,
        DicomTag.RelatedFrameOfReferenceUIDRETIRED,
        DicomTag.RequestedSOPInstanceUID,
        DicomTag.RTTreatmentPhaseUID,
        DicomTag.SeriesInstanceUID,
        DicomTag.SOPInstanceUID,
        DicomTag.SourceConceptualVolumeUID,
        DicomTag.SpecimenUID,
        DicomTag.StorageMediaFileSetUID,
        DicomTag.StudyInstanceUID,
        DicomTag.SynchronizationFrameOfReferenceUID,
        DicomTag.TargetUID,
        DicomTag.TemplateExtensionCreatorUIDRETIRED,
        DicomTag.TemplateExtensionOrganizationUIDRETIRED,
        DicomTag.TrackingUID,
        DicomTag.TransactionUID,
        DicomTag.TreatmentPositionGroupUID,
        DicomTag.TreatmentSessionUID,
        DicomTag.UID,
        DicomTag.Unknown
    }.ToHashSet();
}
