using System;
using System.Collections.Generic;
using Shouldly;
using Xunit;

namespace dotenv.net.Tests;

public class VariableExpanderTests
{
    [Fact]
    public void Expand_NullOrEmptyValue_ShouldReturnEmptyOrSame()
    {
        VariableExpander.Expand(null, null).ShouldBe(string.Empty);
        VariableExpander.Expand(string.Empty, null).ShouldBe(string.Empty);
    }

    [Fact]
    public void Expand_NoVariables_ShouldReturnOriginalValue()
    {
        var raw = "literal_value_without_dollar";
        VariableExpander.Expand(raw, null).ShouldBe(raw);
    }

    [Fact]
    public void Expand_BracedVariable_ShouldResolveFromContext()
    {
        var context = new Dictionary<string, string>
        {
            ["BASE_URL"] = "https://api.example.com"
        };
        var raw = "${BASE_URL}/v1";
        VariableExpander.Expand(raw, context).ShouldBe("https://api.example.com/v1");
    }

    [Fact]
    public void Expand_ShortVariable_ShouldResolveFromContext()
    {
        var context = new Dictionary<string, string>
        {
            ["APP_NAME"] = "myapp"
        };
        var raw = "/opt/$APP_NAME/bin";
        VariableExpander.Expand(raw, context).ShouldBe("/opt/myapp/bin");
    }

    [Fact]
    public void Expand_MultipleVariables_ShouldResolveAll()
    {
        var context = new Dictionary<string, string>
        {
            ["USER"] = "postgres",
            ["PASSWORD"] = "secret",
            ["PORT"] = "5432"
        };
        var raw = "postgres://${USER}:${PASSWORD}@localhost:${PORT}/db";
        VariableExpander.Expand(raw, context).ShouldBe("postgres://postgres:secret@localhost:5432/db");
    }

    [Fact]
    public void Expand_ResolvesFromEnvironment_WhenNotInContext()
    {
        var envVarName = "DOTENV_TEST_VAR_" + Guid.NewGuid().ToString("N");
        try
        {
            Environment.SetEnvironmentVariable(envVarName, "from_env");
            var raw = $"val_${{{envVarName}}}";
            VariableExpander.Expand(raw, new Dictionary<string, string>()).ShouldBe("val_from_env");
        }
        finally
        {
            Environment.SetEnvironmentVariable(envVarName, null);
        }
    }

    [Fact]
    public void Expand_PrefersContextOverEnvironment()
    {
        var envVarName = "DOTENV_TEST_VAR_" + Guid.NewGuid().ToString("N");
        try
        {
            Environment.SetEnvironmentVariable(envVarName, "from_env");
            var context = new Dictionary<string, string>
            {
                [envVarName] = "from_context"
            };
            var raw = $"${{{envVarName}}}";
            VariableExpander.Expand(raw, context).ShouldBe("from_context");
        }
        finally
        {
            Environment.SetEnvironmentVariable(envVarName, null);
        }
    }

    [Fact]
    public void Expand_MissingVariable_WithoutDefault_ShouldResolveToEmpty()
    {
        var raw = "before_${MISSING_VAR}_after";
        VariableExpander.Expand(raw, new Dictionary<string, string>()).ShouldBe("before__after");
    }

    [Fact]
    public void Expand_MissingVariableShort_WithoutDefault_ShouldResolveToEmpty()
    {
        var raw = "before_$MISSING_VAR/after";
        VariableExpander.Expand(raw, new Dictionary<string, string>()).ShouldBe("before_/after");
    }

    [Fact]
    public void Expand_ColonDashFallback_WhenMissing_ShouldResolveToDefault()
    {
        var raw = "${MISSING_VAR:-default_value}";
        VariableExpander.Expand(raw, new Dictionary<string, string>()).ShouldBe("default_value");
    }

    [Fact]
    public void Expand_ColonDashFallback_WhenEmpty_ShouldResolveToDefault()
    {
        var context = new Dictionary<string, string> { ["EMPTY_VAR"] = "" };
        var raw = "${EMPTY_VAR:-default_value}";
        VariableExpander.Expand(raw, context).ShouldBe("default_value");
    }

    [Fact]
    public void Expand_ColonDashFallback_WhenSetNonEmpty_ShouldResolveToValue()
    {
        var context = new Dictionary<string, string> { ["SET_VAR"] = "custom" };
        var raw = "${SET_VAR:-default_value}";
        VariableExpander.Expand(raw, context).ShouldBe("custom");
    }

