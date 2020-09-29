using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CommandLine;
using DcmAnonymize.Names;
using Dicom;

namespace DcmAnonymize
{
    public static class Program
    {
        // ReSharper disable UnusedAutoPropertyAccessor.Global
        // ReSharper disable MemberCanBePrivate.Global
        // ReSharper disable ClassNeverInstantiated.Global
        public class Options
        {
            [Value(0, HelpText = "Anonymize these DICOM files. When missing, this option will be read from the piped input.", Required = false)]
            public IEnumerable<string>? Files { get; set; }
        }
        // ReSharper restore UnusedAutoPropertyAccessor.Global
        // ReSharper restore MemberCanBePrivate.Global
        // ReSharper restore ClassNeverInstantiated.Global
        
        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(Anonymize)
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

        private static void Anonymize(Options options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));

            IEnumerable<string> ReadFilesFromConsole()
            {
                string? file;
                while ((file = Console.ReadLine()) != null)
                {
                    if(file != null && File.Exists(file))
                        yield return file;
                }            
            }

            var files = options.Files != null && options.Files.Any() ? options.Files : ReadFilesFromConsole();

            var randomNameGenerator = new RandomNameGenerator();
            var anonymizer = new DicomAnonymizer(
                new PatientAnonymizer(randomNameGenerator),
                new StudyAnonymizer(randomNameGenerator),
                new SeriesAnonymizer(),
                new InstanceAnonymizer()
            );
            
            foreach (var file in files.Select(f => new FileInfo(f)))
            {
                DicomFile dicomFile;
                using (var inputFileStream = new FileStream(file.FullName, FileMode.Open, FileAccess.Read, FileShare.Read, 4096))
                {
                    try
                    {
                        dicomFile = DicomFile.Open(inputFileStream, FileReadOption.ReadAll);
                    }
                    catch
                    {
                        Console.Error.WriteLine("Not a DICOM file: " + file.FullName);
                        continue;
                    }
                }

                try
                {
                    anonymizer.Anonymize(dicomFile.Dataset);
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine("Failed to generate anonymous data for the provided DICOM file");
                    Console.Error.WriteLine(e.ToString());
                    continue;
                }

                try
                {
                    dicomFile.Save(file.FullName);
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine("Failed to overwrite the original DICOM file");
                    Console.Error.WriteLine(e.ToString());
                    continue;
                }

                Console.WriteLine(file.FullName);
            }
        }
    }
}