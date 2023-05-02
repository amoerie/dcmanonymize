using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace DcmAnonymize.Tests;

[Collection("DcmAnonymize")]
public class TestsForDcmAnonymize : IDisposable
{
    private readonly TextWriter _originalOut;
    private readonly TextWriter _originalError;
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly StringBuilder _output;
    private readonly StringBuilder _errorOutput;
    private readonly StringWriter _outputWriter;
    private readonly StringWriter _errorOutputWriter;
    private readonly TextReader _inputReader;
    private readonly TextReader _originalIn;
    private readonly FileInfo _dicomFile;

    public TestsForDcmAnonymize(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper ?? throw new ArgumentNullException(nameof(testOutputHelper));
        
        var testDataDirectory = new DirectoryInfo("./TestData");
        var sampleDicomFile = new FileInfo(Path.Join(testDataDirectory.Name, "SampleDicomFile.dcm"));
        var sampleDicomFileCopy = new FileInfo(Path.Join(testDataDirectory.Name, $"SampleDicomFile_{Guid.NewGuid()}.dcm"));
        File.Copy(sampleDicomFile.FullName, sampleDicomFileCopy.FullName);
        _dicomFile = sampleDicomFileCopy;
        
        _output = new StringBuilder();
        _outputWriter = new StringWriter(_output);
        _errorOutput = new StringBuilder();
        _errorOutputWriter = new StringWriter(_errorOutput);
        _originalIn = Console.In;
        _originalOut = Console.Out;
        _originalError = Console.Error;
        _inputReader = new StringReader(_dicomFile.FullName + Environment.NewLine);
        Console.SetIn(_inputReader);
        Console.SetOut(_outputWriter);
        Console.SetError(_errorOutputWriter);
    }

    public void Dispose()
    {
        _testOutputHelper.WriteLine(_output.ToString());
        _outputWriter.Dispose();
        _errorOutputWriter.Dispose();
        _inputReader.Dispose();
        Console.SetIn(_originalIn);
        Console.SetOut(_originalOut);
        Console.SetError(_originalError);

        var remainingAttempts = 3;
        while (remainingAttempts > 0 && File.Exists(_dicomFile.FullName))
        {
            try
            {
                File.Delete(_dicomFile.FullName);
            }
            catch
            {
                remainingAttempts--;
                Thread.Sleep(1000);
            }
        }
    }

    [Fact]
    public async Task ShouldAnonymizeTestFile()
    {
        // Arrange
        var expected = $"{_dicomFile.FullName}{Environment.NewLine}";

        // Act
        var statusCode = await Program.Main(new []
        {
            _dicomFile.FullName
        });

        // Assert
        Assert.Equal(expected, _output.ToString());
        Assert.Equal(string.Empty, _errorOutput.ToString());
        Assert.Equal(0, statusCode);
    }

    [Fact]
    public async Task ShouldAnonymizeTestFileFromConsole()
    {
        // Arrange
        var expected = $"{_dicomFile.FullName}{Environment.NewLine}";

        // Act
        var statusCode = await Program.Main(Array.Empty<string>());

        // Assert
        Assert.Equal(expected, _output.ToString());
        Assert.Equal(string.Empty, _errorOutput.ToString());
        Assert.Equal(0, statusCode);
    }

    [Fact]
    public async Task ShouldFailWhenPassedInvalidArgs()
    {
        // Act
        var statusCode = await Program.Main(new []
        {
            "--fail"
        });

        // Assert
        Assert.Equal(-1, statusCode);
    }
}
