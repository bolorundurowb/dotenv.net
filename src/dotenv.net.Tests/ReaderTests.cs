using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using FluentAssertions;
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
        act.Should().Throw<ArgumentException>().WithMessage("The file path cannot be null, empty or whitespace.*");
    }

    [Theory]
    [InlineData(null, true)]
    [InlineData("", true)]
    [InlineData("   ", true)]
    public void ReadFileLines_InvalidPathAndIgnoreExceptionsTrue_ShouldReturnEmptySpan(string path, bool ignoreExceptions)
    {
        var result = Reader.ReadFileLines(path, ignoreExceptions, null).ToArray();
        result.Should().BeEmpty();
    }

    [Fact]
    public void ReadFileLines_NonExistentFileAndIgnoreExceptionsFalse_ShouldThrowFileNotFoundException()
    {
        var path = "nonexistent.env";
        Action act = () => Reader.ReadFileLines(path, false, null);
        act.Should().Throw<FileNotFoundException>().WithMessage($"*{path}*");
    }

    [Fact]
    public void ReadFileLines_NonExistentFileAndIgnoreExceptionsTrue_ShouldReturnEmptySpan()
    {
        var result = Reader.ReadFileLines("nonexistent.env", true, null).ToArray();
        result.Should().BeEmpty();
    }

    [Fact]
    public void ReadFileLines_ValidFile_ShouldReturnLines()
    {
        File.WriteAllLines(_tempFilePath, new[] { "KEY1=value1", "KEY2=value2" });
        var result = Reader.ReadFileLines(_tempFilePath, false, null);
        result.Length.Should().Be(2);
        result[0].Should().Be("KEY1=value1");
        result[1].Should().Be("KEY2=value2");
    }

    [Fact]
    public void ReadFileLines_WithCustomEncoding_ShouldReturnCorrectContent()
    {
        var content = "KEY=üñîçø∂é";
        File.WriteAllText(_tempFilePath, content, Encoding.UTF32);
        var result = Reader.ReadFileLines(_tempFilePath, false, Encoding.UTF32);
        result[0].Should().Be(content);
    }

    [Fact]
    public void ExtractEnvKeyValues_EmptySpan_ShouldReturnEmptySpan()
    {
        var result = Reader.ExtractEnvKeyValues(ReadOnlySpan<string>.Empty, false).ToArray();
        result.Should().BeEmpty();
    }

    [Fact]
    public void ExtractEnvKeyValues_ValidLines_ShouldReturnKeyValuePairs()
    {
        var lines = new[] { "KEY1=value1", "KEY2=value2" };
        var result = Reader.ExtractEnvKeyValues(lines, false);
        result.Length.Should().Be(2);
        result[0].Should().Be(new KeyValuePair<string, string>("KEY1", "value1"));
        result[1].Should().Be(new KeyValuePair<string, string>("KEY2", "value2"));
    }

    [Fact]
    public void MergeEnvKeyValues_NoArrays_ShouldReturnEmptyDictionary()
    {
        var result = Reader.MergeEnvKeyValues(new List<KeyValuePair<string, string>[]>(), false);
        result.Should().BeEmpty();
    }

    [Fact]
    public void MergeEnvKeyValues_SingleArray_ShouldReturnAllItems()
    {
        var input = new[] {
            new[] { new KeyValuePair<string, string>("KEY1", "value1"), new KeyValuePair<string, string>("KEY2", "value2") }
        };
        var result = Reader.MergeEnvKeyValues(input, false);
        result.Should().HaveCount(2);
        result["KEY1"].Should().Be("value1");
        result["KEY2"].Should().Be("value2");
    }

    [Fact]
    public void MergeEnvKeyValues_MultipleArraysWithoutOverwrite_ShouldKeepFirstValue()
    {
        var input = new[] {
            new[] { new KeyValuePair<string, string>("KEY", "first") },
            new[] { new KeyValuePair<string, string>("KEY", "second") }
        };
        var result = Reader.MergeEnvKeyValues(input, false);
        result.Should().ContainSingle().Which.Value.Should().Be("first");
    }

    [Fact]
    public void MergeEnvKeyValues_MultipleArraysWithOverwrite_ShouldKeepLastValue()
    {
        var input = new[] {
            new[] { new KeyValuePair<string, string>("KEY", "first") },
            new[] { new KeyValuePair<string, string>("KEY", "second") }
        };
        var result = Reader.MergeEnvKeyValues(input, true);
        result.Should().ContainSingle().Which.Value.Should().Be("second");
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
        result.Should().HaveCount(3);
        result["KEY1"].Should().Be("value1");
        result["KEY2"].Should().Be("updated");
        result["KEY3"].Should().Be("value3");
    }

    [Fact]
    public void GetProbedEnvPath_FileFound_ShouldReturnPath()
    {
        var envPath = Path.Combine(_tempDirPath, ".env");
        File.WriteAllText(envPath, "TEST=value");
        var startDir = Path.Combine(_tempDirPath, "subdir1", "subdir2");
        Directory.CreateDirectory(startDir);

        using var dir = new TempWorkingDirectory(startDir);
        var result = Reader.GetProbedEnvPath(levelsToSearch: 3, ignoreExceptions: false);
        result.Should().Be(envPath);
    }

    [Fact]
    public void GetProbedEnvPath_FileNotFoundAndIgnoreExceptionsFalse_ShouldThrow()
    {
        using var dir = new TempWorkingDirectory(_tempDirPath);
        Action act = () => Reader.GetProbedEnvPath(levelsToSearch: 2, ignoreExceptions: false);
        act.Should().Throw<FileNotFoundException>()
            .WithMessage($"*{DotEnvOptions.DefaultEnvFileName}*");
    }

    [Fact]
    public void GetProbedEnvPath_FileNotFoundAndIgnoreExceptionsTrue_ShouldReturnNull()
    {
        using var dir = new TempWorkingDirectory(_tempDirPath);
        var result = Reader.GetProbedEnvPath(levelsToSearch: 2, ignoreExceptions: true);
        result.Should().BeNull();
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
        result.Should().BeNull();
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