using FluentAssertions;
using Xunit;

namespace dotenv.net.Tests;

public class DotEnvTests
{
    [Fact]
    public void Read_ComplexExistingEnv_ShouldExtractValidValues()
    {
        var values = DotEnv.Read();
        values.Should().ContainKey("hello").WhoseValue.Should().Be("world");
    }
}
