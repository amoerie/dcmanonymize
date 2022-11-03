using System;

namespace DcmAnonymize.Patient;

public class NationalNumberGenerator
{
    private readonly Random _random;

    public NationalNumberGenerator()
    {
        _random = new Random();
    }
        
    public string GenerateRandomNationalNumber(DateTime birthDate, PatientSex? sex)
    {
        var year = birthDate.Year.ToString("0000").Substring(2, 2);
        var month = birthDate.Month;
        var day = birthDate.Day;
        int index;
        switch (sex)
        {
            case PatientSex.Male:
                // Male have an odd index
                index = _random.Next(0, 499) * 2 + 1;
                break;
            case PatientSex.Female:
                // Females have an even index
                index = _random.Next(0, 499) * 2;
                break;
            default:
                index = _random.Next(0, 999);
                break;
        }

        var number = $"{year}{month:00}{day:00}{index:000}";
        var moduloBase = number;
        if (birthDate.Year >= 2000)
        {
            moduloBase = "2" + moduloBase;
        }
        var checkSum = (97 - long.Parse(moduloBase)%97).ToString("00");

        return $"{number}{checkSum}";
    }

}