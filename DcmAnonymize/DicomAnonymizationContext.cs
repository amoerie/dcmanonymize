using System.Collections.Concurrent;
using FellowOakDicom;

namespace DcmAnonymize;

public sealed record DicomAnonymizationContext(DicomFileMetaInformation MetaInfo, DicomDataset Dataset, ConcurrentDictionary<string, DicomUID> AnonymizedUIDs);
