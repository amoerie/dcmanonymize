using System;
using System.IO;
using System.Threading.Tasks;
using Dicom;

namespace DcmOrganize
{
    class Program
    {
        private static readonly object CreateDirectoryLock = new object();
        
        static void Main(string[] args)
        {
            var files = Directory.EnumerateFiles("./Patients", "*.*", SearchOption.AllDirectories);

            Console.WriteLine("Organizing files");
                
            Parallel.ForEach(files, new ParallelOptions
            {
                MaxDegreeOfParallelism = 25
            }, OrganizeFile);
            
            
        }

        private static void OrganizeFile(string file)
        {
            var sourceFile = new FileInfo(file);
            DicomFile dicomFile;
            try
            {
                using (var fileStream = new FileStream(sourceFile.FullName, FileMode.Open, FileAccess.Read, FileShare.Read, 4096))
                {
                    dicomFile = DicomFile.Open(fileStream, FileReadOption.SkipLargeTags);
                }
            }
            catch
            {
                Console.WriteLine("Not a DICOM file: " + file);
                return;
            }

            string patientName;
            string accessionNumber;
            string seriesNumber;

            try
            {
                patientName = dicomFile.Dataset.GetString(DicomTag.PatientName).Replace("^", ", ");
                accessionNumber = dicomFile.Dataset.GetString(DicomTag.AccessionNumber);
                seriesNumber = dicomFile.Dataset.GetString(DicomTag.SeriesNumber);
            }
            catch
            {
                Console.WriteLine("Missing DICOM tags");
                return;
            }

            var targetDirectory = new DirectoryInfo($"./Patients/{patientName}/{accessionNumber}/{seriesNumber}");

            if (!targetDirectory.Exists)
            {
                lock (CreateDirectoryLock)
                {
                    if (!targetDirectory.Exists)
                    {
                        targetDirectory.Create();
                    }
                }
            }
            
            var targetFile = new FileInfo(Path.Combine(targetDirectory.FullName, sourceFile.Name));

            if (sourceFile.FullName == targetFile.FullName)
            {
                Console.WriteLine($"OK:    {sourceFile.FullName} === {targetFile.FullName}");
                return;
            }
            
            try
            {
                Console.WriteLine($"Moving {sourceFile.FullName} --> {targetFile.FullName}");
                File.Move(sourceFile.FullName, targetFile.FullName);
            }
            catch(Exception e)
            {
                Console.Error.WriteLine("Failed to move file: " + e);
            }
        }
    }
}