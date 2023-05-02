using System;
using FellowOakDicom;

namespace DcmAnonymize.Series;

public record AnonymizedSeries(DicomUID SeriesInstanceUID, DateTime SeriesDateTime);
