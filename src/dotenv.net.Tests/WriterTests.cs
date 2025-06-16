using System;
using System.Collections.Generic;
using FluentAssertions;
using Xunit;

namespace dotenv.net.Tests;

public class WriterTests
{
    private const string TestVariableKey = "TEST_VARIABLE";

    public WriterTests() { Environment.SetEnvironmentVariable(TestVariableKey, null); }

    [Fact]
    public void WriteToEnv_WhenOverwriteIsFalseAndVariableDoesNotExist_ShouldSetVariable()
    {
        var envVars = new Dictionary<string, string> { { TestVariableKey, "test_value" } };

        Writer.WriteToEnv(envVars, overwriteExistingVars: false);

        Environment.GetEnvironmentVariable(TestVariableKey).Should().Be("test_value");
    }

    [Fact]
    public void WriteToEnv_WhenOverwriteIsFalseAndVariableExists_ShouldNotChangeVariable()
    {
        const string existingValue = "existing_value";
        Environment.SetEnvironmentVariable(TestVariableKey, existingValue);
        var envVars = new Dictionary<string, string> { { TestVariableKey, "new_value" } };

        Writer.WriteToEnv(envVars, overwriteExistingVars: false);

        Environment.GetEnvironmentVariable(TestVariableKey).Should().Be(existingValue);
    }

    [Fact]
    public void WriteToEnv_WhenOverwriteIsTrueAndVariableExists_ShouldChangeVariable()
    {
        const string existingValue = "existing_value";
        const string newValue = "new_value";
        Environment.SetEnvironmentVariable(TestVariableKey, existingValue);
        var envVars = new Dictionary<string, string> { { TestVariableKey, newValue } };

        Writer.WriteToEnv(envVars, overwriteExistingVars: true);

        Environment.GetEnvironmentVariable(TestVariableKey).Should().Be(newValue);
    }
}
