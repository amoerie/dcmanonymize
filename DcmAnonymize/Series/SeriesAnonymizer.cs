using System;
using System.Collections.Concurrent;
using System.Globalization;
using System.Threading.Tasks;
using FellowOakDicom;
using KeyedSemaphores;

namespace DcmAnonymize.Series;

public class SeriesAnonymizer
{
    private readonly ConcurrentDictionary<string, AnonymizedSeries> _anonymizedStudies = new ConcurrentDictionary<string, AnonymizedSeries>();

    public async Task AnonymizeAsync(DicomAnonymizationContext context)
    {
        var dicomDataSet = context.Dataset;
        var anonymizedUIDs = context.AnonymizedUIDs;
        var originalSeriesInstanceUID = dicomDataSet.GetSingleValue<string>(DicomTag.SeriesInstanceUID);
    
        if (!_anonymizedStudies.TryGetValue(originalSeriesInstanceUID, out var anonymizedSeries))
        {
            using (await KeyedSemaphore.LockAsync(originalSeriesInstanceUID))
            {
                if (!_anonymizedStudies.TryGetValue(originalSeriesInstanceUID, out anonymizedSeries))
                {
                    var anonymizedSeriesInstanceUID = anonymizedUIDs.GetOrAdd(originalSeriesInstanceUID, _ => DicomUIDGenerator.GenerateDerivedFromUUID());
                    anonymizedUIDs[anonymizedSeriesInstanceUID.UID] = anonymizedSeriesInstanceUID;
                    var seriesDateTime = DateTime.Now;

                    anonymizedSeries = new AnonymizedSeries(anonymizedSeriesInstanceUID, seriesDateTime);
                        
                    _anonymizedStudies[originalSeriesInstanceUID] = anonymizedSeries;
                }
            }
        }

        dicomDataSet.AddOrUpdate(DicomTag.SeriesInstanceUID, anonymizedSeries.SeriesInstanceUID);
        dicomDataSet.AddOrUpdate(DicomTag.SeriesDate, anonymizedSeries.SeriesDateTime.ToString("yyyyMMdd", CultureInfo.InvariantCulture));
        dicomDataSet.AddOrUpdate(DicomTag.SeriesTime, anonymizedSeries.SeriesDateTime.ToString("HHmmss", CultureInfo.InvariantCulture));
        dicomDataSet.Remove(DicomTag.RelatedSeriesSequence);
    }
}
