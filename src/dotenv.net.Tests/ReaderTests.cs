using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Shouldly;
using Xunit;

namespace dotenv.net.Tests;

public class ReaderTests : IDisposable
{
    private readonly string _tempFilePath;
    private readonly string _tempDirPath;

    private readonly string _testRootPath;
    private readonly string _startPath;
    private readonly string _originalBaseDirectory;

    public ReaderTests()
    {
        _tempFilePath = Path.GetTempFileName();
        _tempDirPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(_tempDirPath);

        // Create a unique root directory for this test run in the system's temp folder.
        _testRootPath = Path.Combine(Path.GetTempPath(), "DotEnvTests_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_testRootPath);

        // Create a nested structure to simulate parent directories.
        // StartPath will be our simulated AppContext.BaseDirectory.
        _startPath = Path.Combine(_testRootPath, "level1", "level2");
        Directory.CreateDirectory(_startPath);

        // HACK: To robustly test the method without changing its source code,
        // we temporarily set the AppContext.BaseDirectory to our controlled test path.
        _originalBaseDirectory = (string) AppContext.GetData("APP_CONTEXT_BASE_DIRECTORY")!;
        AppDomain.CurrentDomain.SetData("APP_CONTEXT_BASE_DIRECTORY", _startPath);
    }

    public void Dispose()
    {
        if (File.Exists(_tempFilePath))
            File.Delete(_tempFilePath);

        if (Directory.Exists(_tempDirPath))
            Directory.Delete(_tempDirPath, true);

        AppDomain.CurrentDomain.SetData("APP_CONTEXT_BASE_DIRECTORY", _originalBaseDirectory);

        if (Directory.Exists(_testRootPath))
            Directory.Delete(_testRootPath, true);
    }

    [Theory]
    [InlineData(null, false)]
    [InlineData("", false)]
    [InlineData("   ", false)]
    public void ReadFileLines_InvalidPathAndIgnoreExceptionsFalse_ShouldThrowArgumentException(string path,
        bool ignoreExceptions)
    {
        Action act = () => Reader.ReadFileLines(path, ignoreExceptions, null);
        act.ShouldThrow<ArgumentException>().Message
            .ShouldContain("The file path cannot be null, empty or whitespace.");
    }

    [Theory]
    [InlineData(null, true)]
    [InlineData("", true)]
    [InlineData("   ", true)]
    public void ReadFileLines_InvalidPathAndIgnoreExceptionsTrue_ShouldReturnEmptySpan(string path,
        bool ignoreExceptions)
    {
        var result = Reader.ReadFileLines(path, ignoreExceptions, null).ToArray();
        result.ShouldBeEmpty();
    }

    [Fact]
    public void ReadFileLines_NonExistentFileAndIgnoreExceptionsFalse_ShouldThrowFileNotFoundException()
    {
        var path = "nonexistent.env";
        Action act = () => Reader.ReadFileLines(path, false, null);
        act.ShouldThrow<FileNotFoundException>().Message.ShouldContain(path);
    }

    [Fact]
    public void ReadFileLines_NonExistentFileAndIgnoreExceptionsTrue_ShouldReturnEmptySpan()
    {
        var result = Reader.ReadFileLines("nonexistent.env", true, null).ToArray();
        result.ShouldBeEmpty();
    }

    [Fact]
    public void ReadFileLines_ValidFile_ShouldReturnLines()
    {
        File.WriteAllLines(_tempFilePath, ["KEY1=value1", "KEY2=value2"]);
        var result = Reader.ReadFileLines(_tempFilePath, false, null);
        result.Length.ShouldBe(2);
        result[0].ShouldBe("KEY1=value1");
        result[1].ShouldBe("KEY2=value2");
    }

    [Fact]
    public void ReadFileLines_WithCustomEncoding_ShouldReturnCorrectContent()
    {
        var content = "KEY=üñîçø∂é";
        File.WriteAllText(_tempFilePath, content, Encoding.UTF32);
        var result = Reader.ReadFileLines(_tempFilePath, false, Encoding.UTF32);
        result[0].ShouldBe(content);
    }

    [Fact]
    public void ExtractEnvKeyValues_EmptySpan_ShouldReturnEmptySpan()
    {
        var result = Reader.ExtractEnvKeyValues(ReadOnlySpan<string>.Empty, false).ToArray();
        result.ShouldBeEmpty();
    }

    [Fact]
    public void ExtractEnvKeyValues_ValidLines_ShouldReturnKeyValuePairs()
    {
        var lines = new[] { "KEY1=value1", "KEY2=value2" };
        var result = Reader.ExtractEnvKeyValues(lines, false);
        result.Length.ShouldBe(2);
        result[0].ShouldBe(new KeyValuePair<string, string>("KEY1", "value1"));
        result[1].ShouldBe(new KeyValuePair<string, string>("KEY2", "value2"));
    }

    [Fact]
    public void MergeEnvKeyValues_NoArrays_ShouldReturnEmptyDictionary()
    {
        var result = Reader.MergeEnvKeyValues(new List<KeyValuePair<string, string>[]>(), false);
        result.ShouldBeEmpty();
    }

    [Fact]
    public void MergeEnvKeyValues_SingleArray_ShouldReturnAllItems()
    {
        var input = new[]
        {
            new[]
            {
                new KeyValuePair<string, string>("KEY1", "value1"), new KeyValuePair<string, string>("KEY2", "value2")
            }
        };
        var result = Reader.MergeEnvKeyValues(input, false);
        result.ShouldBe(new Dictionary<string, string> { { "KEY1", "value1" }, { "KEY2", "value2" } });
    }

    [Fact]
    public void MergeEnvKeyValues_MultipleArraysWithoutOverwrite_ShouldKeepFirstValue()
    {
        var input = new[]
        {
            new[] { new KeyValuePair<string, string>("KEY", "first") },
            new[] { new KeyValuePair<string, string>("KEY", "second") }
        };
        var result = Reader.MergeEnvKeyValues(input, false);
        result.ShouldContainKey("KEY");
        result["KEY"].ShouldBe("first");
    }

    [Fact]
    public void MergeEnvKeyValues_MultipleArraysWithOverwrite_ShouldKeepLastValue()
    {
        var input = new[]
        {
            new[] { new KeyValuePair<string, string>("KEY", "first") },
            new[] { new KeyValuePair<string, string>("KEY", "second") }
        };
        var result = Reader.MergeEnvKeyValues(input, true);
        result.ShouldContainKey("KEY");
        result["KEY"].ShouldBe("second");
    }

    [Fact]
    public void MergeEnvKeyValues_ComplexMerge_ShouldHandleAllCases()
    {
        var input = new[]
        {
            new[]
            {
                new KeyValuePair<string, string>("KEY1", "value1"),
                new KeyValuePair<string, string>("KEY2", "value2")
            },
            new[]
            {
                new KeyValuePair<string, string>("KEY2", "updated"),
                new KeyValuePair<string, string>("KEY3", "value3")
            }
        };
        var result = Reader.MergeEnvKeyValues(input, true);
        result.ShouldBe(new Dictionary<string, string>
            { { "KEY1", "value1" }, { "KEY2", "updated" }, { "KEY3", "value3" } });
    }

    [Fact]
    public void GetProbedEnvPath_FileNotFoundAndIgnoreExceptionsFalse_ShouldThrow()
    {
        Action act = () => Reader.GetProbedEnvPath(levelsToSearch: 2, ignoreExceptions: false);
        act.ShouldThrow<FileNotFoundException>()
            .Message.ShouldContain(DotEnvOptions.DefaultEnvFileName);
    }

    [Fact]
    public void GetProbedEnvPath_ShouldFindFile_InCurrentDirectory()
    {
        var expectedPath = Path.Combine(_startPath, DotEnvOptions.DefaultEnvFileName);
        CreateEnvFileAt(_startPath);

        var result = Reader.GetProbedEnvPath(levelsToSearch: 0, ignoreExceptions: true).ToList();

        result.ShouldHaveSingleItem();
        result.First().ShouldBe(expectedPath);
    }

    [Fact]
    public void GetProbedEnvPath_ShouldFindFile_InParentDirectory()
    {
        var parentDirectory = Directory.GetParent(_startPath)!.FullName;
        var expectedPath = Path.Combine(parentDirectory, DotEnvOptions.DefaultEnvFileName);
        CreateEnvFileAt(parentDirectory);

        var result = Reader.GetProbedEnvPath(levelsToSearch: 1, ignoreExceptions: true).ToList();

        result.ShouldHaveSingleItem();
        result.First().ShouldBe(expectedPath);
    }

    [Fact]
    public void GetProbedEnvPath_ShouldFindFile_TwoLevelsUp()
    {
        var grandParentDirectory = Directory.GetParent(_startPath)!.Parent!.FullName;
        var expectedPath = Path.Combine(grandParentDirectory, DotEnvOptions.DefaultEnvFileName);
        CreateEnvFileAt(grandParentDirectory);

        var result = Reader.GetProbedEnvPath(levelsToSearch: 2, ignoreExceptions: true).ToList();

        result.ShouldHaveSingleItem();
        result.First().ShouldBe(expectedPath);
    }

    [Fact]
    public void GetProbedEnvPath_ShouldReturnEmpty_WhenFileNotFoundAndExceptionsIgnored()
    {
        var result = Reader.GetProbedEnvPath(levelsToSearch: 3, ignoreExceptions: true);
        result.ShouldBeEmpty();
    }

    [Fact]
    public void GetProbedEnvPath_ShouldThrow_WhenFileNotFoundAndExceptionsNotIgnored()
    {
        // No .env file is created.
        var levelsToSearch = 2;
        var parentPath = Directory.GetParent(_startPath)!.FullName;
        var grandParentPath = Directory.GetParent(parentPath)!.FullName;

        var exception = Should.Throw<FileNotFoundException>(() =>
        {
            Reader.GetProbedEnvPath(levelsToSearch, ignoreExceptions: false);
        });

        exception.Message.ShouldContain($"Could not find '{DotEnvOptions.DefaultEnvFileName}'");
        exception.Message.ShouldContain($"after searching {levelsToSearch} directory level(s) upwards.");
        exception.Message.ShouldContain("Searched paths:");
        exception.Message.ShouldContain(_startPath);      // Searched level 0
        exception.Message.ShouldContain(parentPath);      // Searched level 1
        exception.Message.ShouldContain(grandParentPath); // Searched level 2
    }

    [Fact]
    public void GetProbedEnvPath_ShouldReturnEmpty_WhenFileExistsButIsOutOfSearchRange()
    {
        // File is at level 2, but we only search up to level 1.
        var grandParentDirectory = Directory.GetParent(_startPath)!.Parent!.FullName;
        CreateEnvFileAt(grandParentDirectory);

        var result = Reader.GetProbedEnvPath(levelsToSearch: 1, ignoreExceptions: true);

        result.ShouldBeEmpty();
    }

    [Fact]
    public void GetProbedEnvPath_ShouldThrow_WhenFileExistsButIsOutOfSearchRange()
    {
        // File is at level 2, but we only search up to level 1.
        var grandParentDirectory = Directory.GetParent(_startPath)!.Parent!.FullName;
        CreateEnvFileAt(grandParentDirectory);

        var exception = Should.Throw<FileNotFoundException>(() =>
        {
            Reader.GetProbedEnvPath(levelsToSearch: 1, ignoreExceptions: false);
        });

        exception.Message.ShouldContain($"Could not find '{DotEnvOptions.DefaultEnvFileName}'");
    }

    private void CreateEnvFileAt(string directoryPath) =>
        File.WriteAllText(Path.Combine(directoryPath, DotEnvOptions.DefaultEnvFileName), "TEST=true");
}
