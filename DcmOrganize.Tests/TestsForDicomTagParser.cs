using System;
using Dicom;
using FluentAssertions;
using Xunit;

namespace DcmOrganize.Tests
{
    public class TestsForDicomTagParser
    {
        [Fact]
        public void ShouldParseTagByGroupAndElement()
        {
            DicomTagParser.TryParse("(0008,0050)", out var dicomTag).Should().BeTrue();
            dicomTag.Should().Be(DicomTag.AccessionNumber);
        }
        
        [Fact]
        public void ShouldParseTagByName()
        {
            DicomTagParser.TryParse("AccessionNumber", out var dicomTag).Should().BeTrue();
            dicomTag.Should().Be(DicomTag.AccessionNumber);
        }
        
        [Fact]
        public void ShouldNotParseUnknownTag()
        {
            DicomTagParser.TryParse("Banana", out _).Should().BeFalse();
        }
    }
}