using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Shouldly;
using Xunit;

namespace dotenv.net.Tests;

public class ReaderTests : IDisposable
{
    private readonly string _tempFilePath;
    private readonly string _tempDirPath;

    public ReaderTests()
    {
        _tempFilePath = Path.GetTempFileName();
        _tempDirPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(_tempDirPath);
    }

    public void Dispose()
    {
        if (File.Exists(_tempFilePath))
            File.Delete(_tempFilePath);
            
        if (Directory.Exists(_tempDirPath))
            Directory.Delete(_tempDirPath, true);
    }

    [Theory]
    [InlineData(null, false)]
    [InlineData("", false)]
    [InlineData("   ", false)]
    public void ReadFileLines_InvalidPathAndIgnoreExceptionsFalse_ShouldThrowArgumentException(string path, bool ignoreExceptions)
    {
        Action act = () => Reader.ReadFileLines(path, ignoreExceptions, null);
        act.ShouldThrow<ArgumentException>().Message.ShouldContain("The file path cannot be null, empty or whitespace.");
    }

    [Theory]
    [InlineData(null, true)]
    [InlineData("", true)]
    [InlineData("   ", true)]
    public void ReadFileLines_InvalidPathAndIgnoreExceptionsTrue_ShouldReturnEmptySpan(string path, bool ignoreExceptions)
    {
        var result = Reader.ReadFileLines(path, ignoreExceptions, null).ToArray();
        result.ShouldBeEmpty();
    }

    [Fact]
    public void ReadFileLines_NonExistentFileAndIgnoreExceptionsFalse_ShouldThrowFileNotFoundException()
    {
        var path = "nonexistent.env";
        Action act = () => Reader.ReadFileLines(path, false, null);
        act.ShouldThrow<FileNotFoundException>().Message.ShouldContain (path);
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
        var input = new[] {
            new[] { new KeyValuePair<string, string>("KEY1", "value1"), new KeyValuePair<string, string>("KEY2", "value2") }
        };
        var result = Reader.MergeEnvKeyValues(input, false);
        result.ShouldBe(new Dictionary<string, string> { { "KEY1", "value1" }, { "KEY2", "value2" } });
    }

    [Fact]
    public void MergeEnvKeyValues_MultipleArraysWithoutOverwrite_ShouldKeepFirstValue()
    {
        var input = new[] {
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
        var input = new[] {
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
        var input = new[] {
            new[] {
                new KeyValuePair<string, string>("KEY1", "value1"),
                new KeyValuePair<string, string>("KEY2", "value2")
            },
            new[] {
                new KeyValuePair<string, string>("KEY2", "updated"),
                new KeyValuePair<string, string>("KEY3", "value3")
            }
        };
        var result = Reader.MergeEnvKeyValues(input, true);
        result.ShouldBe(new Dictionary<string, string> { { "KEY1", "value1" }, { "KEY2", "updated" }, { "KEY3", "value3" } });
    }

    [Fact]
    public void GetProbedEnvPath_FileNotFoundAndIgnoreExceptionsFalse_ShouldThrow()
    {
        using var dir = new TempWorkingDirectory(_tempDirPath);
        Action act = () => Reader.GetProbedEnvPath(levelsToSearch: 2, ignoreExceptions: false);
        act.ShouldThrow<FileNotFoundException>()
            .Message.ShouldContain(DotEnvOptions.DefaultEnvFileName);
    }

    [Fact]
    public void GetProbedEnvPath_FileNotFoundAndIgnoreExceptionsTrue_ShouldReturnNull()
    {
        using var dir = new TempWorkingDirectory(_tempDirPath);
        var result = Reader.GetProbedEnvPath(levelsToSearch: 2, ignoreExceptions: true);
        result.ShouldBeNull();
    }

    [Fact]
    public void GetProbedEnvPath_LevelsTooLow_ShouldNotFindFile()
    {
        var envPath = Path.Combine(_tempDirPath, ".env");
        File.WriteAllText(envPath, "TEST=value");
        var startDir = Path.Combine(_tempDirPath, "subdir1", "subdir2", "subdir3");
        Directory.CreateDirectory(startDir);

        using var dir = new TempWorkingDirectory(startDir);
        var result = Reader.GetProbedEnvPath(levelsToSearch: 2, ignoreExceptions: true);
        result.ShouldBeNull();
    }

    private class TempWorkingDirectory : IDisposable
    {
        private readonly string _originalDirectory;

        public TempWorkingDirectory(string path)
        {
            _originalDirectory = Directory.GetCurrentDirectory();
            Directory.SetCurrentDirectory(path);
        }

        public void Dispose() => Directory.SetCurrentDirectory(_originalDirectory);
    }
}