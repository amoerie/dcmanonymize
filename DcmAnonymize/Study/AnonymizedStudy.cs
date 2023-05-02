using System;
using DcmAnonymize.Names;
using FellowOakDicom;

namespace DcmAnonymize;

public record AnonymizedStudy(
    DicomUID StudyInstanceUID,
    string AccessionNumber,
    DateTime StudyDateTime,
    RandomName RequestingPhysician,
    RandomName PerformingPhysician,
    string StudyId,
    string InstitutionName
);
