using System;
using System.Collections.Generic;
using FluentAssertions;
using Xunit;

namespace dotenv.net.Tests;

public class ParserTests
{
    [Fact]
    public void Parse_EmptyLines_ShouldBeIgnored()
    {
        var lines = new[] { "", "  ", null, "KEY=value" };
        var result = Parser.Parse(lines, trimValues: false).ToArray();
        result.Should().ContainSingle().Which.Should().Be(new KeyValuePair<string, string>("KEY", "value"));
    }

    [Fact]
    public void Parse_CommentLines_ShouldBeIgnored()
    {
        var lines = new[] { "# Comment", " # Indented comment", "KEY=value" };
        var result = Parser.Parse(lines, trimValues: false).ToArray();
        result.Should().ContainSingle().Which.Should().Be(new KeyValuePair<string, string>("KEY", "value"));
    }

    [Fact]
    public void Parse_LinesWithoutKey_ShouldBeIgnored()
    {
        var lines = new[] { "=value", "NOKEY", "KEY=value" };
        var result = Parser.Parse(lines, trimValues: false).ToArray();
        result.Should().ContainSingle().Which.Should().Be(new KeyValuePair<string, string>("KEY", "value"));
    }

    [Fact]
    public void Parse_SimpleKeyValue_ShouldReturnPair()
    {
        var lines = new[] { "TEST_KEY=test_value" };
        var result = Parser.Parse(lines, trimValues: false).ToArray();
        result.Should().ContainSingle().Which.Should().Be(new KeyValuePair<string, string>("TEST_KEY", "test_value"));
    }

    [Fact]
    public void Parse_UntrimmedValueWithTrimValuesFalse_ShouldPreserveWhitespace()
    {
        var lines = new[] { "  KEY  =  value  " };
        var result = Parser.Parse(lines, trimValues: false).ToArray();
        result.Should().ContainSingle().Which.Should().Be(new KeyValuePair<string, string>("KEY", "  value  "));
    }

    [Fact]
    public void Parse_UntrimmedValueWithTrimValuesTrue_ShouldTrimValue()
    {
        var lines = new[] { "KEY=  value  " };
        var result = Parser.Parse(lines, trimValues: true).ToArray();
        result.Should().ContainSingle().Which.Should().Be(new KeyValuePair<string, string>("KEY", "value"));
    }

    [Fact]
    public void Parse_SingleQuotedValue_ShouldUnescapeQuotes()
    {
        var lines = new[] { "KEY='value with \\' quote'" };
        var result = Parser.Parse(lines, trimValues: false).ToArray();
        result.Should().ContainSingle().Which.Should()
            .Be(new KeyValuePair<string, string>("KEY", "value with ' quote"));
    }

    [Fact]
    public void Parse_DoubleQuotedValue_ShouldUnescapeQuotes()
    {
        var lines = new[] { "KEY=\"value with \\\" quote\"" };
        var result = Parser.Parse(lines, trimValues: false).ToArray();
        result.Should().ContainSingle().Which.Should()
            .Be(new KeyValuePair<string, string>("KEY", "value with \" quote"));
    }

    [Fact]
    public void Parse_EscapedBackslashes_ShouldUnescape()
    {
        var lines = new[] { "KEY='escaped \\\\ backslash'" };
        var result = Parser.Parse(lines, trimValues: false).ToArray();
        result.Should().ContainSingle().Which.Should()
            .Be(new KeyValuePair<string, string>("KEY", "escaped \\ backslash"));
    }

    [Fact]
    public void Parse_MultiLineValue_ShouldCombineLines()
    {
        var lines = new[] { "KEY='first line", "second line'", "NEXT=value" };
        var result = Parser.Parse(lines, trimValues: false).ToArray();
        result.Should().HaveCount(2);
        result[0].Should().Be(new KeyValuePair<string, string>("KEY", $"first line{Environment.NewLine}second line"));
        result[1].Should().Be(new KeyValuePair<string, string>("NEXT", "value"));
    }

    [Fact]
    public void Parse_UnclosedQuote_ShouldThrowException()
    {
        var lines = new[] { "KEY='unclosed quote" };
        Action act = () => Parser.Parse(lines, trimValues: false).ToArray();
        act.Should().Throw<ArgumentException>()
            .WithMessage("Unable to parse environment variable: KEY. Missing closing quote.");
    }

    [Fact]
    public void Parse_EscapedQuoteInMiddle_ShouldUnescape()
    {
        var lines = new[] { "KEY='before\\'after'" };
        var result = Parser.Parse(lines, trimValues: false).ToArray();
        result.Should().ContainSingle().Which.Should().Be(new KeyValuePair<string, string>("KEY", "before'after"));
    }

    [Fact]
    public void Parse_BackslashNotEscapingQuote_ShouldRemain()
    {
        var lines = new[] { "KEY='before\\after'" };
        var result = Parser.Parse(lines, trimValues: false).ToArray();
        result.Should().ContainSingle().Which.Should().Be(new KeyValuePair<string, string>("KEY", "before\\after"));
    }
}
