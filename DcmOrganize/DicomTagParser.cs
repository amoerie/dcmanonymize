using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Dicom;

namespace DcmOrganize
{
    public static class DicomTagParser
    {
        private static readonly Lazy<IEnumerable<FieldInfo>> DicomTagFields = new Lazy<IEnumerable<FieldInfo>>(
            () => typeof(DicomTag)
                .GetFields(BindingFlags.Static | BindingFlags.Public)
                .Where(f => f.FieldType == typeof(DicomTag))
                .ToList()
        );

        public static bool TryParse(string dicomTagAsString, out DicomTag? dicomTag)
        {
            try
            {
                if (dicomTagAsString[0] == '(' || char.IsDigit(dicomTagAsString[0]))
                {
                    dicomTag = DicomTag.Parse(dicomTagAsString);
                    return true;
                }

                var field = DicomTagFields.Value
                    .FirstOrDefault(f => string.Equals(f.Name, dicomTagAsString));
                if (field != null)
                {
                    dicomTag = (DicomTag?) field.GetValue(null);
                    return true;
                }

                dicomTag = DicomTag.Parse(dicomTagAsString);
                return true;
            }
            catch (DicomDataException e)
            {
                Console.Error.WriteLine($"Invalid DICOM tag '{dicomTagAsString}': " + e.Message);
                Console.Error.WriteLine(e);
                dicomTag = null;
                return false;
            }
        }
    }
}