using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace dotenv.net;

internal static class EnvFileCascade
{
    internal const string AspNetCoreEnvironmentVariable = "ASPNETCORE_ENVIRONMENT";
    internal const string DotNetEnvironmentVariable = "DOTNET_ENVIRONMENT";
    internal const string DotEnvEnvironmentVariable = "DOTENV_ENV";

    internal static string? ResolveEnvironmentName(string? explicitName)
    {
        if (!string.IsNullOrWhiteSpace(explicitName))
            return explicitName!.Trim();

        return FirstNonEmpty(
            Environment.GetEnvironmentVariable(AspNetCoreEnvironmentVariable),
            Environment.GetEnvironmentVariable(DotNetEnvironmentVariable),
            Environment.GetEnvironmentVariable(DotEnvEnvironmentVariable));
    }

    internal static void EnsureEnvironmentNameIsSafe(string environmentName)
    {
        if (string.IsNullOrWhiteSpace(environmentName))
            throw new ArgumentException("The environment name cannot be null, empty or whitespace.",
                nameof(environmentName));

        if (environmentName.IndexOf("..", StringComparison.Ordinal) >= 0 ||
            environmentName.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0 ||
            environmentName.IndexOfAny(['/', '\\']) >= 0)
            throw new ArgumentException(
                $"The environment name \"{environmentName}\" is invalid because it contains path separator or relative path characters.",
                nameof(environmentName));
    }

    internal static IReadOnlyList<string> GetCandidateFileNames(string? environmentName)
    {
        var names = new List<string>
        {
            DotEnvOptions.DefaultEnvFileName,
            $"{DotEnvOptions.DefaultEnvFileName}.local"
        };

        if (string.IsNullOrWhiteSpace(environmentName))
            return names;

        var resolvedName = environmentName!;
        EnsureEnvironmentNameIsSafe(resolvedName);
        names.Add($"{DotEnvOptions.DefaultEnvFileName}.{resolvedName}");
        names.Add($"{DotEnvOptions.DefaultEnvFileName}.{resolvedName}.local");
        return names;
    }

    internal static IReadOnlyList<string> ResolveExistingFilePaths(string directory, string? environmentName,
        bool ignoreExceptions)
    {
        if (string.IsNullOrWhiteSpace(directory))
            directory = ".";

        var candidates = GetCandidateFileNames(environmentName)
            .Select(name => Path.Combine(directory, name))
            .ToArray();

        var existing = candidates.Where(File.Exists).ToArray();
        if (existing.Length == 0 && !ignoreExceptions)
            throw new FileNotFoundException(
                $"Could not find any environment files among:{Environment.NewLine}{string.Join(Environment.NewLine, candidates)}");

        return existing;
    }

    private static string? FirstNonEmpty(params string?[] values)
    {
        foreach (var value in values)
        {
            if (!string.IsNullOrWhiteSpace(value))
                return value!.Trim();
        }

        return null;
    }
}