    [Fact]
    public void Expand_DashFallback_WhenMissing_ShouldResolveToDefault()
    {
        var raw = "${MISSING_VAR-default_value}";
        VariableExpander.Expand(raw, new Dictionary<string, string>()).ShouldBe("default_value");
    }

    [Fact]
    public void Expand_DashFallback_WhenEmpty_ShouldPreserveEmpty()
    {
        var context = new Dictionary<string, string> { ["EMPTY_VAR"] = "" };
        var raw = "${EMPTY_VAR-default_value}";
        VariableExpander.Expand(raw, context).ShouldBe(string.Empty);
    }

    [Fact]
    public void Expand_DashFallback_WhenSetNonEmpty_ShouldResolveToValue()
    {
        var context = new Dictionary<string, string> { ["SET_VAR"] = "custom" };
        var raw = "${SET_VAR-default_value}";
        VariableExpander.Expand(raw, context).ShouldBe("custom");
    }

    [Fact]
    public void Expand_NestedFallbacks_ShouldCascadeCorrectly()
    {
        var raw = "${CUSTOM_URL:-${DEFAULT_HOST:-localhost}:3000}";
        VariableExpander.Expand(raw, new Dictionary<string, string>()).ShouldBe("localhost:3000");

        var context = new Dictionary<string, string> { ["DEFAULT_HOST"] = "127.0.0.1" };
        VariableExpander.Expand(raw, context).ShouldBe("127.0.0.1:3000");

        context["CUSTOM_URL"] = "remote.host:8080";
        VariableExpander.Expand(raw, context).ShouldBe("remote.host:8080");
    }

    [Fact]
    public void Expand_DeeplyNestedFallbacks_ShouldResolveFinalDefault()
    {
        var raw = "${A:-${B:-${C:-final}}}";
        VariableExpander.Expand(raw, new Dictionary<string, string>()).ShouldBe("final");
    }

    [Fact]
    public void Expand_EscapedBracedVariable_ShouldOutputLiteral()
    {
        var raw = @"\${VAR}";
        VariableExpander.Expand(raw, new Dictionary<string, string> { ["VAR"] = "val" }).ShouldBe("${VAR}");
    }

    [Fact]
    public void Expand_EscapedShortVariable_ShouldOutputLiteral()
    {
        var raw = @"\$VAR";
        VariableExpander.Expand(raw, new Dictionary<string, string> { ["VAR"] = "val" }).ShouldBe("$VAR");
    }

    [Fact]
    public void Expand_DoubleBackslashBeforeBracedVariable_ShouldUnescapeBackslashAndExpand()
    {
        var raw = @"\\${VAR}";
        VariableExpander.Expand(raw, new Dictionary<string, string> { ["VAR"] = "val" }).ShouldBe(@"\val");
    }

    [Fact]
    public void Expand_TripleBackslashBeforeBracedVariable_ShouldOutputSingleBackslashAndLiteral()
    {
        var raw = @"\\\" + "${VAR}";
        VariableExpander.Expand(raw, new Dictionary<string, string> { ["VAR"] = "val" }).ShouldBe(@"\${VAR}");
    }

    [Fact]
    public void Expand_DollarsWithoutValidIdentifier_ShouldPreserveLiteral()
    {
        VariableExpander.Expand("$100", null).ShouldBe("$100");
        VariableExpander.Expand("price=$", null).ShouldBe("price=$");
        VariableExpander.Expand("$#special", null).ShouldBe("$#special");
        VariableExpander.Expand("$%percent", null).ShouldBe("$%percent");
    }

    [Fact]
    public void Expand_DirectCycle_ShouldThrow_WhenIgnoreExceptionsFalse()
    {
        var context = new Dictionary<string, string>
        {
            ["A"] = "${A}"
        };

        var act = () => VariableExpander.Expand("${A}", context, ignoreExceptions: false);
        var ex = act.ShouldThrow<InvalidOperationException>();
        ex.Message.ShouldContain("Circular reference detected");
        ex.Message.ShouldContain("A");
    }

    [Fact]
    public void Expand_DirectCycle_ShouldReturnEmpty_WhenIgnoreExceptionsTrue()
    {
        var context = new Dictionary<string, string>
        {
            ["A"] = "${A}"
        };

        var result = VariableExpander.Expand("${A}", context, ignoreExceptions: true);
        result.ShouldBe(string.Empty);
    }

