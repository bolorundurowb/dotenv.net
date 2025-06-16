using System;
using dotenv.net.Utilities;
using Shouldly;
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
        result.ShouldBe("ValidValue");
    }

    [Fact]
    public void GetStringValue_KeyMissing_ThrowsException()
    {
        ClearTestVariable();
        Assert.Throws<Exception>(() => EnvReader.GetStringValue(TestKey));
    }

    [Fact]
    public void GetIntValue_ValidInteger_ReturnsValue()
    {
        SetTestVariable("42");
        var result = EnvReader.GetIntValue(TestKey);
        result.ShouldBe(42);
    }

    [Fact]
    public void GetIntValue_InvalidInteger_ThrowsException()
    {
        SetTestVariable("Invalid");
        Assert.Throws<Exception>(() => EnvReader.GetIntValue(TestKey));
    }

    [Fact]
    public void GetDoubleValue_ValidDouble_ReturnsValue()
    {
        SetTestVariable("3.14");
        var result = EnvReader.GetDoubleValue(TestKey);
        result.ShouldBe(3.14);
    }

    [Fact]
    public void GetDoubleValue_InvalidDouble_ThrowsException()
    {
        SetTestVariable("Invalid");
        Assert.Throws<Exception>(() => EnvReader.GetDoubleValue(TestKey));
    }

    [Fact]
    public void GetDecimalValue_ValidDecimal_ReturnsValue()
    {
        SetTestVariable("99.99");
        var result = EnvReader.GetDecimalValue(TestKey);
        result.ShouldBe(99.99m);
    }

    [Fact]
    public void GetDecimalValue_InvalidDecimal_ThrowsException()
    {
        SetTestVariable("Invalid");
        Assert.Throws<Exception>(() => EnvReader.GetDecimalValue(TestKey));
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
        result.ShouldBe(expected);
    }

    [Fact]
    public void GetBooleanValue_InvalidBool_ThrowsException()
    {
        SetTestVariable("Invalid");
        Assert.Throws<Exception>(() => EnvReader.GetBooleanValue(TestKey));
    }

    [Fact]
    public void TryGetStringValue_KeyExists_ReturnsTrueAndValue()
    {
        SetTestVariable("ValidValue");
        var success = EnvReader.TryGetStringValue(TestKey, out var value);
        success.ShouldBeTrue();
        value.ShouldBe("ValidValue");
    }

    [Fact]
    public void TryGetStringValue_KeyMissing_ReturnsFalseAndNull()
    {
        ClearTestVariable();
        var success = EnvReader.TryGetStringValue(TestKey, out var value);
        success.ShouldBeFalse();
        value.ShouldBeNull();
    }

    [Fact]
    public void TryGetIntValue_ValidInteger_ReturnsTrueAndValue()
    {
        SetTestVariable("42");
        var success = EnvReader.TryGetIntValue(TestKey, out var value);
        success.ShouldBeTrue();
        value.ShouldBe(42);
    }

    [Fact]
    public void TryGetIntValue_InvalidInteger_ReturnsFalseAndDefault()
    {
        SetTestVariable("Invalid");
        var success = EnvReader.TryGetIntValue(TestKey, out var value);
        success.ShouldBeFalse();
        value.ShouldBe(0);
    }

    [Fact]
    public void HasValue_KeyExists_ReturnsTrue()
    {
        SetTestVariable("AnyValue");
        var result = EnvReader.HasValue(TestKey);
        result.ShouldBeTrue();
    }

    [Fact]
    public void HasValue_KeyMissing_ReturnsFalse()
    {
        ClearTestVariable();
        var result = EnvReader.HasValue(TestKey);
        result.ShouldBeFalse();
    }

    [Fact]
    public void HasValue_KeyExistsEmptyValue_ReturnsFalse()
    {
        SetTestVariable("");
        var result = EnvReader.HasValue(TestKey);
        result.ShouldBeFalse();
    }

    private static void SetTestVariable(string value) => Environment.SetEnvironmentVariable(TestKey, value);

    private static void ClearTestVariable() => Environment.SetEnvironmentVariable(TestKey, null);
}