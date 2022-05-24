using System;

namespace DcmAnonymize.Series
{
    public record AnonymizedSeries(string SeriesInstanceUID, DateTime SeriesDateTime);
}
