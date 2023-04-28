using System.Collections.Generic;
using System.Linq;
using FellowOakDicom;

namespace DcmAnonymize.Recursive;

public static partial class KnownDicomTags
{
    public static readonly ISet<DicomTag> UIDTagsToAnonymize = new []
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
