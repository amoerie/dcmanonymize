using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CommandLine;
using DcmAnonymize.Blanking;
using DcmAnonymize.Imaging;
using DcmAnonymize.Instance;
using DcmAnonymize.Names;
using DcmAnonymize.Order;
using DcmAnonymize.Patient;
using DcmAnonymize.Recursive;
using DcmAnonymize.Series;
using DcmAnonymize.Study;
using FellowOakDicom;
using FellowOakDicom.Imaging.NativeCodec;

namespace DcmAnonymize;

public class Program
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
        
        [Option("blank-rectangle", HelpText = "One or more rectangular regions to blank in the pixel data. Provide values in the shape (x1,y1)->(x2,y2), e.g. (0,0)->(10,10)", Required = false)]
        public IEnumerable<string>? RectanglesToBlank { get; set; }
    }
    
    // ReSharper restore UnusedAutoPropertyAccessor.Global
    // ReSharper restore MemberCanBePrivate.Global
    // ReSharper restore ClassNeverInstantiated.Global

    public TextReader Input { get; set; } = Console.In;
    public TextWriter Output { get; set; } = Console.Out;
    public TextWriter ErrorOutput { get; set; } = Console.Error;

    public static Task<int> Main(string[] args)
    {
        var program = new Program();

        return program.Run(args);
    }
    
    public async Task<int> Run(string[] args)
    {
        // Configure Fellow Oak DICOM
        new DicomSetupBuilder()
            .RegisterServices(s =>
            {
                s.AddImageManager<ImageSharpImageManager>();
                s.AddTranscoderManager<NativeTranscoderManager>();
            })
            .Build();

        var parser = new Parser(settings =>
        {
            settings.HelpWriter = ErrorOutput;
        });
        
        switch (parser.ParseArguments<Options>(args))
        {
            case Parsed<Options> parsed:
                await AnonymizeAsync(parsed.Value).ConfigureAwait(false);
                return 0;
            case NotParsed<Options> notParsed:
                Fail(notParsed.Errors);
                return -1;
            default:
                throw new InvalidOperationException("Invalid parser result");
        }
    }

    private void Fail(IEnumerable<Error> errors)
    {
        ErrorOutput.WriteLine("Invalid arguments provided");
        foreach (var error in errors.Where(e => e.Tag != ErrorType.HelpRequestedError))
        {
            ErrorOutput.WriteLine(error.ToString());
        }
    }

    private async Task AnonymizeAsync(Options options)
    {
        if (options == null) throw new ArgumentNullException(nameof(options));

        IEnumerable<FileInfo> ReadFilesFromConsole()
        {
            while (Input.ReadLine() is { } file)
            {
                if (File.Exists(file))
                    yield return new FileInfo(file);
            }
        }
        
        var files = options.Files != null && options.Files.Any()
            ? options.Files.Select(f => new FileInfo(f))
            : ReadFilesFromConsole();
        var parallelism = options.Parallelism;
        var randomNameGenerator = new RandomNameGenerator();
        var nationalNumberGenerator = new NationalNumberGenerator();
        var dummyValueFiller = new DicomTagCleaner(randomNameGenerator);
        var anonymizer = new DicomAnonymizer(
            new PatientAnonymizer(randomNameGenerator, nationalNumberGenerator),
            new StudyAnonymizer(randomNameGenerator),
            new SeriesAnonymizer(),
            new InstanceAnonymizer(),
            new RecursiveAnonymizer(dummyValueFiller),
            new OrderAnonymizer(),
            new BlankingAnonymizer()
        );
        var anonymizationOptions = AnonymizationOptions.Parse(options.RectanglesToBlank);

        await Task.WhenAll(
            Partitioner
                .Create(files)
                .GetPartitions(parallelism)
                .AsParallel()
                .Select(partition => AnonymizeFilesAsync(partition, anonymizer, anonymizationOptions))
        );
    }

    private async Task AnonymizeFilesAsync(IEnumerator<FileInfo> files, DicomAnonymizer anonymizer, AnonymizationOptions options)
    {
        using (files)
        {
            while (files.MoveNext())
            {
                await AnonymizeFileAsync(files.Current, anonymizer, options).ConfigureAwait(false);
            }
        }
    }

    private async Task AnonymizeFileAsync(FileInfo file, DicomAnonymizer anonymizer, AnonymizationOptions options)
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
                await ErrorOutput.WriteLineAsync("Not a DICOM file: " + file.FullName);
                return;
            }
        }

        if (file.Name == "DICOMDIR" || dicomFile.FileMetaInfo.MediaStorageSOPClassUID == DicomUID.MediaStorageDirectoryStorage)
        {
            File.Delete(file.FullName);
            return;
        }

        try
        {
            await anonymizer.AnonymizeAsync(dicomFile, options);
        }
        catch (Exception e)
        {
            await ErrorOutput.WriteLineAsync($"Failed to generate anonymous data for the provided DICOM file: {file.FullName}\n{e}");
            return;
        }

        try
        {
            await dicomFile.SaveAsync(file.FullName);
        }
        catch (Exception e)
        {
            await ErrorOutput.WriteLineAsync($"Failed to overwrite the original DICOM file: {file.FullName}\n{e}");
            return;
        }

        await Output.WriteLineAsync(file.FullName);
    }
}
