using DcmAnonymize.Names;
using DcmAnonymize.Patient;
using DcmAnonymize.Series;
using DcmAnonymize.Study;
using Dicom;
using FluentAssertions;
using Xunit;

namespace DcmAnonymize.Tests
{
    public class TestsForDicomAnonymizer
    {
        private readonly DicomAnonymizer _anonymizer;

        public TestsForDicomAnonymizer()
        {
            var randomNameGenerator = new RandomNameGenerator();
            _anonymizer = new DicomAnonymizer(
                new PatientAnonymizer(randomNameGenerator), 
                new StudyAnonymizer(randomNameGenerator),
                new SeriesAnonymizer(), 
                new InstanceAnonymizer());
        }
        
        [Fact]
        public void ShouldBeAbleToAnonymizeEmptyDataSet()
        {
            // Arrange
            var metaInfo = new DicomFileMetaInformation();
            var dicomDataSet = new DicomDataset
            {
                { DicomTag.PatientName, "Bar^Foo" },
                { DicomTag.StudyInstanceUID, "1" },
                { DicomTag.SeriesInstanceUID, "1.1" },
                { DicomTag.SOPInstanceUID, "1.1.1" },
            };
            
            // Act
            _anonymizer.AnonymizeAsync(metaInfo, dicomDataSet);
            
            // Assert
            dicomDataSet.Contains(DicomTag.PatientName).Should().BeTrue();
            dicomDataSet.GetSingleValue<string>(DicomTag.PatientName).Should().NotBe("Bar^Foo");
            dicomDataSet.GetSingleValue<string>(DicomTag.StudyInstanceUID).Should().NotBe("1");
            dicomDataSet.GetSingleValue<string>(DicomTag.SeriesInstanceUID).Should().NotBe("1.1");
            dicomDataSet.GetSingleValue<string>(DicomTag.SOPInstanceUID).Should().NotBe("1.1.1");
        }
        
        [Fact]
        public void ShouldAnonymizeSamePatient()
        {
            // Arrange
            var dicomDataSet1 = new DicomDataset
            {
                { DicomTag.PatientName, "Bar^Foo" },
                { DicomTag.StudyInstanceUID, "1" },
                { DicomTag.SeriesInstanceUID, "1.1" },
                { DicomTag.SOPInstanceUID, "1.1.1" },
            };
            var metaInfo1 = new DicomFileMetaInformation();
            var dicomDataSet2 = new DicomDataset
            {
                { DicomTag.PatientName, "Bar^Foo" },
                { DicomTag.StudyInstanceUID, "2" },
                { DicomTag.SeriesInstanceUID, "2.2" },
                { DicomTag.SOPInstanceUID, "2.2.2" },
            };
            var metaInfo2 = new DicomFileMetaInformation();
            
            // Act
            _anonymizer.AnonymizeAsync(metaInfo1, dicomDataSet1);
            _anonymizer.AnonymizeAsync(metaInfo2, dicomDataSet2);
            
            // Assert
            var patientName1 = dicomDataSet1.GetSingleValue<string>(DicomTag.PatientName);
            var patientName2 = dicomDataSet2.GetSingleValue<string>(DicomTag.PatientName);
            patientName1.Should().NotBe("Bar^Foo");
            patientName2.Should().NotBe("Bar^Foo");
            patientName1.Should().Be(patientName2);
        }
        
        [Fact]
        public void ShouldAnonymizeSameStudy()
        {
            // Arrange
            var dicomDataSet1 = new DicomDataset
            {
                { DicomTag.PatientName, "Bar^Foo" },
                { DicomTag.StudyInstanceUID, "1" },
                { DicomTag.SeriesInstanceUID, "1.1" },
                { DicomTag.SOPInstanceUID, "1.1.1" },
            };
            var metaInfo1 = new DicomFileMetaInformation();
            var dicomDataSet2 = new DicomDataset
            {
                { DicomTag.PatientName, "Bar^Foo" },
                { DicomTag.StudyInstanceUID, "1" },
                { DicomTag.SeriesInstanceUID, "1.2" },
                { DicomTag.SOPInstanceUID, "1.2.2" },
            };
            var metaInfo2 = new DicomFileMetaInformation();
            
            // Act
            _anonymizer.AnonymizeAsync(metaInfo1, dicomDataSet1);
            _anonymizer.AnonymizeAsync(metaInfo2, dicomDataSet2);
            
            // Assert
            var studyInstanceUID1 = dicomDataSet1.GetSingleValue<string>(DicomTag.StudyInstanceUID);
            var studyInstanceUID2 = dicomDataSet2.GetSingleValue<string>(DicomTag.StudyInstanceUID);
            studyInstanceUID1.Should().NotBe("1");
            studyInstanceUID2.Should().NotBe("1");
            studyInstanceUID1.Should().Be(studyInstanceUID2);
        }
        
        [Fact]
        public void ShouldAnonymizeSameSeries()
        {
            // Arrange
            var dicomDataSet1 = new DicomDataset
            {
                { DicomTag.PatientName, "Bar^Foo" },
                { DicomTag.StudyInstanceUID, "1" },
                { DicomTag.SeriesInstanceUID, "1.1" },
                { DicomTag.SOPInstanceUID, "1.1.1" },
            };
            var metaInfo1 = new DicomFileMetaInformation();
            var dicomDataSet2 = new DicomDataset
            {
                { DicomTag.PatientName, "Bar^Foo" },
                { DicomTag.StudyInstanceUID, "1" },
                { DicomTag.SeriesInstanceUID, "1.1" },
                { DicomTag.SOPInstanceUID, "1.1.2" },
            };
            var metaInfo2 = new DicomFileMetaInformation();
            
            // Act
            _anonymizer.AnonymizeAsync(metaInfo1, dicomDataSet1);
            _anonymizer.AnonymizeAsync(metaInfo2, dicomDataSet2);
            
            // Assert
            var seriesInstanceUID1 = dicomDataSet1.GetSingleValue<string>(DicomTag.SeriesInstanceUID);
            var seriesInstanceUID2 = dicomDataSet2.GetSingleValue<string>(DicomTag.SeriesInstanceUID);
            seriesInstanceUID1.Should().NotBe("1");
            seriesInstanceUID2.Should().NotBe("1");
            seriesInstanceUID1.Should().Be(seriesInstanceUID2);
        }
    }
}