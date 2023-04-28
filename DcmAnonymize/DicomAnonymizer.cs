using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DcmAnonymize.Instance;
using DcmAnonymize.Patient;
using DcmAnonymize.Series;
using DcmAnonymize.Study;
using DcmAnonymize.UIDs;
using FellowOakDicom;

namespace DcmAnonymize;

public class DicomAnonymizer
{
    private readonly PatientAnonymizer _patientAnonymizer;
    private readonly StudyAnonymizer _studyAnonymizer;
    private readonly SeriesAnonymizer _seriesAnonymizer;
    private readonly InstanceAnonymizer _instanceAnonymizer;
    private readonly UIDsAnonymizer _uidsAnonymizer;
    private readonly ConcurrentDictionary<string, DicomUID> _anonymizedUIDs = new ConcurrentDictionary<string, DicomUID>();

    public DicomAnonymizer(PatientAnonymizer patientAnonymizer, StudyAnonymizer studyAnonymizer,
        SeriesAnonymizer seriesAnonymizer, InstanceAnonymizer instanceAnonymizer, UIDsAnonymizer uidsAnonymizer)
    {
        _patientAnonymizer = patientAnonymizer ?? throw new ArgumentNullException(nameof(patientAnonymizer));
        _studyAnonymizer = studyAnonymizer ?? throw new ArgumentNullException(nameof(studyAnonymizer));
        _seriesAnonymizer = seriesAnonymizer ?? throw new ArgumentNullException(nameof(seriesAnonymizer));
        _instanceAnonymizer = instanceAnonymizer ?? throw new ArgumentNullException(nameof(instanceAnonymizer));
        _uidsAnonymizer = uidsAnonymizer;
    }

    public Task AnonymizeAsync(DicomFile dicomFile)
    {
        return AnonymizeAsync(dicomFile.FileMetaInfo, dicomFile.Dataset);
    }

    public async Task AnonymizeAsync(DicomFileMetaInformation metaInfo, DicomDataset dataset)
    {
        var context = new DicomAnonymizationContext(metaInfo, dataset, _anonymizedUIDs);
        await _patientAnonymizer.AnonymizeAsync(context);
        await _studyAnonymizer.AnonymizeAsync(context);
        await _seriesAnonymizer.AnonymizeAsync(context);
        await _instanceAnonymizer.AnonymizeAsync(context);
        await _uidsAnonymizer.AnonymizeAsync(context);
        
        
    }
    
}