    [Fact]
    public void Expand_IndirectCycle_ShouldThrow_WhenIgnoreExceptionsFalse()
    {
        var context = new Dictionary<string, string>
        {
            ["A"] = "${B}",
            ["B"] = "${C}",
            ["C"] = "${A}"
        };

        var act = () => VariableExpander.Expand("${A}", context, ignoreExceptions: false);
        var ex = act.ShouldThrow<InvalidOperationException>();
        ex.Message.ShouldContain("Circular reference detected");
        ex.Message.ShouldContain("A");
        ex.Message.ShouldContain("B");
        ex.Message.ShouldContain("C");
    }

    [Fact]
    public void Expand_IndirectCycle_ShouldReturnEmpty_WhenIgnoreExceptionsTrue()
    {
        var context = new Dictionary<string, string>
        {
            ["A"] = "${B}",
            ["B"] = "${C}",
            ["C"] = "${A}"
        };

        var result = VariableExpander.Expand("${A}", context, ignoreExceptions: true);
        result.ShouldBe(string.Empty);
    }

    [Fact]
    public void Expand_SelfReferencingDefault_ShouldDetectCycle()
    {
        var act = () => VariableExpander.Expand("${A:-${A}}", new Dictionary<string, string>(), ignoreExceptions: false);
        var ex = act.ShouldThrow<InvalidOperationException>();
        ex.Message.ShouldContain("Circular reference detected");
    }

    [Fact]
    public void Expand_UnclosedBrace_ShouldPreserveLiteral()
    {
        VariableExpander.Expand("${UNCLOSED", null).ShouldBe("${UNCLOSED");
    }

    [Fact]
    public void Expand_BackslashNotFollowedByDollar_ShouldPreserve()
    {
        VariableExpander.Expand(@"C:\dir\file", null).ShouldBe(@"C:\dir\file");
        VariableExpander.Expand(@"\\server\share", null).ShouldBe(@"\\server\share");
        VariableExpander.Expand(@"trailing\", null).ShouldBe(@"trailing\");
    }

    [Fact]
    public void Expand_EmptyVarNameWithColonDashDefault_ShouldResolveDefault()
    {
        VariableExpander.Expand("${:-fallback}", new Dictionary<string, string>()).ShouldBe("fallback");
    }

    [Fact]
    public void Expand_EmptyVarNameWithDashDefault_ShouldResolveDefault()
    {
        VariableExpander.Expand("${-fallback}", new Dictionary<string, string>()).ShouldBe("fallback");
    }

    [Fact]
    public void Expand_EmptyVarNameWithoutOperator_ShouldResolveEmpty()
    {
        VariableExpander.Expand("${}", new Dictionary<string, string>()).ShouldBe(string.Empty);
    }

    [Fact]
    public void Expand_EmptyDefault_ShouldResolveEmpty()
    {
        VariableExpander.Expand("${MISSING:-}", new Dictionary<string, string>()).ShouldBe(string.Empty);
    }

    [Fact]
    public void Expand_NestedBracesInVariableName_ShouldResolveEmpty()
    {
        VariableExpander.Expand("${${A}}", new Dictionary<string, string> { ["A"] = "x" }).ShouldBe(string.Empty);
    }

    [Fact]
    public void Expand_ResolvesFromEnvironment_WhenContextIsNull()
    {
        var envVarName = "DOTENV_TEST_VAR_" + Guid.NewGuid().ToString("N");
        try
        {
            Environment.SetEnvironmentVariable(envVarName, "from_env");
            VariableExpander.Expand($"${{{envVarName}}}", null).ShouldBe("from_env");
        }
        finally
        {
            Environment.SetEnvironmentVariable(envVarName, null);
        }
    }

    [Fact]
    public void Expand_UnderscoreAndDigitIdentifiers_ShouldResolve()
    {
        var context = new Dictionary<string, string>
        {
            ["_private"] = "secret",
            ["VAR_1"] = "one",
            ["lower"] = "low"
        };
        VariableExpander.Expand("$_private/$VAR_1/$lower", context).ShouldBe("secret/one/low");
    }

    [Fact]
    public void Expand_IndirectReference_ShouldResolveRecursively()
    {
        var context = new Dictionary<string, string>
        {
            ["A"] = "${B}",
            ["B"] = "final"
        };
        VariableExpander.Expand("${A}", context).ShouldBe("final");
    }
}
