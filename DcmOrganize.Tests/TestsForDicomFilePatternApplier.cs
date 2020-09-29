using System;
using System.IO;
using Dicom;
using FluentAssertions;
using Xunit;

namespace DcmOrganize.Tests
{
    public class TestsForDicomFilePatternApplier
    {
        [Fact]
        public void ShouldApplySimplePattern()
        {
            // Arrange
            var dicomDataSet = new DicomDataset
            {
                { DicomTag.AccessionNumber, "ABC123" },
                { DicomTag.InstanceNumber, "7" },
            };
            var pattern = "{AccessionNumber}/{InstanceNumber}.dcm";

            // Act
            var success = DicomFilePatternApplier.TryApply(dicomDataSet, pattern, out var file);

            // Assert
            success.Should().BeTrue();
            file.Should().Be(Path.Join("ABC123", "7.dcm"));
        }
        
        [Fact]
        public void ShouldApplyComplexPattern()
        {
            // Arrange
            var dicomDataSet = new DicomDataset
            {
                { DicomTag.PatientName, "Samson^Gert" },
                { DicomTag.AccessionNumber, "ABC123" },
                { DicomTag.SeriesNumber, "20" },
                { DicomTag.InstanceNumber, "7" },
            };
            var pattern = "Patient {PatientName}/Study {AccessionNumber}/Series {SeriesNumber}/Image {InstanceNumber}.dcm";

            // Act
            var success = DicomFilePatternApplier.TryApply(dicomDataSet, pattern, out var file);

            // Assert
            success.Should().BeTrue();
            file.Should().Be(Path.Join("Patient Samson Gert", "Study ABC123", "Series 20", "Image 7.dcm"));
        }
        
        [Fact]
        public void ShouldUseValueWhenPatternContainsFallbackAndValueIsPresent()
        {
            // Arrange
            var dicomDataSet = new DicomDataset
            {
                { DicomTag.SOPInstanceUID, "1.2.3" },
                { DicomTag.InstanceNumber, "10" },
            };
            var pattern = "{InstanceNumber ?? SOPInstanceUID}.dcm";

            // Act
            var success = DicomFilePatternApplier.TryApply(dicomDataSet, pattern, out var file);

            // Assert
            success.Should().BeTrue();
            file.Should().Be("10.dcm");
        }
        
        [Fact]
        public void ShouldUseFallbackWhenPatternContainsFallbackAndValueIsNotPresent()
        {
            // Arrange
            var dicomDataSet = new DicomDataset
            {
                { DicomTag.SOPInstanceUID, "1.2.3" },
            };
            var pattern = "{InstanceNumber ?? SOPInstanceUID}.dcm";

            // Act
            var success = DicomFilePatternApplier.TryApply(dicomDataSet, pattern, out var file);

            // Assert
            success.Should().BeTrue();
            file.Should().Be("1.2.3.dcm");
        }
    }
}