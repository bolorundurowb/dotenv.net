using System;
using System.Collections.Generic;
using Shouldly;
using Xunit;

namespace dotenv.net.Tests;

public class ParserTests
{
    [Fact]
    public void Parse_EmptyLines_ShouldBeIgnored()
    {
        var lines = new[] { "", "  ", null, "KEY=value" };
        var result = Parser.Parse(lines, trimValues: false, supportExportSyntax: false, supportInlineComments: false).ToArray();
        result.Length.ShouldBe(1);
        result[0].ShouldBe(new KeyValuePair<string, string>("KEY", "value"));
    }

    [Fact]
    public void Parse_CommentLines_ShouldBeIgnored()
    {
        var lines = new[] { "# Comment", " # Indented comment", "KEY=value" };
        var result = Parser.Parse(lines, trimValues: false, supportExportSyntax: false, supportInlineComments: false).ToArray();
        result.Length.ShouldBe(1);
        result[0].ShouldBe(new KeyValuePair<string, string>("KEY", "value"));
    }

    [Fact]
    public void Parse_LinesWithoutKey_ShouldBeIgnored()
    {
        var lines = new[] { "=value", "NOKEY", "KEY=value" };
        var result = Parser.Parse(lines, trimValues: false, supportExportSyntax: false, supportInlineComments: false).ToArray();
        result.Length.ShouldBe(1);
        result[0].ShouldBe(new KeyValuePair<string, string>("KEY", "value"));
    }

    [Fact]
    public void Parse_WhitespaceKey_ShouldBeIgnored()
    {
        var lines = new[] { " =value", "KEY=value" };
        var result = Parser.Parse(lines, trimValues: false, supportExportSyntax: false, supportInlineComments: false).ToArray();
        result.Length.ShouldBe(1);
        result[0].ShouldBe(new KeyValuePair<string, string>("KEY", "value"));
    }

    [Fact]
    public void Parse_SimpleKeyValue_ShouldReturnPair()
    {
        var lines = new[] { "TEST_KEY=test_value" };
        var result = Parser.Parse(lines, trimValues: false, supportExportSyntax: false, supportInlineComments: false).ToArray();
        result.Length.ShouldBe(1);
        result[0].ShouldBe(new KeyValuePair<string, string>("TEST_KEY", "test_value"));
    }

    [Fact]
    public void Parse_UntrimmedValueWithTrimValuesFalse_ShouldPreserveWhitespace()
    {
        var lines = new[] { "  KEY  =  value  " };
        var result = Parser.Parse(lines, trimValues: false, supportExportSyntax: false, supportInlineComments: false).ToArray();
        result.Length.ShouldBe(1);
        result[0].ShouldBe(new KeyValuePair<string, string>("KEY", "  value  "));
    }

    [Fact]
    public void Parse_UntrimmedValueWithTrimValuesTrue_ShouldTrimValue()
    {
        var lines = new[] { "KEY=  value  " };
        var result = Parser.Parse(lines, trimValues: true, supportExportSyntax: false, supportInlineComments: false).ToArray();
        result.Length.ShouldBe(1);
        result[0].ShouldBe(new KeyValuePair<string, string>("KEY", "value"));
    }

    [Fact]
    public void Parse_SingleQuotedValue_ShouldUnescapeQuotes()
    {
        var lines = new[] { "KEY='value with \\' quote'" };
        var result = Parser.Parse(lines, trimValues: false, supportExportSyntax: false, supportInlineComments: false).ToArray();
        result.Length.ShouldBe(1);
        result[0].ShouldBe(new KeyValuePair<string, string>("KEY", "value with ' quote"));
    }

    [Fact]
    public void Parse_DoubleQuotedValue_ShouldUnescapeQuotes()
    {
        var lines = new[] { "KEY=\"value with \\\" quote\"" };
        var result = Parser.Parse(lines, trimValues: false, supportExportSyntax: false, supportInlineComments: false).ToArray();
        result.Length.ShouldBe(1);
        result[0].ShouldBe(new KeyValuePair<string, string>("KEY", "value with \" quote"));
    }

    [Fact]
    public void Parse_EscapedBackslashes_ShouldUnescape()
    {
        var lines = new[] { "KEY='escaped \\\\ backslash'" };
        var result = Parser.Parse(lines, trimValues: false, supportExportSyntax: false, supportInlineComments: false).ToArray();
        result.Length.ShouldBe(1);
        result.ShouldContain(new KeyValuePair<string, string>("KEY", "escaped \\ backslash"));
    }

    [Fact]
    public void Parse_MultiLineValue_ShouldCombineLines()
    {
        var lines = new[] { "KEY='first line", "second line'", "NEXT=value" };
        var result = Parser.Parse(lines, trimValues: false, supportExportSyntax: false, supportInlineComments: false).ToArray();
        result.Length.ShouldBe(2);
        result[0].ShouldBe(new KeyValuePair<string, string>("KEY", $"first line{Environment.NewLine}second line"));
        result[1].ShouldBe(new KeyValuePair<string, string>("NEXT", "value"));
    }

