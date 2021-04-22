using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dicom;
using KeyedSemaphores;

namespace DcmAnonymize.Series
{
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
                        anonymizedSeries = new AnonymizedSeries();

                        anonymizedSeries.SeriesInstanceUID = DicomUIDGenerator.GenerateDerivedFromUUID().UID;
                        anonymizedSeries.SeriesDateTime = DateTime.Now;

                        _anonymizedStudies[originalSeriesInstanceUID] = anonymizedSeries;
                    }
                }
            }

            dicomDataSet.AddOrUpdate(DicomTag.SeriesInstanceUID, anonymizedSeries.SeriesInstanceUID);
            dicomDataSet.AddOrUpdate(DicomTag.SeriesDate, anonymizedSeries.SeriesDateTime.ToString("yyyyMMdd"));
            dicomDataSet.AddOrUpdate(DicomTag.SeriesTime, anonymizedSeries.SeriesDateTime.ToString("HHmmss"));
        }
    }
}