using System;
using System.IO;
using FluentAssertions;
using Xunit;

namespace dotenv.net.Tests;

public class ReaderTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Read_WhenFilePathIsNullOrWhiteSpaceAndNotIgnoringExceptions_ThrowsArgumentException(string filePath)
    {
        Action act = () => Reader.ReadFileLines(filePath, ignoreExceptions: false, encoding: null);

        act.Should().Throw<ArgumentException>()
            .WithMessage("The file path cannot be null, empty or whitespace. (Parameter 'envFilePath')")
            .WithParameterName("envFilePath");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Read_WhenFilePathIsNullOrWhiteSpaceAndIgnoringExceptions_ReturnsEmptySpan(string filePath)
    {
        var result = Reader.ReadFileLines(filePath, ignoreExceptions: true, encoding: null);

        result.IsEmpty.Should().BeTrue();
    }

    [Fact]
    public void Read_WhenFileDoesNotExistAndNotIgnoringExceptions_ThrowsFileNotFoundException()
    {
        const string nonExistentPath = "nonexistent.env";
        Action act = () => Reader.ReadFileLines(nonExistentPath, ignoreExceptions: false, encoding: null);

        act.Should().Throw<FileNotFoundException>()
            .WithMessage($"A file with provided path \"{nonExistentPath}\" does not exist.");
    }

    [Fact]
    public void Read_WhenFileDoesNotExistAndIgnoringExceptions_ReturnsEmptySpan()
    {
        const string nonExistentPath = "nonexistent.env";
        var result = Reader.ReadFileLines(nonExistentPath, ignoreExceptions: true, encoding: null);

        result.IsEmpty.Should().BeTrue();
    }
}
