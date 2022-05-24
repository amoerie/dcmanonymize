using System;
using System.Threading.Tasks;
using DcmAnonymize.Patient;
using FluentAssertions;
using Xunit;

namespace DcmAnonymize.Tests.Patient
{
    public class TestsForNationalNumberGenerator
    {
        private readonly NationalNumberGenerator _nationalNumberGenerator;

        public TestsForNationalNumberGenerator()
        {
            _nationalNumberGenerator = new NationalNumberGenerator();
        }
        
        [Fact]
        public void ShouldGenerateCorrectMaleNationalNumber()
        {
            // Arrange
            var birthDate = new DateTime(1994, 12, 5);
            var sex = PatientSex.Male;
            
            // Act
            var nationalNumber = _nationalNumberGenerator.GenerateRandomNationalNumber(birthDate, sex);
            
            // Assert
            var year = int.Parse(nationalNumber.Substring(0, 2));
            var month = int.Parse(nationalNumber.Substring(2, 2));
            var day = int.Parse(nationalNumber.Substring(4, 2));
            var index = int.Parse(nationalNumber.Substring(6, 3));
            var modulo = int.Parse(nationalNumber.Substring(9, 2));
            var combined = long.Parse(nationalNumber.Substring(0, 9));

            year.Should().Be(94);
            month.Should().Be(12);
            day.Should().Be(5);
            (index % 2).Should().Be(1); // Male should produce an odd index
            modulo.Should().Be((int) (97 - combined % 97));
        }
        
        [Fact]
        public void ShouldGenerateCorrectFemaleNationalNumber()
        {
            // Arrange
            var birthDate = new DateTime(1994, 12, 5);
            var sex = PatientSex.Female;
            
            // Act
            var nationalNumber = _nationalNumberGenerator.GenerateRandomNationalNumber(birthDate, sex);
            
            // Assert
            var year = int.Parse(nationalNumber.Substring(0, 2));
            var month = int.Parse(nationalNumber.Substring(2, 2));
            var day = int.Parse(nationalNumber.Substring(4, 2));
            var index = int.Parse(nationalNumber.Substring(6, 3));
            var modulo = int.Parse(nationalNumber.Substring(9, 2));
            var combined = long.Parse(nationalNumber.Substring(0, 9));

            year.Should().Be(94);
            month.Should().Be(12);
            day.Should().Be(5);
            (index % 2).Should().Be(0); // Female should produce an even index
            modulo.Should().Be((int) (97 - combined % 97));
        }
        
        [Fact]
        public void ShouldGenerateCorrectNationalNumberAfter2000()
        {
            // Arrange
            var birthDate = new DateTime(2001, 12, 5);
            var sex = PatientSex.Male;
            
            // Act
            var nationalNumber = _nationalNumberGenerator.GenerateRandomNationalNumber(birthDate, sex);
            
            // Assert
            var year = int.Parse(nationalNumber.Substring(0, 2));
            var month = int.Parse(nationalNumber.Substring(2, 2));
            var day = int.Parse(nationalNumber.Substring(4, 2));
            var index = int.Parse(nationalNumber.Substring(6, 3));
            var modulo = int.Parse(nationalNumber.Substring(9, 2));
            var combined = long.Parse("2" + nationalNumber.Substring(0, 9));

            year.Should().Be(1);
            month.Should().Be(12);
            day.Should().Be(5);
            (index % 2).Should().Be(1); // Male should produce an odd index
            modulo.Should().Be((int) (97 - combined % 97));
        }
    }
}