    [Fact]
    public void Parse_MultiLineValueWithNullLine_ShouldTreatAsEmpty()
    {
        var lines = new[] { "KEY='first", null, "last'" };
        var result = Parser.Parse(lines, trimValues: false, supportExportSyntax: false, supportInlineComments: false).ToArray();
        result.Length.ShouldBe(1);
        result[0].ShouldBe(new KeyValuePair<string, string>("KEY",
            $"first{Environment.NewLine}{Environment.NewLine}last"));
    }

    [Fact]
    public void Parse_UnclosedQuote_ShouldThrowException()
    {
        var lines = new[] { "KEY='unclosed quote" };
        Action act = () => Parser.Parse(lines, trimValues: false, supportExportSyntax: false, supportInlineComments: false).ToArray();
        act.ShouldThrow<ArgumentException>()
            .Message.ShouldBe("Unable to parse environment variable: KEY. Missing closing quote.");
    }

    [Fact]
    public void Parse_EscapedQuoteInMiddle_ShouldUnescape()
    {
        var lines = new[] { "KEY='before\\'after'" };
        var result = Parser.Parse(lines, trimValues: false, supportExportSyntax: false, supportInlineComments: false).ToArray();
        result.Length.ShouldBe(1);
        result.ShouldContain(new KeyValuePair<string, string>("KEY", "before'after"));
    }

    [Fact]
    public void Parse_ExportEscapedQuoteInMiddle_ShouldUnescape()
    {
        var lines = new[] { "export KEY='before\\'after'" };
        var result = Parser.Parse(lines, trimValues: false, supportExportSyntax: true, supportInlineComments: false).ToArray();
        result.Length.ShouldBe(1);
        result.ShouldContain(new KeyValuePair<string, string>("KEY", "before'after"));
    }

    [Fact]
    public void Parse_BackslashNotEscapingQuote_ShouldRemain()
    {
        var lines = new[] { "KEY='before\\after'" };
        var result = Parser.Parse(lines, trimValues: false, supportExportSyntax: false, supportInlineComments: false).ToArray();
        result.Length.ShouldBe(1);
        result.ShouldContain(new KeyValuePair<string, string>("KEY", "before\\after"));
    }

    [Fact]
    public void Parse_InlineComment_ShouldBeStripped()
    {
        var lines = new[] { "KEY=value # comment" };
        var result = Parser.Parse(lines, trimValues: false, supportExportSyntax: false, supportInlineComments: true).ToArray();
        result.Length.ShouldBe(1);
        result[0].ShouldBe(new KeyValuePair<string, string>("KEY", "value"));
    }

    [Fact]
    public void Parse_InlineCommentWithoutPrecedingWhitespace_ShouldRemain()
    {
        var lines = new[] { "KEY=value#not-a-comment" };
        var result = Parser.Parse(lines, trimValues: false, supportExportSyntax: false, supportInlineComments: true).ToArray();
        result.Length.ShouldBe(1);
        result[0].ShouldBe(new KeyValuePair<string, string>("KEY", "value#not-a-comment"));
    }

    [Fact]
    public void Parse_InlineCommentOnSingleQuotedValue_ShouldRemain()
    {
        var lines = new[] { "KEY='value # not a comment'" };
        var result = Parser.Parse(lines, trimValues: false, supportExportSyntax: false, supportInlineComments: true).ToArray();
        result.Length.ShouldBe(1);
        result[0].ShouldBe(new KeyValuePair<string, string>("KEY", "value # not a comment"));
    }

    [Fact]
    public void Parse_InlineCommentOnDoubleQuotedValue_ShouldRemain()
    {
        var lines = new[] { "KEY=\"value # not a comment\"" };
        var result = Parser.Parse(lines, trimValues: false, supportExportSyntax: false, supportInlineComments: true).ToArray();
        result.Length.ShouldBe(1);
        result[0].ShouldBe(new KeyValuePair<string, string>("KEY", "value # not a comment"));
    }

    [Fact]
    public void Parse_HashAtValueStart_ShouldRemain()
    {
        var lines = new[] { "KEY=#fff" };
        var result = Parser.Parse(lines, trimValues: false, supportExportSyntax: false, supportInlineComments: true).ToArray();
        result.Length.ShouldBe(1);
        result[0].ShouldBe(new KeyValuePair<string, string>("KEY", "#fff"));
    }

    [Fact]
    public void Parse_InlineCommentWhenDisabled_ShouldRemain()
    {
        var lines = new[] { "KEY=value # comment" };
        var result = Parser.Parse(lines, trimValues: false, supportExportSyntax: false, supportInlineComments: false).ToArray();
        result.Length.ShouldBe(1);
        result[0].ShouldBe(new KeyValuePair<string, string>("KEY", "value # comment"));
    }

