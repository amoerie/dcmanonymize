using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using FellowOakDicom;
using FellowOakDicom.Imaging;
using FellowOakDicom.Imaging.Codec;
using FellowOakDicom.IO.Buffer;

namespace DcmAnonymize.Blanking;

public class BlankingAnonymizer
{
    public Task AnonymizeAsync(DicomAnonymizationContext context)
    {
        if (context.Options.RectanglesToBlank.Count == 0)
        {
            return Task.CompletedTask;
        }

        var metaInfo = context.MetaInfo;
        var originalDataset = context.Dataset;
        var decodedDataset = metaInfo.TransferSyntax.IsEncapsulated
            ? originalDataset.Clone(DicomTransferSyntax.ExplicitVRLittleEndian)
            : originalDataset.Clone();
        var newDataSet = decodedDataset.Clone();

        var numberOfFrames = decodedDataset.GetSingleValueOrDefault(DicomTag.NumberOfFrames, 1);
        var originalPixelData = DicomPixelData.Create(decodedDataset);
        var newPixelData = DicomPixelData.Create(newDataSet, true);
        var photometricInterpration = originalPixelData.PhotometricInterpretation;
        
        var bytesPerPixel = newPixelData.BytesAllocated * newPixelData.SamplesPerPixel;
        var rowLength = bytesPerPixel * newPixelData.Width;

        var transforms = new List<Func<IByteBuffer,IByteBuffer>>(); 
        
        if (originalPixelData.PlanarConfiguration == PlanarConfiguration.Planar)
        {
            newPixelData.PlanarConfiguration = PlanarConfiguration.Interleaved;
            transforms.Add(PixelDataConverter.PlanarToInterleaved24);
        }
        
        if (photometricInterpration == PhotometricInterpretation.YbrFull)
        {
            transforms.Add(PixelDataConverter.YbrFullToRgb);
            newPixelData.PhotometricInterpretation = PhotometricInterpretation.Rgb;
        }
        else if (photometricInterpration == PhotometricInterpretation.YbrFull422)
        {
            transforms.Add(buffer => PixelDataConverter.YbrFull422ToRgb(buffer, originalPixelData.Width));
            newPixelData.PhotometricInterpretation = PhotometricInterpretation.Rgb;
        }
        else  if (photometricInterpration == PhotometricInterpretation.YbrPartial422)
        {
            transforms.Add(buffer => PixelDataConverter.YbrPartial422ToRgb(buffer, originalPixelData.Width));
            newPixelData.PhotometricInterpretation = PhotometricInterpretation.Rgb;
        }
        
        for (var frame = 0; frame < numberOfFrames; frame++)
        {
            var framePixelData = originalPixelData.GetFrame(frame);

            foreach (var transform in transforms)
            {
                framePixelData = transform(framePixelData);
            }

            var bytes = new byte[framePixelData.Size];
            framePixelData.CopyToStream(new MemoryStream(bytes));

            foreach (var rectangle in context.Options.RectanglesToBlank)
            {
                var (x1, y1, x2, y2) = rectangle;
                var offsetX = x1 * bytesPerPixel;
                var widthX = (x2 - x1) * bytesPerPixel;

                for (var offsetY = y1 * rowLength; offsetY < y2 * rowLength; offsetY += rowLength)
                {
                    Array.Fill(bytes, (byte) 0, offsetY + offsetX, widthX);
                }
            }

            newPixelData.AddFrame(new MemoryByteBuffer(bytes));
        }

        var encodedDataset = metaInfo.TransferSyntax.IsEncapsulated
            ? newDataSet.Clone(metaInfo.TransferSyntax)
            : newDataSet;

        var encodedPixelData = encodedDataset.GetDicomItem<DicomItem>(DicomTag.PixelData);

        originalDataset.AddOrUpdate(encodedPixelData);

        return Task.CompletedTask;
    }

}
