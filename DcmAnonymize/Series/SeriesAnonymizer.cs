using System;
using System.Collections.Generic;
using DcmAnonymize.Names;
using Dicom;

namespace DcmAnonymize
{
    public class SeriesAnonymizer
    {
        private readonly IDictionary<string, AnonymizedSeries> _anonymizedStudies = new Dictionary<string, AnonymizedSeries>();

        public void Anonymize(DicomDataset dicomDataSet)
        {
            var originalSeriesInstanceUID = dicomDataSet.GetSingleValue<string>(DicomTag.SeriesInstanceUID);
    
            if (!_anonymizedStudies.TryGetValue(originalSeriesInstanceUID, out var anonymizedSeries))
            {
                anonymizedSeries = new AnonymizedSeries();

                anonymizedSeries.SeriesInstanceUID = DicomUIDGenerator.GenerateDerivedFromUUID().UID;
                anonymizedSeries.SeriesDateTime = DateTime.Now;

                _anonymizedStudies[originalSeriesInstanceUID] = anonymizedSeries;
            }

            dicomDataSet.AddOrUpdate(DicomTag.SeriesInstanceUID, anonymizedSeries.SeriesInstanceUID);
            dicomDataSet.AddOrUpdate(DicomTag.SeriesDate, anonymizedSeries.SeriesDateTime.ToString("yyyyMMdd"));
            dicomDataSet.AddOrUpdate(DicomTag.SeriesTime, anonymizedSeries.SeriesDateTime.ToString("HHmmss"));
        }
    }
}