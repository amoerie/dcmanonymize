using System;
using DcmAnonymize.Names;
using Dicom;

namespace DcmAnonymize
{
    public class AnonymizedPatient
    {
        public RandomName Name { get; set; }
        public string NationalNumber { get; set; }
        public DateTime BirthDate { get; set; }
        public string PatientId { get; set; }

        public PatientSex? Sex { get; set; }
    }

    public enum PatientSex
    {
        Male,
        Female,
        Other
    }
}
