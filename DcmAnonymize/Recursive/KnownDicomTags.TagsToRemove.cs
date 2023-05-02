using System.Collections.Generic;
using System.Linq;
using FellowOakDicom;

namespace DcmAnonymize.Recursive;

public static partial class KnownDicomTags
{
    public static readonly ISet<DicomTag> TagsToRemove = new[]
    {
        DicomTag.AcquisitionCommentsRETIRED,
        DicomTag.AcquisitionContextSequence,
        DicomTag.AcquisitionDateTime,
        DicomTag.AcquisitionDeviceProcessingDescription,
        DicomTag.AcquisitionProtocolDescription,
        DicomTag.ActualHumanPerformersSequence,
        DicomTag.AdditionalPatientHistory,
        DicomTag.AddressTrialRETIRED,
        DicomTag.AdmissionID,
        DicomTag.AdmittingDate,
        DicomTag.AdmittingDiagnosesCodeSequence,
        DicomTag.AdmittingDiagnosesDescription,
        DicomTag.AdmittingTime,
        DicomTag.AffectedSOPInstanceUID,
        DicomTag.Allergies,
        DicomTag.AnnotationGroupDescription,
        DicomTag.ApprovalStatusDateTime,
        DicomTag.ArbitraryRETIRED,
        DicomTag.AssertionExpirationDateTime,
        DicomTag.AudioCommentsRETIRED,
        DicomTag.AudioSampleDataRETIRED,
        DicomTag.AudioSampleFormatRETIRED,
        DicomTag.AudioTypeRETIRED,
        DicomTag.AuthorObserverSequence,
        DicomTag.AxisLabelsRETIRED,
        DicomTag.AxisUnitsRETIRED,
        DicomTag.BarcodeValue,
        DicomTag.BeamDescription,
        DicomTag.BolusDescription,
        DicomTag.BranchOfService,
        DicomTag.CalibrationDate,
        DicomTag.CalibrationTime,
        DicomTag.CameraOwnerName,
        DicomTag.CassetteID,
        DicomTag.CertifiedTimestamp,
        DicomTag.ClinicalTrialProtocolEthicsCommitteeApprovalNumber,
        DicomTag.ClinicalTrialSeriesDescription,
        DicomTag.ClinicalTrialSeriesID,
        DicomTag.ClinicalTrialTimePointDescription,
        DicomTag.CommentsOnRadiationDose,
        DicomTag.CommentsOnThePerformedProcedureStep,
        DicomTag.CompensatorDescription,
        DicomTag.ConfidentialityConstraintOnPatientDataDescription,
        DicomTag.ConsultingPhysicianIdentificationSequence,
        DicomTag.ContainerComponentID,
        DicomTag.ContainerDescription,
        DicomTag.ContentCreatorIdentificationCodeSequence,
        DicomTag.ContrastBolusStartTime,
        DicomTag.ContrastBolusStopTime,
        DicomTag.ContributionDateTime,
        DicomTag.ContributionDescription,
        DicomTag.CoordinateStartValueRETIRED,
        DicomTag.CoordinateStepValueRETIRED,
        DicomTag.CountryOfResidence,
        DicomTag.CreationDate,
        DicomTag.CreationTime,
        DicomTag.CurrentObserverTrialRETIRED,
        DicomTag.CurrentPatientLocation,
        DicomTag.CurveActivationLayerRETIRED,
        DicomTag.CurveDataDescriptorRETIRED,
        DicomTag.CurveDataRETIRED,
        DicomTag.CurveDateRETIRED,
        DicomTag.CurveDescriptionRETIRED,
        DicomTag.CurveDimensionsRETIRED,
        DicomTag.CurveLabelRETIRED,
        DicomTag.CurveRangeRETIRED,
        DicomTag.CurveReferencedOverlayGroupRETIRED,
        DicomTag.CurveReferencedOverlaySequenceRETIRED,
        DicomTag.CurveTimeRETIRED,
        DicomTag.CustodialOrganizationSequence,
        DicomTag.DataSetTrailingPadding,
        DicomTag.DataValueRepresentationRETIRED,
        DicomTag.DateOfDocumentOrVerbalTransactionTrialRETIRED,
        DicomTag.DateOfLastCalibration,
        DicomTag.DateOfLastDetectorCalibration,
        DicomTag.DateOfSecondaryCapture,
        DicomTag.DateTimeOfLastCalibration,
        DicomTag.DecompositionDescription,
        DicomTag.DerivationDescription,
        DicomTag.DetectorID,
        DicomTag.DeviceDescription,
        DicomTag.DeviceSerialNumber,
        DicomTag.DeviceSettingDescription,
        DicomTag.DigitalSignaturesSequence,
        DicomTag.DischargeDateRETIRED,
        DicomTag.DischargeDiagnosisDescriptionRETIRED,
        DicomTag.DischargeTimeRETIRED,
        DicomTag.DisplacementReferenceLabel,
        DicomTag.DistributionAddressRETIRED,
        DicomTag.DistributionNameRETIRED,
        DicomTag.DoseReferenceDescription,
        DicomTag.EndAcquisitionDateTime,
        DicomTag.EntityDescription,
        DicomTag.EntityName,
        DicomTag.EquipmentFrameOfReferenceDescription,
        DicomTag.EthicsCommitteeApprovalEffectivenessEndDate,
        DicomTag.EthicsCommitteeApprovalEffectivenessStartDate,
        DicomTag.EthnicGroup,
        DicomTag.ExpectedCompletionDateTime,
        DicomTag.FindingsGroupRecordingDateTrialRETIRED,
        DicomTag.FindingsGroupRecordingTimeTrialRETIRED,
        DicomTag.FirstTreatmentDate,
        DicomTag.FixationDeviceDescription,
        DicomTag.FractionGroupDescription,
        DicomTag.FrameComments,
        DicomTag.GantryID,
        DicomTag.GeneratorID,
        DicomTag.GPSAltitude,
        DicomTag.GPSAltitudeRef,
        DicomTag.GPSAreaInformation,
        DicomTag.GPSDateStamp,
        DicomTag.GPSDestBearing,
        DicomTag.GPSDestBearingRef,
        DicomTag.GPSDestDistance,
        DicomTag.GPSDestDistanceRef,
        DicomTag.GPSDestLatitude,
        DicomTag.GPSDestLatitudeRef,
        DicomTag.GPSDestLongitude,
        DicomTag.GPSDestLongitudeRef,
        DicomTag.GPSDifferential,
        DicomTag.GPSDOP,
        DicomTag.GPSImgDirection,
        DicomTag.GPSImgDirectionRef,
        DicomTag.GPSLatitude,
        DicomTag.GPSLatitudeRef,
        DicomTag.GPSLongitude,
        DicomTag.GPSLongitudeRef,
        DicomTag.GPSMapDatum,
        DicomTag.GPSMeasureMode,
        DicomTag.GPSProcessingMethod,
        DicomTag.GPSSatellites,
        DicomTag.GPSSpeed,
        DicomTag.GPSSpeedRef,
        DicomTag.GPSStatus,
        DicomTag.GPSTimeStamp,
        DicomTag.GPSTrack,
        DicomTag.GPSTrackRef,
        DicomTag.GPSVersionID,
        DicomTag.HL7DocumentEffectiveTime,
        DicomTag.HumanPerformerName,
        DicomTag.HumanPerformerOrganization,
        DicomTag.IconImageSequence,
        DicomTag.IdentifyingCommentsRETIRED,
        DicomTag.ImageComments,
        DicomTag.ImagePresentationCommentsRETIRED,
        DicomTag.ImagingServiceRequestComments,
        DicomTag.ImpressionsRETIRED,
        DicomTag.InstanceCoercionDateTime,
        DicomTag.InstanceOriginStatus,
        DicomTag.InstitutionAddress,
        DicomTag.InstitutionalDepartmentName,
        DicomTag.InstitutionalDepartmentTypeCodeSequence,
        DicomTag.InstitutionCodeSequence,
        DicomTag.InsurancePlanIdentificationRETIRED,
        DicomTag.IntendedFractionStartTime,
        DicomTag.IntendedPhaseEndDate,
        DicomTag.IntendedPhaseStartDate,
        DicomTag.IntendedRecipientsOfResultsIdentificationSequence,
        DicomTag.InterpretationApprovalDateRETIRED,
        DicomTag.InterpretationApprovalTimeRETIRED,
        DicomTag.InterpretationApproverSequenceRETIRED,
        DicomTag.InterpretationAuthorRETIRED,
        DicomTag.InterpretationDiagnosisDescriptionRETIRED,
        DicomTag.InterpretationIDIssuerRETIRED,
        DicomTag.InterpretationIDRETIRED,
        DicomTag.InterpretationRecordedDateRETIRED,
        DicomTag.InterpretationRecordedTimeRETIRED,
        DicomTag.InterpretationRecorderRETIRED,
        DicomTag.InterpretationTextRETIRED,
        DicomTag.InterpretationTranscriberRETIRED,
        DicomTag.InterpretationTranscriptionDateRETIRED,
        DicomTag.InterpretationTranscriptionTimeRETIRED,
        DicomTag.InterventionDrugStartTime,
        DicomTag.InterventionDrugStopTime,
        DicomTag.IssueDateOfImagingServiceRequest,
        DicomTag.IssuerOfAdmissionIDRETIRED,
        DicomTag.IssuerOfAdmissionIDSequence,
        DicomTag.IssuerOfPatientID,
        DicomTag.IssuerOfServiceEpisodeIDRETIRED,
        DicomTag.IssuerOfServiceEpisodeIDSequence,
        DicomTag.IssueTimeOfImagingServiceRequest,
        DicomTag.LabelText,
        DicomTag.LastMenstrualDate,
        DicomTag.LensMake,
        DicomTag.LensModel,
        DicomTag.LensSerialNumber,
        DicomTag.LensSpecification,
        DicomTag.LongDeviceDescription,
        DicomTag.MAC,
        DicomTag.MakerNote,
        DicomTag.MaximumCoordinateValueRETIRED,
        DicomTag.MedicalAlerts,
        DicomTag.MedicalRecordLocatorRETIRED,
        DicomTag.MilitaryRank,
        DicomTag.MinimumCoordinateValueRETIRED,
        DicomTag.ModifiedAttributesSequence,
        DicomTag.ModifiedImageDateRETIRED,
        DicomTag.ModifiedImageDescriptionRETIRED,
        DicomTag.ModifiedImageTimeRETIRED,
        DicomTag.ModifyingDeviceIDRETIRED,
        DicomTag.MostRecentTreatmentDate,
        DicomTag.MultienergyAcquisitionDescription,
        DicomTag.NamesOfIntendedRecipientsOfResults,
        DicomTag.NonconformingDataElementValue,
        DicomTag.NonconformingModifiedAttributesSequence,
        DicomTag.NumberOfChannelsRETIRED,
        DicomTag.NumberOfPointsRETIRED,
        DicomTag.NumberOfSamplesRETIRED,
        DicomTag.ObservationDateTime,
        DicomTag.ObservationDateTrialRETIRED,
        DicomTag.ObservationStartDateTime,
        DicomTag.ObservationTimeTrialRETIRED,
        DicomTag.Occupation,
        DicomTag.OperatorIdentificationSequence,
        DicomTag.OperatorsName,
        DicomTag.OrderCallbackPhoneNumber,
        DicomTag.OrderCallbackTelecomInformation,
        DicomTag.OrderEnteredBy,
        DicomTag.OrderEntererLocation,
        DicomTag.OriginalAttributesSequence,
        DicomTag.OtherPatientIDsRETIRED,
        DicomTag.OtherPatientIDsSequence,
        DicomTag.OtherPatientNames,
        DicomTag.OverlayCommentsRETIRED,
        DicomTag.OverlayData,
        DicomTag.OverlayDateRETIRED,
        DicomTag.OverlayTimeRETIRED,
        DicomTag.ParticipantSequence,
        DicomTag.PatientAddress,
        DicomTag.PatientBirthName,
        DicomTag.PatientBirthTime,
        DicomTag.PatientComments,
        DicomTag.PatientInstitutionResidence,
        DicomTag.PatientInsurancePlanCodeSequence,
        DicomTag.PatientMotherBirthName,
        DicomTag.PatientPrimaryLanguageCodeSequence,
        DicomTag.PatientPrimaryLanguageModifierCodeSequence,
        DicomTag.PatientReligiousPreference,
        DicomTag.PatientSetupPhotoDescription,
        DicomTag.PatientSexNeutered,
        DicomTag.PatientSize,
        DicomTag.PatientState,
        DicomTag.PatientTelecomInformation,
        DicomTag.PatientTelephoneNumbers,
        DicomTag.PatientTransportArrangements,
        DicomTag.PatientTreatmentPreparationMethodDescription,
        DicomTag.PatientTreatmentPreparationProcedureParameterDescription,
        DicomTag.PatientWeight,
        DicomTag.PerformedLocation,
        DicomTag.PerformedProcedureStepDescription,
        DicomTag.PerformedProcedureStepEndDate,
        DicomTag.PerformedProcedureStepEndDateTime,
        DicomTag.PerformedProcedureStepEndTime,
        DicomTag.PerformedProcedureStepID,
        DicomTag.PerformedProcedureStepStartDate,
        DicomTag.PerformedProcedureStepStartDateTime,
        DicomTag.PerformedProcedureStepStartTime,
        DicomTag.PerformedStationAETitle,
        DicomTag.PerformedStationGeographicLocationCodeSequence,
        DicomTag.PerformedStationName,
        DicomTag.PerformedStationNameCodeSequence,
        DicomTag.PerformingPhysicianIdentificationSequence,
        DicomTag.PersonAddress,
        DicomTag.PersonTelecomInformation,
        DicomTag.PersonTelephoneNumbers,
        DicomTag.PhysicianApprovingInterpretationRETIRED,
        DicomTag.PhysiciansOfRecord,
        DicomTag.PhysiciansOfRecordIdentificationSequence,
        DicomTag.PhysiciansReadingStudyIdentificationSequence,
        DicomTag.PlateID,
        DicomTag.PregnancyStatus,
        DicomTag.PreMedication,
        DicomTag.PrescriptionDescription,
        DicomTag.PresentationCreationDate,
        DicomTag.PresentationCreationTime,
        DicomTag.PriorTreatmentDoseDescription,
        DicomTag.ProcedureStepCancellationDateTime,
        DicomTag.ProductExpirationDateTime,
        DicomTag.ProtocolName,
        DicomTag.PyramidDescription,
        DicomTag.PyramidLabel,
        DicomTag.RadiopharmaceuticalStartDateTime,
        DicomTag.RadiopharmaceuticalStartTime,
        DicomTag.RadiopharmaceuticalStopDateTime,
        DicomTag.RadiopharmaceuticalStopTime,
        DicomTag.ReasonForOmissionDescription,
        DicomTag.ReasonForRequestedProcedureCodeSequence,
        DicomTag.ReasonForStudyRETIRED,
        DicomTag.ReasonForTheImagingServiceRequestRETIRED,
        DicomTag.ReasonForTheRequestedProcedure,
        DicomTag.ReasonForVisit,
        DicomTag.ReasonForVisitCodeSequence,
        DicomTag.ReferencedDigitalSignatureSequence,
        DicomTag.ReferencedImageSequence,
        DicomTag.ReferencedPatientAliasSequence,
        DicomTag.ReferencedPatientPhotoSequence,
        DicomTag.ReferencedPatientSequence,
        DicomTag.ReferencedPerformedProcedureStepSequence,
        DicomTag.ReferencedSOPInstanceMACSequence,
        DicomTag.ReferencedStudySequence,
        DicomTag.ReferringPhysicianAddress,
        DicomTag.ReferringPhysicianIdentificationSequence,
        DicomTag.ReferringPhysicianTelephoneNumbers,
        DicomTag.RegionOfResidence,
        DicomTag.RequestAttributesSequence,
        DicomTag.RequestedContrastAgent,
        DicomTag.RequestedProcedureComments,
        DicomTag.RequestedProcedureDescription,
        DicomTag.RequestedProcedureID,
        DicomTag.RequestedProcedureLocation,
        DicomTag.RequestedSeriesDescription,
        DicomTag.RequestingService,
        DicomTag.RespiratoryMotionCompensationTechniqueDescription,
        DicomTag.ResponsibleOrganization,
        DicomTag.ResponsiblePerson,
        DicomTag.ResultsCommentsRETIRED,
        DicomTag.ResultsDistributionListSequenceRETIRED,
        DicomTag.ResultsIDIssuerRETIRED,
        DicomTag.ResultsIDRETIRED,
        DicomTag.ReviewerName,
        DicomTag.ROIDescription,
        DicomTag.ROIGenerationDescription,
        DicomTag.ROIObservationDescription,
        DicomTag.ROIObservationLabel,
        DicomTag.RTPlanDate,
        DicomTag.RTPlanDescription,
        DicomTag.RTPlanName,
        DicomTag.RTPlanTime,
        DicomTag.RTTreatmentApproachLabel,
        DicomTag.SampleRateRETIRED,
        DicomTag.ScheduledAdmissionDateRETIRED,
        DicomTag.ScheduledAdmissionTimeRETIRED,
        DicomTag.ScheduledDischargeDateRETIRED,
        DicomTag.ScheduledDischargeTimeRETIRED,
        DicomTag.ScheduledHumanPerformersSequence,
        DicomTag.ScheduledPatientInstitutionResidenceRETIRED,
        DicomTag.ScheduledPerformingPhysicianIdentificationSequence,
        DicomTag.ScheduledPerformingPhysicianName,
        DicomTag.ScheduledProcedureStepDescription,
        DicomTag.ScheduledProcedureStepEndDate,
        DicomTag.ScheduledProcedureStepEndTime,
        DicomTag.ScheduledProcedureStepExpirationDateTime,
        DicomTag.ScheduledProcedureStepID,
        DicomTag.ScheduledProcedureStepLocation,
        DicomTag.ScheduledProcedureStepModificationDateTime,
        DicomTag.ScheduledProcedureStepStartDate,
        DicomTag.ScheduledProcedureStepStartDateTime,
        DicomTag.ScheduledProcedureStepStartTime,
        DicomTag.ScheduledStationAETitle,
        DicomTag.ScheduledStationGeographicLocationCodeSequence,
        DicomTag.ScheduledStationName,
        DicomTag.ScheduledStationNameCodeSequence,
        DicomTag.ScheduledStudyLocationAETitleRETIRED,
        DicomTag.ScheduledStudyLocationRETIRED,
        DicomTag.ScheduledStudyStartDateRETIRED,
        DicomTag.ScheduledStudyStartTimeRETIRED,
        DicomTag.ScheduledStudyStopDateRETIRED,
        DicomTag.ScheduledStudyStopTimeRETIRED,
        DicomTag.ServiceEpisodeDescription,
        DicomTag.ServiceEpisodeID,
        DicomTag.SetupTechniqueDescription,
        DicomTag.ShieldingDeviceDescription,
        DicomTag.SlideIdentifierRETIRED,
        DicomTag.SmokingStatus,
        DicomTag.SOPAuthorizationDateTime,
        DicomTag.SourceImageSequence,
        DicomTag.SourceManufacturer,
        DicomTag.SourceSerialNumber,
        DicomTag.SpecialNeeds,
        DicomTag.SpecimenAccessionNumberRETIRED,
        DicomTag.SpecimenDetailedDescription,
        DicomTag.SpecimenShortDescription,
        DicomTag.StartAcquisitionDateTime,
        DicomTag.StationName,
        DicomTag.StructureSetDescription,
        DicomTag.StructureSetName,
        DicomTag.StudyArrivalDateRETIRED,
        DicomTag.StudyArrivalTimeRETIRED,
        DicomTag.StudyCommentsRETIRED,
        DicomTag.StudyCompletionDateRETIRED,
        DicomTag.StudyCompletionTimeRETIRED,
        DicomTag.StudyDescription,
        DicomTag.StudyIDIssuerRETIRED,
        DicomTag.StudyReadDateRETIRED,
        DicomTag.StudyReadTimeRETIRED,
        DicomTag.StudyVerifiedDateRETIRED,
        DicomTag.StudyVerifiedTimeRETIRED,
        DicomTag.SubstanceAdministrationDateTime,
        DicomTag.TelephoneNumberTrialRETIRED,
        DicomTag.TemplateLocalVersionRETIRED,
        DicomTag.TemplateVersionRETIRED,
        DicomTag.TextCommentsRETIRED,
        DicomTag.TextString,
        DicomTag.TimeOfDocumentCreationOrVerbalTransactionTrialRETIRED,
        DicomTag.TimeOfLastCalibration,
        DicomTag.TimeOfLastDetectorCalibration,
        DicomTag.TimeOfSecondaryCapture,
        DicomTag.TimezoneOffsetFromUTC,
        DicomTag.TopicAuthorRETIRED,
        DicomTag.TopicKeywordsRETIRED,
        DicomTag.TopicSubjectRETIRED,
        DicomTag.TopicTitleRETIRED,
        DicomTag.TotalTimeRETIRED,
        DicomTag.TransducerIdentificationSequence,
        DicomTag.TreatmentDate,
        DicomTag.TreatmentMachineName,
        DicomTag.TreatmentSitesRETIRED,
        DicomTag.TreatmentTime,
        DicomTag.TypeOfDataRETIRED,
        DicomTag.UDISequence,
        DicomTag.UniqueDeviceIdentifier,
        DicomTag.Unknown,
        DicomTag.VerbalSourceIdentifierCodeSequenceTrialRETIRED,
        DicomTag.VerbalSourceTrialRETIRED,
        DicomTag.VisitComments,
        DicomTag.XRayDetectorLabel
    }.ToHashSet();
}
