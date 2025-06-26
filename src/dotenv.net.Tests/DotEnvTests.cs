using System;
using Shouldly;
using Xunit;

namespace dotenv.net.Tests;

public class DotEnvTests
{
    [Fact]
    public void Read_ComplexExistingEnv_ShouldExtractValidValues()
    {
        var options = new DotEnvOptions(trimValues: true, probeForEnv: true, probeLevelsToSearch: 5);
        var values = DotEnv.Read(options);

        values.Count.ShouldBe(11);
        values.ShouldContainKeyAndValue("lower_case_key", "world");
        values.ShouldContainKeyAndValue("DOUBLE_QUOTES", "double");
        values.ShouldContainKeyAndValue("SINGLE_QUOTES", "single");
        values.ShouldContainKeyAndValue("BOOLEAN", "true");
        values.ShouldContainKeyAndValue("NUMERIC", "34.56");
        values.ShouldContainKeyAndValue("DOTTED.KEY", "spaced value");
        values.ShouldContainKeyAndValue("KeyWithNoValue", string.Empty);
        values.ShouldContainKeyAndValue("DOUBLE_QUOTE_EVEN_MORE_LINES",
            $"""this{Environment.NewLine}is{Environment.NewLine}"a{Environment.NewLine}multi-line{Environment.NewLine}  value""");
        values.ShouldContainKeyAndValue("OidcAuthentication:ClientId", "your-client-id");
        values.ShouldContainKeyAndValue("OidcAuthentication:ClientSecret", "your-client-secret");
    }
}