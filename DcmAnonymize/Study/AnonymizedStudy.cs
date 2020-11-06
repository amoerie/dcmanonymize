using System;
using System.Diagnostics;
using DcmAnonymize.Names;
using Dicom;

namespace DcmAnonymize
{
    public class AnonymizedStudy
    {
        public string StudyInstanceUID { get; set; }
        public string Description { get; set; }
        public string AccessionNumber { get; set; }
        public DateTime StudyDateTime { get; set; }
        public RandomName StudyRequestingPhysician { get; set; }

        public RandomName StudyPerformingPhysician { get; set; }
        public string StudyID { get; set; }

        public string InstitutionName { get; set; }
    }
}