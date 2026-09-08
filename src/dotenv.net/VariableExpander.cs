using System;
using System.Collections.Generic;
using System.Text;

namespace dotenv.net;

internal static class VariableExpander
{
    internal static string Expand(
        string? rawValue,
        IDictionary<string, string>? context,
        bool ignoreExceptions = true,
        ISet<string>? resolutionStack = null)
    {
        if (string.IsNullOrEmpty(rawValue))
            return string.Empty;

        var stack = resolutionStack ?? new HashSet<string>(StringComparer.Ordinal);
        return ExpandInternal(rawValue, context, stack, ignoreExceptions);
    }

    private static string ExpandInternal(
        string? rawValue,
        IDictionary<string, string>? context,
        ISet<string> resolutionStack,
        bool ignoreExceptions)
    {
        if (string.IsNullOrEmpty(rawValue))
            return string.Empty;

        var value = rawValue!;
        var sb = new StringBuilder();
        var length = value.Length;
        var i = 0;

        while (i < length)
        {
            var c = value[i];

            if (c == '\\')
            {
                var backslashStart = i;
                while (i < length && value[i] == '\\')
                {
                    i++;
                }
                var backslashCount = i - backslashStart;

                if (i < length && value[i] == '$')
                {
                    if (backslashCount % 2 != 0)
                    {
                        // Escaped $: output (backslashCount - 1) / 2 backslashes + '$'
                        sb.Append('\\', (backslashCount - 1) / 2);
                        sb.Append('$');
                        i++; // consume '$' so it remains literal
                        continue;
                    }
                    else
                    {
                        // Even number of backslashes: output backslashCount / 2 backslashes
                        sb.Append('\\', backslashCount / 2);
                        // Do not consume '$', let loop process '$' as variable token
                        continue;
                    }
                }
                else
                {
                    sb.Append('\\', backslashCount);
                    continue;
                }
            }

            if (c == '$')
            {
                if (i + 1 < length && value[i + 1] == '{')
                {
                    var braceStart = i + 2;
                    var depth = 1;
                    var j = braceStart;

                    while (j < length && depth > 0)
                    {
                        if (value[j] == '{')
                        {
                            depth++;
                        }
                        else if (value[j] == '}')
                        {
                            depth--;
                        }
                        j++;
                    }

                    if (depth == 0)
                    {
                        var innerExpression = value.Substring(braceStart, j - 1 - braceStart);
                        i = j;

                        var expanded = ResolveBracedExpression(innerExpression, context, resolutionStack, ignoreExceptions);
                        sb.Append(expanded);
                        continue;
                    }
                    else
                    {
                        // Unclosed brace, treat '$' as literal
                        sb.Append('$');
                        i++;
                        continue;
                    }
                }
                else if (i + 1 < length && IsValidIdentifierStart(value[i + 1]))
                {
                    var varStart = i + 1;
                    var j = varStart + 1;
                    while (j < length && IsValidIdentifierPart(value[j]))
                    {
                        j++;
                    }

                    var varName = value.Substring(varStart, j - varStart);
                    i = j;

                    var expanded = ResolveVariable(varName, null, null, context, resolutionStack, ignoreExceptions);
                    sb.Append(expanded);
                    continue;
                }
                else
                {
                    sb.Append('$');
                    i++;
                    continue;
                }
            }

            sb.Append(c);
            i++;
        }

        return sb.ToString();
    }

    private static string ResolveBracedExpression(
        string innerExpression,
        IDictionary<string, string>? context,
        ISet<string> resolutionStack,
        bool ignoreExceptions)
    {
        string varName;
        string? operatorType = null;
        string? defaultValue = null;

        var innerBraceDepth = 0;
        var operatorIndex = -1;

        for (var k = 0; k < innerExpression.Length; k++)
        {
            if (innerExpression[k] == '{')
            {
                innerBraceDepth++;
            }
            else if (innerExpression[k] == '}')
            {
                innerBraceDepth--;
            }
            else if (innerBraceDepth == 0)
            {
                if (innerExpression[k] == ':' && k + 1 < innerExpression.Length && innerExpression[k + 1] == '-')
                {
                    operatorIndex = k;
                    operatorType = ":-";
                    break;
                }

                if (innerExpression[k] == '-')
                {
                    operatorIndex = k;
                    operatorType = "-";
                    break;
                }
            }
        }

        if (operatorIndex >= 0)
        {
            varName = innerExpression.Substring(0, operatorIndex).Trim();
            defaultValue = innerExpression.Substring(operatorIndex + operatorType!.Length);
        }
        else
        {
            varName = innerExpression.Trim();
        }

        return ResolveVariable(varName, operatorType, defaultValue, context, resolutionStack, ignoreExceptions);
    }

    private static string ResolveVariable(
        string varName,
        string? operatorType,
        string? defaultValue,
        IDictionary<string, string>? context,
        ISet<string> resolutionStack,
        bool ignoreExceptions)
    {
        if (string.IsNullOrEmpty(varName))
        {
            if (operatorType != null && defaultValue != null)
                return ExpandInternal(defaultValue, context, resolutionStack, ignoreExceptions);

            return string.Empty;
        }

        if (resolutionStack.Contains(varName))
        {
            if (!ignoreExceptions)
            {
                var cyclePath = string.Join(" -> ", resolutionStack) + " -> " + varName;
                throw new InvalidOperationException($"Circular reference detected: {cyclePath}");
            }

            return string.Empty;
        }

        resolutionStack.Add(varName);
        try
        {
            var isSet = false;
            string? resolvedValue = null;

            if (context != null && context.TryGetValue(varName, out var contextVal))
            {
                isSet = true;
                resolvedValue = contextVal;
            }
            else
            {
                var envVal = Environment.GetEnvironmentVariable(varName);
                if (envVal != null)
                {
                    isSet = true;
                    resolvedValue = envVal;
                }
            }

            if (isSet && resolvedValue != null && resolvedValue.IndexOf('$') >= 0)
            {
                resolvedValue = ExpandInternal(resolvedValue, context, resolutionStack, ignoreExceptions);
            }

            if (operatorType == ":-")
            {
                if (!isSet || string.IsNullOrEmpty(resolvedValue))
                {
                    return defaultValue != null
                        ? ExpandInternal(defaultValue, context, resolutionStack, ignoreExceptions)
                        : string.Empty;
                }

                return resolvedValue!;
            }

            if (operatorType == "-")
            {
                if (!isSet)
                {
                    return defaultValue != null
                        ? ExpandInternal(defaultValue, context, resolutionStack, ignoreExceptions)
                        : string.Empty;
                }

                return resolvedValue ?? string.Empty;
            }

            // No default fallback operator
            return isSet ? (resolvedValue ?? string.Empty) : string.Empty;
        }
        finally
        {
            resolutionStack.Remove(varName);
        }
    }

    private static bool IsValidIdentifierStart(char c) =>
        (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || c == '_';

    private static bool IsValidIdentifierPart(char c) =>
        (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || (c >= '0' && c <= '9') || c == '_';
}
