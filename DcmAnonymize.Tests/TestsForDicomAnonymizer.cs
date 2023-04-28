using System.Linq;
using System.Threading.Tasks;
using DcmAnonymize.Instance;
using DcmAnonymize.Names;
using DcmAnonymize.Patient;
using DcmAnonymize.Series;
using DcmAnonymize.Study;
using FellowOakDicom;
using FluentAssertions;
using Xunit;

namespace DcmAnonymize.Tests;

public class TestsForDicomAnonymizer
{
    private readonly DicomAnonymizer _anonymizer;

    public TestsForDicomAnonymizer()
    {
        var randomNameGenerator = new RandomNameGenerator();
        var nationalNumberGenerator = new NationalNumberGenerator();
        _anonymizer = new DicomAnonymizer(
            new PatientAnonymizer(randomNameGenerator, nationalNumberGenerator), 
            new StudyAnonymizer(randomNameGenerator),
            new SeriesAnonymizer(), 
            new InstanceAnonymizer());
    }
        
    [Fact]
    public async Task ShouldBeAbleToAnonymizeEmptyDataSet()
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
        dicomDataSet.Validate();
            
        // Act
        await _anonymizer.AnonymizeAsync(metaInfo, dicomDataSet);
            
        // Assert
        dicomDataSet.Contains(DicomTag.PatientName).Should().BeTrue();
        dicomDataSet.GetSingleValue<string>(DicomTag.PatientName).Should().NotBe("Bar^Foo");
        dicomDataSet.GetSingleValue<string>(DicomTag.StudyInstanceUID).Should().NotBe("1");
        dicomDataSet.GetSingleValue<string>(DicomTag.SeriesInstanceUID).Should().NotBe("1.1");
        dicomDataSet.GetSingleValue<string>(DicomTag.SOPInstanceUID).Should().NotBe("1.1.1");
    }
        
    [Fact]
    public async Task ShouldAnonymizeSamePatient()
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
        await _anonymizer.AnonymizeAsync(metaInfo1, dicomDataSet1);
        await _anonymizer.AnonymizeAsync(metaInfo2, dicomDataSet2);
            
        // Assert
        var patientName1 = dicomDataSet1.GetSingleValue<string>(DicomTag.PatientName);
        var patientName2 = dicomDataSet2.GetSingleValue<string>(DicomTag.PatientName);
        patientName1.Should().NotBe("Bar^Foo");
        patientName2.Should().NotBe("Bar^Foo");
        patientName1.Should().Be(patientName2);
    }
        
    [Fact]
    public async Task ShouldAnonymizeSameStudy()
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
        await _anonymizer.AnonymizeAsync(metaInfo1, dicomDataSet1);
        await _anonymizer.AnonymizeAsync(metaInfo2, dicomDataSet2);
            
        // Assert
        var studyInstanceUID1 = dicomDataSet1.GetSingleValue<string>(DicomTag.StudyInstanceUID);
        var studyInstanceUID2 = dicomDataSet2.GetSingleValue<string>(DicomTag.StudyInstanceUID);
        studyInstanceUID1.Should().NotBe("1");
        studyInstanceUID2.Should().NotBe("1");
        studyInstanceUID1.Should().Be(studyInstanceUID2);
    }
        
    [Fact]
    public async Task ShouldAnonymizeSameSeries()
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
        await _anonymizer.AnonymizeAsync(metaInfo1, dicomDataSet1);
        await _anonymizer.AnonymizeAsync(metaInfo2, dicomDataSet2);
            
        // Assert
        var seriesInstanceUID1 = dicomDataSet1.GetSingleValue<string>(DicomTag.SeriesInstanceUID);
        var seriesInstanceUID2 = dicomDataSet2.GetSingleValue<string>(DicomTag.SeriesInstanceUID);
        seriesInstanceUID1.Should().NotBe("1");
        seriesInstanceUID2.Should().NotBe("1");
        seriesInstanceUID1.Should().Be(seriesInstanceUID2);
    }
        
    [Fact]
    public async Task ShouldAnonymizeSameInstances()
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
            { DicomTag.SOPInstanceUID, "1.1.1" },
        };
        var metaInfo2 = new DicomFileMetaInformation();
            
        // Act
        await _anonymizer.AnonymizeAsync(metaInfo1, dicomDataSet1);
        await _anonymizer.AnonymizeAsync(metaInfo2, dicomDataSet2);
            
        // Assert
        var sopInstanceUID1 = dicomDataSet1.GetSingleValue<string>(DicomTag.SOPInstanceUID);
        var sopInstanceUID2 = dicomDataSet2.GetSingleValue<string>(DicomTag.SOPInstanceUID);
        sopInstanceUID1.Should().NotBe("1.1.1");
        sopInstanceUID2.Should().NotBe("1.1.1");
        sopInstanceUID1.Should().Be(sopInstanceUID2);
    }
        
    [Fact]
    public async Task ShouldBeAbleToAnonymizeSampleDicomFile()
    {
        // Arrange
        var sampleDicomFile = await DicomFile.OpenAsync("./TestData/SampleDicomFile.dcm");
        var metaInfo = sampleDicomFile.FileMetaInfo;
        var dicomDataSet = sampleDicomFile.Dataset;
            
        dicomDataSet.Validate();

        // Act
        await _anonymizer.AnonymizeAsync(metaInfo, dicomDataSet);

        // Assert
        dicomDataSet.Validate();
    }

    [Fact]
    public async Task ShouldRetainReferences()
    {
        // Arrange
        const string file1 = "./TestData/Im1-removedByRJ1-113037.dcm";
        const string file2 = "./TestData/RJ1-113037.dcm";
        var dicomFile1 = await DicomFile.OpenAsync(file1);
        var dicomFile2 = await DicomFile.OpenAsync(file2);
        var sopInstanceUID = dicomFile1.Dataset.GetSingleValue<string>(DicomTag.SOPInstanceUID);
        var currentRequestedProcedureEvidenceSequence = dicomFile2.Dataset.GetSequence(DicomTag.CurrentRequestedProcedureEvidenceSequence);
        var referencedSeriesSequence = currentRequestedProcedureEvidenceSequence.Single().GetSequence(DicomTag.ReferencedSeriesSequence);
        var referencedSopSequence = referencedSeriesSequence.Single().GetSequence(DicomTag.ReferencedSOPSequence);
        var firstReferencedSopInstanceUID = referencedSopSequence.First().GetSingleValue<string>(DicomTag.ReferencedSOPInstanceUID);
        
        // Ensure our test data is correct
        sopInstanceUID.Should().NotBeNullOrEmpty();
        firstReferencedSopInstanceUID.Should().NotBeNullOrEmpty();
        firstReferencedSopInstanceUID.Should().Be(sopInstanceUID); 
        
        // Act
        await _anonymizer.AnonymizeAsync(dicomFile1.FileMetaInfo, dicomFile1.Dataset);
        await _anonymizer.AnonymizeAsync(dicomFile2.FileMetaInfo, dicomFile2.Dataset);

        // Assert
        var sopInstanceUID2 = dicomFile1.Dataset.GetSingleValue<string>(DicomTag.SOPInstanceUID);
        var currentRequestedProcedureEvidenceSequence2 = dicomFile2.Dataset.GetSequence(DicomTag.CurrentRequestedProcedureEvidenceSequence);
        var referencedSeriesSequence2 = currentRequestedProcedureEvidenceSequence2.Single().GetSequence(DicomTag.ReferencedSeriesSequence);
        var referencedSopSequence2 = referencedSeriesSequence2.Single().GetSequence(DicomTag.ReferencedSOPSequence);
        var firstReferencedSopInstanceUID2 = referencedSopSequence2.First().GetSingleValue<string>(DicomTag.ReferencedSOPInstanceUID);

        // Ensure data is still present
        sopInstanceUID2.Should().NotBeNullOrEmpty();
        firstReferencedSopInstanceUID2.Should().NotBeNullOrEmpty();
        
        // Ensure data is anonymized
        sopInstanceUID.Should().NotBe(sopInstanceUID2);
        firstReferencedSopInstanceUID.Should().NotBe(firstReferencedSopInstanceUID2);
        
        // Ensure referential integrity is retained
        sopInstanceUID2.Should().Be(firstReferencedSopInstanceUID2);
    }
}
