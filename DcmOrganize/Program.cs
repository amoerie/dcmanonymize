using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CommandLine;
using Dicom;

namespace DcmOrganize
{
    public static class Program
    {
        // ReSharper disable UnusedAutoPropertyAccessor.Global
        // ReSharper disable MemberCanBePrivate.Global
        // ReSharper disable ClassNeverInstantiated.Global
        public class Options
        {
            [Value(0, HelpText = "Organize these DICOM files. When missing, this option will be read from the piped input.", Required = false)]
            public IEnumerable<string>? Files { get; set; }
            
            [Option('t', "targetDirectory", Default = ".", HelpText = "Organize DICOM files in this directory")]
            public string? TargetDirectory { get; set; }
            
            [Option('p', "targetFilePattern", Default = "{PatientName}/{AccessionNumber}/{SeriesNumber}/{InstanceNumber ?? SOPInstanceUID}.dcm", HelpText = "Organize DICOM files in this directory")]
            public string? TargetFilePattern { get; set; }
        }
        // ReSharper restore UnusedAutoPropertyAccessor.Global
        // ReSharper restore MemberCanBePrivate.Global
        // ReSharper restore ClassNeverInstantiated.Global
        
        private static readonly object CreateDirectoryLock = new object();
        
        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(Organize)
                .WithNotParsed(Fail);
        }
        
        private static void Fail(IEnumerable<Error> errors)
        {
            Console.Error.WriteLine("Invalid arguments provided");
            foreach (var error in errors.Where(e => e.Tag != ErrorType.HelpRequestedError))
            {
                Console.Error.WriteLine(error.ToString());
            }
        }

        private static void Organize(Options options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));

            IEnumerable<string> ReadFilesFromConsole()
            {
                string? file;
                while ((file = Console.ReadLine()) != null)
                {
                    yield return file;
                }            
            }

            var files = options.Files != null && options.Files.Any() ? options.Files : ReadFilesFromConsole();
            var targetDirectory = new DirectoryInfo(options.TargetDirectory!);
            var targetFilePattern = options.TargetFilePattern!;

            if (!targetDirectory.Exists)
            {
                Console.Error.WriteLine($"Target directory does not exist: {targetDirectory.FullName}");
                return;
            }

            foreach (var file in files.Select(f => new FileInfo(f)))
            {
                DicomFile dicomFile;
                try
                {
                    using (var fileStream = new FileStream(file.FullName, FileMode.Open, FileAccess.Read, FileShare.Read, 4096))
                    {
                        dicomFile = DicomFile.Open(fileStream, FileReadOption.SkipLargeTags);
                    }
                }
                catch
                {
                    Console.Error.WriteLine("Not a DICOM file: " + file.FullName);
                    continue;
                }

                if (!DicomFilePatternApplier.TryApply(dicomFile.Dataset, targetFilePattern, out var fileName))
                {
                    Console.Error.WriteLine("Failed to apply DICOM file pattern");
                    continue;
                }

                var targetFile = new FileInfo(Path.Join(targetDirectory.FullName, fileName));

                if (!targetFile.Directory!.Exists)
                {
                    try
                    {
                        targetFile.Directory!.Create();
                    }
                    catch (IOException exception)
                    {
                        Console.Error.WriteLine($"Failed to create directory {targetFile.Directory.FullName}");
                        Console.Error.WriteLine(exception);
                        return;
                    }
                }

                if (file.FullName == targetFile.FullName)
                {
                    Console.WriteLine($"OK:    {file.FullName} === {targetFile.FullName}");
                    continue;
                }

                try
                {
                    Console.WriteLine($"Moving {file.FullName} --> {targetFile.FullName}");
                    File.Move(file.FullName, targetFile.FullName);
                }
                catch (IOException exception)
                {
                    Console.Error.WriteLine("Failed to move file");
                    Console.Error.WriteLine(exception);
                    continue;
                }
            }
        }
    }
}