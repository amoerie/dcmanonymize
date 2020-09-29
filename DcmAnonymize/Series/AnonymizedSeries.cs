using System;

namespace DcmAnonymize
{
    public class AnonymizedSeries
    {
        public string SeriesInstanceUID { get; set; }
        public DateTime SeriesDateTime { get; set; }
    }
}