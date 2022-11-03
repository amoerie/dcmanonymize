using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using FellowOakDicom;
using KeyedSemaphores;

namespace DcmAnonymize.Series;

public class SeriesAnonymizer
{
    private readonly ConcurrentDictionary<string, AnonymizedSeries> _anonymizedStudies = new ConcurrentDictionary<string, AnonymizedSeries>();

    public async Task AnonymizeAsync(DicomFileMetaInformation metaInfo, DicomDataset dicomDataSet)
    {
        var originalSeriesInstanceUID = dicomDataSet.GetSingleValue<string>(DicomTag.SeriesInstanceUID);
    
        if (!_anonymizedStudies.TryGetValue(originalSeriesInstanceUID, out var anonymizedSeries))
        {
            var key = $"SERIES_{originalSeriesInstanceUID}";
            using (await KeyedSemaphore.LockAsync(key))
            {
                if (!_anonymizedStudies.TryGetValue(originalSeriesInstanceUID, out anonymizedSeries))
                {
                    var seriesInstanceUID = DicomUIDGenerator.GenerateDerivedFromUUID().UID;
                    var seriesDateTime = DateTime.Now;

                    anonymizedSeries = new AnonymizedSeries(seriesInstanceUID, seriesDateTime);
                        
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