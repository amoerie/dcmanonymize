using FellowOakDicom;

namespace DcmAnonymize.Instance;

public record AnonymizedInstance(DicomUID SopInstanceUID, string InstanceCreationDate, string InstanceCreationTime);
