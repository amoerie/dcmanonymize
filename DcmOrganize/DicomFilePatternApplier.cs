using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Dicom;

namespace DcmOrganize
{
    public static class DicomFilePatternApplier
    {
        public static bool TryApply(DicomDataset dicomDataset, string filePattern, out string? file)
        {
            file = filePattern.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
            var openCurlyBraceIndex = file.IndexOf('{');
            var closingCurlyBraceIndex = file.IndexOf('}');
            var directorySeparatorIndex = file.LastIndexOf(Path.DirectorySeparatorChar);
            
            while (openCurlyBraceIndex != -1 && closingCurlyBraceIndex != -1)
            {
                var dicomTagExpression = file.Substring(openCurlyBraceIndex, closingCurlyBraceIndex - openCurlyBraceIndex).Trim('{', '}');
                var dicomTagsToTry = new Stack<string>(
                    dicomTagExpression
                        .Split("??", StringSplitOptions.RemoveEmptyEntries)
                        .Select(d => d.Trim())
                        .Reverse()
                );

                string? dicomStringValue = null;

                while (dicomTagsToTry.TryPop(out var nextDicomTagToTry ) && dicomStringValue == null)
                {
                    if (!DicomTagParser.TryParse(nextDicomTagToTry, out var dicomTag))
                    {
                        Console.Error.WriteLine($"ERROR: DICOM tag '{nextDicomTagToTry}' could not be parsed");
                        file = null;
                        return false;
                    }

                    dicomStringValue = dicomDataset.GetValueOrDefault(dicomTag, 0, (string?) null)?.Replace('^', ' ');
                }

                if (dicomStringValue == null)
                {
                    Console.Error.WriteLine($"ERROR: DICOM tag expression '{dicomTagExpression}' is not present in DICOM dataset");
                    file = null;
                    return false;
                }

                if (directorySeparatorIndex >= closingCurlyBraceIndex)
                    dicomStringValue = FolderNameCleaner.Clean(dicomStringValue);

                file = file.Substring(0, openCurlyBraceIndex)
                       + dicomStringValue
                       + file.Substring(Math.Min(file.Length - 1, closingCurlyBraceIndex + 1));
                
                openCurlyBraceIndex = file.IndexOf('{');
                closingCurlyBraceIndex = file.IndexOf('}');
                directorySeparatorIndex = file.LastIndexOf(Path.DirectorySeparatorChar);
            }

            file = file.Trim(Path.DirectorySeparatorChar);

            return true;
        }        
    }
}