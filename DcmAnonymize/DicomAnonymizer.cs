using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DcmAnonymize.Instance;
using DcmAnonymize.Order;
using DcmAnonymize.Patient;
using DcmAnonymize.Recursive;
using DcmAnonymize.Series;
using DcmAnonymize.Study;
using FellowOakDicom;

namespace DcmAnonymize;

public class DicomAnonymizer
{
    private readonly PatientAnonymizer _patientAnonymizer;
    private readonly StudyAnonymizer _studyAnonymizer;
    private readonly SeriesAnonymizer _seriesAnonymizer;
    private readonly InstanceAnonymizer _instanceAnonymizer;
    private readonly RecursiveAnonymizer _recursiveAnonymizer;
    private readonly OrderAnonymizer _orderAnonymizer;
    private readonly ConcurrentDictionary<string, DicomUID> _anonymizedUIDs = new ConcurrentDictionary<string, DicomUID>();

    public DicomAnonymizer(PatientAnonymizer patientAnonymizer, StudyAnonymizer studyAnonymizer,
        SeriesAnonymizer seriesAnonymizer, InstanceAnonymizer instanceAnonymizer, 
        RecursiveAnonymizer recursiveAnonymizer, OrderAnonymizer orderAnonymizer)
    {
        _patientAnonymizer = patientAnonymizer ?? throw new ArgumentNullException(nameof(patientAnonymizer));
        _studyAnonymizer = studyAnonymizer ?? throw new ArgumentNullException(nameof(studyAnonymizer));
        _seriesAnonymizer = seriesAnonymizer ?? throw new ArgumentNullException(nameof(seriesAnonymizer));
        _instanceAnonymizer = instanceAnonymizer ?? throw new ArgumentNullException(nameof(instanceAnonymizer));
        _recursiveAnonymizer = recursiveAnonymizer ?? throw new ArgumentNullException(nameof(recursiveAnonymizer));
        _orderAnonymizer = orderAnonymizer ?? throw new ArgumentNullException(nameof(orderAnonymizer));
    }

    public Task AnonymizeAsync(DicomFile dicomFile)
    {
        return AnonymizeAsync(dicomFile.FileMetaInfo, dicomFile.Dataset);
    }

    public async Task AnonymizeAsync(DicomFileMetaInformation metaInfo, DicomDataset dataset)
    {
        var context = new DicomAnonymizationContext(metaInfo, dataset, _anonymizedUIDs);
        await _orderAnonymizer.AnonymizeAsync(context);
        await _patientAnonymizer.AnonymizeAsync(context);
        await _studyAnonymizer.AnonymizeAsync(context);
        await _seriesAnonymizer.AnonymizeAsync(context);
        await _instanceAnonymizer.AnonymizeAsync(context);
        await _recursiveAnonymizer.AnonymizeAsync(context);
    }
    
}
