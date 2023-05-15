using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace DcmAnonymize.Tests;

[Collection("DcmAnonymize")]
public class TestsForDcmAnonymize : IAsyncLifetime
{
    private readonly ITestOutputHelper _testOutputHelper;

    private StringBuilder _output = default!;
    private StringBuilder _errorOutput = default!;
    private StringWriter _outputWriter = default!;
    private StringWriter _errorOutputWriter = default!;
    private TextReader _inputReader = default!;
    private FileInfo _dicomFile = default!;
    private Program _program = default!;

    public TestsForDcmAnonymize(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper ?? throw new ArgumentNullException(nameof(testOutputHelper));
    }

    public Task InitializeAsync()
    {
        var testDataDirectory = new DirectoryInfo("./TestData");
        var sampleDicomFile = new FileInfo(Path.Join(testDataDirectory.Name, "SampleDicomFile.dcm"));
        var sampleDicomFileCopy = new FileInfo(Path.Join(testDataDirectory.Name, $"SampleDicomFile_{Guid.NewGuid()}.dcm"));
        File.Copy(sampleDicomFile.FullName, sampleDicomFileCopy.FullName);
        _dicomFile = sampleDicomFileCopy;
        _output = new StringBuilder();
        _outputWriter = new StringWriter(_output);
        _errorOutput = new StringBuilder();
        _errorOutputWriter = new StringWriter(_errorOutput);
        _inputReader = new StringReader(_dicomFile.FullName + Environment.NewLine);
        _program = new Program
        {
            Input = _inputReader,
            Output = _outputWriter,
            ErrorOutput = _errorOutputWriter
        };
        return Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        await _outputWriter.DisposeAsync();
        await _errorOutputWriter.DisposeAsync();
        _inputReader.Dispose();

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
                await Task.Delay(1000);
            }
        }
    }

    [Fact]
    public async Task ShouldAnonymizeTestFile()
    {
        // Arrange
        var expected = $"{_dicomFile.FullName}{Environment.NewLine}";

        // Act
        var statusCode = await _program.Run(new[]
        {
            _dicomFile.FullName
        });

        // Assert
        _testOutputHelper.WriteLine(_output.ToString());
        _testOutputHelper.WriteLine(_errorOutput.ToString());
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
        var statusCode = await _program.Run(Array.Empty<string>());

        // Assert
        _testOutputHelper.WriteLine(_output.ToString());
        _testOutputHelper.WriteLine(_errorOutput.ToString());
        Assert.Equal(expected, _output.ToString());
        Assert.Equal(string.Empty, _errorOutput.ToString());
        Assert.Equal(0, statusCode);
    }

    [Fact]
    public async Task ShouldFailWhenPassedInvalidArgs()
    {
        // Arrange + Act
        var statusCode = await _program.Run(new[]
        {
            "--fail"
        });

        // Assert
        _testOutputHelper.WriteLine(_output.ToString());
        _testOutputHelper.WriteLine(_errorOutput.ToString());
        Assert.Equal(-1, statusCode);
    }

    [Fact]
    public async Task ShouldSupportParallelism()
    {
        // Arrange
        var expected = $"{_dicomFile.FullName}{Environment.NewLine}";

        // Act
        var statusCode = await _program.Run(new[]
        {
            _dicomFile.FullName,
            "--parallelism", "4"
        });

        // Assert
        _testOutputHelper.WriteLine(_output.ToString());
        _testOutputHelper.WriteLine(_errorOutput.ToString());
        Assert.Equal(expected, _output.ToString());
        Assert.Equal(string.Empty, _errorOutput.ToString());
        Assert.Equal(0, statusCode);
    }

    [Fact]
    public async Task ShouldSupportBlankingRectangles()
    {
        // Arrange
        var expected = $"{_dicomFile.FullName}{Environment.NewLine}";

        // Act
        var statusCode = await _program.Run(new[]
        {
            _dicomFile.FullName,
            "--blank-rectangle", "(25,25)->(50,50)"
        });

        // Assert
        _testOutputHelper.WriteLine(_output.ToString());
        _testOutputHelper.WriteLine(_errorOutput.ToString());
        Assert.Equal(expected, _output.ToString());
        Assert.Equal(string.Empty, _errorOutput.ToString());
        Assert.Equal(0, statusCode);
    }
}
