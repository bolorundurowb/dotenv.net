using System;
using dotenv.net.Utilities;
using FluentAssertions;
using Xunit;

namespace dotenv.net.Tests.Utilities;

public class EnvReaderTests
{
    private const string TestKey = "TEST_ENV_KEY";

    [Fact]
    public void GetStringValue_KeyExists_ReturnsValue()
    {
        SetTestVariable("ValidValue");
        var result = EnvReader.GetStringValue(TestKey);
        result.Should().Be("ValidValue");
    }

    [Fact]
    public void GetStringValue_KeyMissing_ThrowsException()
    {
        ClearTestVariable();
        Action act = () => EnvReader.GetStringValue(TestKey);
        act.Should().Throw<Exception>().WithMessage("Value could not be retrieved.");
    }

    [Fact]
    public void GetIntValue_ValidInteger_ReturnsValue()
    {
        SetTestVariable("42");
        var result = EnvReader.GetIntValue(TestKey);
        result.Should().Be(42);
    }

    [Fact]
    public void GetIntValue_InvalidInteger_ThrowsException()
    {
        SetTestVariable("Invalid");
        Action act = () => EnvReader.GetIntValue(TestKey);
        act.Should().Throw<Exception>().WithMessage("Value could not be retrieved.");
    }

    [Fact]
    public void GetDoubleValue_ValidDouble_ReturnsValue()
    {
        SetTestVariable("3.14");
        var result = EnvReader.GetDoubleValue(TestKey);
        result.Should().Be(3.14);
    }

    [Fact]
    public void GetDoubleValue_InvalidDouble_ThrowsException()
    {
        SetTestVariable("Invalid");
        Action act = () => EnvReader.GetDoubleValue(TestKey);
        act.Should().Throw<Exception>().WithMessage("Value could not be retrieved.");
    }

    [Fact]
    public void GetDecimalValue_ValidDecimal_ReturnsValue()
    {
        SetTestVariable("99.99");
        var result = EnvReader.GetDecimalValue(TestKey);
        result.Should().Be(99.99m);
    }

    [Fact]
    public void GetDecimalValue_InvalidDecimal_ThrowsException()
    {
        SetTestVariable("Invalid");
        Action act = () => EnvReader.GetDecimalValue(TestKey);
        act.Should().Throw<Exception>().WithMessage("Value could not be retrieved.");
    }

    [Theory]
    [InlineData("true", true)]
    [InlineData("false", false)]
    [InlineData("True", true)]
    [InlineData("False", false)]
    public void GetBooleanValue_ValidBool_ReturnsValue(string input, bool expected)
    {
        SetTestVariable(input);
        var result = EnvReader.GetBooleanValue(TestKey);
        result.Should().Be(expected);
    }

    [Fact]
    public void GetBooleanValue_InvalidBool_ThrowsException()
    {
        SetTestVariable("Invalid");
        Action act = () => EnvReader.GetBooleanValue(TestKey);
        act.Should().Throw<Exception>().WithMessage("Value could not be retrieved.");
    }

    [Fact]
    public void TryGetStringValue_KeyExists_ReturnsTrueAndValue()
    {
        SetTestVariable("ValidValue");
        var success = EnvReader.TryGetStringValue(TestKey, out var value);
        success.Should().BeTrue();
        value.Should().Be("ValidValue");
    }

    [Fact]
    public void TryGetStringValue_KeyMissing_ReturnsFalseAndNull()
    {
        ClearTestVariable();
        var success = EnvReader.TryGetStringValue(TestKey, out var value);
        success.Should().BeFalse();
        value.Should().BeNull();
    }

    [Fact]
    public void TryGetIntValue_ValidInteger_ReturnsTrueAndValue()
    {
        SetTestVariable("42");
        var success = EnvReader.TryGetIntValue(TestKey, out var value);
        success.Should().BeTrue();
        value.Should().Be(42);
    }

    [Fact]
    public void TryGetIntValue_InvalidInteger_ReturnsFalseAndDefault()
    {
        SetTestVariable("Invalid");
        var success = EnvReader.TryGetIntValue(TestKey, out var value);
        success.Should().BeFalse();
        value.Should().Be(0);
    }

    [Fact]
    public void HasValue_KeyExists_ReturnsTrue()
    {
        SetTestVariable("AnyValue");
        var result = EnvReader.HasValue(TestKey);
        result.Should().BeTrue();
    }

    [Fact]
    public void HasValue_KeyMissing_ReturnsFalse()
    {
        ClearTestVariable();
        var result = EnvReader.HasValue(TestKey);
        result.Should().BeFalse();
    }

    [Fact]
    public void HasValue_KeyExistsEmptyValue_ReturnsFalse()
    {
        SetTestVariable("");
        var result = EnvReader.HasValue(TestKey);
        result.Should().BeFalse();
    }

    private void SetTestVariable(string value) => Environment.SetEnvironmentVariable(TestKey, value);

    private void ClearTestVariable() => Environment.SetEnvironmentVariable(TestKey, null);
}
