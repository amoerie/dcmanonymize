using System;
using DcmAnonymize.Names;
using FellowOakDicom;

namespace DcmAnonymize;

public class DicomTagCleaner
{
    private readonly RandomNameGenerator _randomNameGenerator;

    public DicomTagCleaner(RandomNameGenerator randomNameGenerator)
    {
        _randomNameGenerator = randomNameGenerator ?? throw new ArgumentNullException(nameof(randomNameGenerator));
    }

    public void Clean(DicomDataset dataset, DicomItem item)
    {
        var tag = item.Tag;
        var vr = item.ValueRepresentation;
        switch (vr.Code)
        {
            case DicomVRCode.PN:
                // Generate random name
                var randomName = _randomNameGenerator.GenerateRandomName();
                dataset.AddOrUpdate(new DicomPersonName(tag, randomName.LastName, randomName.FirstName));
                break;
            case DicomVRCode.SQ:
                dataset.AddOrUpdate<DicomDataset>(DicomVR.SQ, tag);
                break;
            case DicomVRCode.OB:
            case DicomVRCode.OD:
            case DicomVRCode.OF:
            case DicomVRCode.OL:
            case DicomVRCode.OV:
            case DicomVRCode.OW:
            case DicomVRCode.UN:
                dataset.AddOrUpdate(tag, Array.Empty<byte>());
                break;
            case DicomVRCode.FL:
            case DicomVRCode.FD:
            case DicomVRCode.IS:
            case DicomVRCode.SL:
            case DicomVRCode.SS:
            case DicomVRCode.SV:
            case DicomVRCode.UL:
            case DicomVRCode.US:
            case DicomVRCode.UV:
            case DicomVRCode.DS:
                dataset.AddOrUpdate(tag, 0);
                break;
            default:
                dataset.AddOrUpdate(tag, string.Empty);
                break;
        }
    }
}
