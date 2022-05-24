using System;
using DcmAnonymize.Names;

namespace DcmAnonymize.Patient
{
    public record AnonymizedPatient(
        RandomName Name,
        string NationalNumber,
        DateTime BirthDate,
        string PatientId,
        PatientSex? Sex);

    public enum PatientSex
    {
        Male,
        Female,
        Other
    }
}
