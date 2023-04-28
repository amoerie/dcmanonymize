using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CommandLine;
using DcmAnonymize.Instance;
using DcmAnonymize.Names;
using DcmAnonymize.Patient;
using DcmAnonymize.Series;
using DcmAnonymize.Study;
using FellowOakDicom;

namespace DcmAnonymize;

public static class Program
{
    // ReSharper disable UnusedAutoPropertyAccessor.Global
    // ReSharper disable MemberCanBePrivate.Global
    // ReSharper disable ClassNeverInstantiated.Global
    public class Options
    {
        [Value(0, HelpText = "Anonymize these DICOM files. When missing, this option will be read from the piped input.", Required = false)]
        public IEnumerable<string>? Files { get; set; }

        [Option('p', "parallelism", Default = 8, HelpText = "Process this many files in parallel")]
        public int Parallelism { get; set; }
            

    }
    // ReSharper restore UnusedAutoPropertyAccessor.Global
    // ReSharper restore MemberCanBePrivate.Global
    // ReSharper restore ClassNeverInstantiated.Global

    static async Task Main(string[] args)
    {
        var parserResult = Parser.Default.ParseArguments<Options>(args);

        if (parserResult is Parsed<Options> parsed)
        {
            await AnonymizeAsync(parsed.Value).ConfigureAwait(false);
        }
        else if (parserResult is NotParsed<Options> notParsed)
        {
            Fail(notParsed.Errors);
        }
    }

    private static void Fail(IEnumerable<Error> errors)
    {
        Console.Error.WriteLine("Invalid arguments provided");
        foreach (var error in errors.Where(e => e.Tag != ErrorType.HelpRequestedError))
        {
            Console.Error.WriteLine(error.ToString());
        }
    }

    private static async Task AnonymizeAsync(Options options)
    {
        if (options == null) throw new ArgumentNullException(nameof(options));

        IEnumerable<FileInfo> ReadFilesFromConsole()
        {
            string? file;
            while ((file = Console.ReadLine()) != null)
            {
                if (file != null && File.Exists(file))
                    yield return new FileInfo(file);
            }
        }

        var files = options.Files != null && options.Files.Any()
            ? options.Files.Select(f => new FileInfo(f))
            : ReadFilesFromConsole();
        var parallelism = options.Parallelism;
        var randomNameGenerator = new RandomNameGenerator();
        var nationalNumberGenerator = new NationalNumberGenerator();
        var anonymizer = new DicomAnonymizer(
            new PatientAnonymizer(randomNameGenerator, nationalNumberGenerator),
            new StudyAnonymizer(randomNameGenerator),
            new SeriesAnonymizer(),
            new InstanceAnonymizer()
        );

        await Task.WhenAll(
            Partitioner
                .Create(files)
                .GetPartitions(parallelism)
                .AsParallel()
                .Select(partition => AnonymizeFilesAsync(partition, anonymizer))
        ).ConfigureAwait(false);
    }

    private static async Task AnonymizeFilesAsync(IEnumerator<FileInfo> files, DicomAnonymizer anonymizer)
    {
        using (files)
        {
            while (files.MoveNext())
            {
                await AnonymizeFileAsync(files.Current, anonymizer).ConfigureAwait(false);
            }
        }
    }

    private static async Task AnonymizeFileAsync(FileInfo file, DicomAnonymizer anonymizer)
    {
        DicomFile dicomFile;
        using (var inputFileStream = new FileStream(file.FullName, FileMode.Open, FileAccess.Read, FileShare.Read, 4096))
        {
            try
            {
                dicomFile = await DicomFile.OpenAsync(inputFileStream, FileReadOption.ReadAll);
            }
            catch
            {
                await Console.Error.WriteLineAsync("Not a DICOM file: " + file.FullName);
                return;
            }
        }

        if (file.Name == "DICOMDIR" || dicomFile.FileMetaInfo.MediaStorageSOPClassUID == DicomUID.MediaStorageDirectoryStorage)
        {
            // Do not anonymize DICOM directory files, just delete them.
            // TODO also anonymize this DICOMDIR
            File.Delete(file.FullName);
            return;
        }

        try
        {
            await anonymizer.AnonymizeAsync(dicomFile);
        }
        catch (Exception e)
        {
            await Console.Error.WriteLineAsync($"Failed to generate anonymous data for the provided DICOM file: {file.FullName}\n{e}");
            return;
        }

        try
        {
            await dicomFile.SaveAsync(file.FullName);
        }
        catch (Exception e)
        {
            await Console.Error.WriteLineAsync($"Failed to overwrite the original DICOM file: {file.FullName}\n{e}");
            return;
        }

        Console.WriteLine(file.FullName);
    }
}