    [Fact]
    public void Parse_WithVariableExpansion_SingleQuotedValue_ShouldPreserveLiteralWithoutExpansion()
    {
        var lines = new[]
        {
            "VAR=resolved",
            "SINGLE='${VAR}'"
        };
        var result = Parser.Parse(lines, trimValues: false, supportExportSyntax: false, supportInlineComments: true,
            supportVariableExpansion: true).ToArray();

        result.Length.ShouldBe(2);
        result[0].ShouldBe(new KeyValuePair<string, string>("VAR", "resolved"));
        result[1].ShouldBe(new KeyValuePair<string, string>("SINGLE", "${VAR}"));
    }

    [Fact]
    public void Parse_WithVariableExpansion_DoubleQuotedValue_ShouldExpandVariables()
    {
        var lines = new[]
        {
            "HOST=example.com",
            "URL=\"https://${HOST}/api\""
        };
        var result = Parser.Parse(lines, trimValues: false, supportExportSyntax: false, supportInlineComments: true,
            supportVariableExpansion: true).ToArray();

        result.Length.ShouldBe(2);
        result[1].ShouldBe(new KeyValuePair<string, string>("URL", "https://example.com/api"));
    }

    [Fact]
    public void Parse_WithVariableExpansion_UnquotedValue_ShouldExpandVariables()
    {
        var lines = new[]
        {
            "BASE=http://localhost",
            "PORT=5000",
            "ENDPOINT=$BASE:$PORT/v1"
        };
        var result = Parser.Parse(lines, trimValues: false, supportExportSyntax: false, supportInlineComments: true,
            supportVariableExpansion: true).ToArray();

        result.Length.ShouldBe(3);
        result[2].ShouldBe(new KeyValuePair<string, string>("ENDPOINT", "http://localhost:5000/v1"));
    }

    [Fact]
    public void Parse_WithVariableExpansion_SequentialDependency_ShouldResolveEarlierVariables()
    {
        var lines = new[]
        {
            "A=foo",
            "B=${A}_bar",
            "C=${B}_baz"
        };
        var result = Parser.Parse(lines, trimValues: false, supportExportSyntax: false, supportInlineComments: true,
            supportVariableExpansion: true).ToArray();

        result.Length.ShouldBe(3);
        result[0].ShouldBe(new KeyValuePair<string, string>("A", "foo"));
        result[1].ShouldBe(new KeyValuePair<string, string>("B", "foo_bar"));
        result[2].ShouldBe(new KeyValuePair<string, string>("C", "foo_bar_baz"));
    }

    [Fact]
    public void Parse_WithVariableExpansion_Disabled_ShouldNotExpandVariables()
    {
        var lines = new[]
        {
            "A=foo",
            "B=${A}"
        };
        var result = Parser.Parse(lines, trimValues: false, supportExportSyntax: false, supportInlineComments: true,
            supportVariableExpansion: false).ToArray();

        result.Length.ShouldBe(2);
        result[1].ShouldBe(new KeyValuePair<string, string>("B", "${A}"));
    }

    [Fact]
    public void Parse_WithVariableExpansion_InlineCommentOnUnquotedValue_ShouldStripCommentAndExpand()
    {
        var lines = new[]
        {
            "BASE=https://example.com",
            "URL=${BASE}/v1 # this is an endpoint"
        };
        var result = Parser.Parse(lines, trimValues: false, supportExportSyntax: false, supportInlineComments: true,
            supportVariableExpansion: true).ToArray();

        result.Length.ShouldBe(2);
        result[1].ShouldBe(new KeyValuePair<string, string>("URL", "https://example.com/v1"));
    }

    [Fact]
    public void Parse_WithVariableExpansion_EscapedVariableInDoubleQuotes_ShouldPreserveLiteral()
    {
        var lines = new[]
        {
            "VAR=hello",
            "ESCAPED=\"\\${VAR}\""
        };
        var result = Parser.Parse(lines, trimValues: false, supportExportSyntax: false, supportInlineComments: true,
            supportVariableExpansion: true).ToArray();

        result.Length.ShouldBe(2);
        result[1].ShouldBe(new KeyValuePair<string, string>("ESCAPED", "${VAR}"));
    }

    [Fact]
    public void Parse_WithVariableExpansion_SingleQuotedReferencedLater_ShouldNotExpandInternalTokens()
    {
        var lines = new[]
        {
            "SECRET='p@ss$word'",
            "MY_SECRET=\"${SECRET}\""
        };
        var result = Parser.Parse(lines, trimValues: false, supportExportSyntax: false, supportInlineComments: true,
            supportVariableExpansion: true).ToArray();

        result.Length.ShouldBe(2);
        result[0].ShouldBe(new KeyValuePair<string, string>("SECRET", "p@ss$word"));
        result[1].ShouldBe(new KeyValuePair<string, string>("MY_SECRET", "p@ss$word"));
    }
}
