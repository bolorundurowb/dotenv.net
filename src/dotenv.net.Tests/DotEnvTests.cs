using System;
using FluentAssertions;
using Xunit;

namespace dotenv.net.Tests;

public class DotEnvTests
{
    [Fact]
    public void Read_ComplexExistingEnv_ShouldExtractValidValues()
    {
        var options = new DotEnvOptions(trimValues: true, probeForEnv: true, probeLevelsToSearch: 5);
        var values = DotEnv.Read(options);

        values.Should().ContainKey("lower_case_key").WhoseValue.Should().Be("world");
        values.Should().ContainKey("DOUBLE_QUOTES").WhoseValue.Should().Be("double");
        values.Should().ContainKey("SINGLE_QUOTES").WhoseValue.Should().Be("single");
        values.Should().ContainKey("BOOLEAN").WhoseValue.Should().Be("true");
        values.Should().ContainKey("NUMERIC").WhoseValue.Should().Be("34.56");
        values.Should().ContainKey("DOTTED.KEY").WhoseValue.Should().Be("spaced value");
        values.Should().ContainKey("KeyWithNoValue").WhoseValue.Should().BeEmpty();
        values.Should().ContainKey("DOUBLE_QUOTE_EVEN_MORE_LINES").WhoseValue.Should().Be(
            $"this{Environment.NewLine}is{Environment.NewLine}a{Environment.NewLine}multi-line{Environment.NewLine}  value");
    }
}
