using System;
using DcmAnonymize.Names;

namespace DcmAnonymize
{
    public record AnonymizedStudy(
        string StudyInstanceUID,
        string AccessionNumber,
        DateTime StudyDateTime,
        RandomName RequestingPhysician,
        RandomName PerformingPhysician,
        string StudyId,
        string InstitutionName
    );
}
