using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dotenv.net;

/// <summary>
/// Represents the configuration options for reading and loading .env files.
/// </summary>
public class DotEnvOptions
{
    private static readonly string[] DefaultEnvPath = [DefaultEnvFileName];
    internal const string DefaultEnvFileName = ".env";
    internal const int DefaultProbeAscendLimit = 4;

    /// <summary>
    /// A value to state whether to throw or swallow exceptions. The default is true. <see cref="T:dotenv.net.DotEnvOptions"/>
    /// </summary>
    public bool IgnoreExceptions { get; private set; }

    /// <summary>
    /// The paths to the env files. The default is [.env]. <see cref="T:dotenv.net.DotEnvOptions"/>
    /// </summary>
    public IEnumerable<string> EnvFilePaths { get; private set; }

    /// <summary>
    /// The encoding that the env file was created with. The default is UTF-8. <see cref="T:dotenv.net.DotEnvOptions"/>
    /// </summary>
    public Encoding Encoding { get; private set; }

    /// <summary>
    /// A value to state whether to trim whitespace from the values retrieved. The default is false. <see cref="T:dotenv.net.DotEnvOptions"/>
    /// </summary>
    public bool TrimValues { get; private set; }

    /// <summary>
    /// Whether to overwrite existing environment variables. Also applies to multiple env files; if false and other env
    /// files have the same key, the values are ignored. Defaults to true. <see cref="T:dotenv.net.DotEnvOptions"/>
    /// </summary>
    public bool OverwriteExistingVars { get; private set; }

    /// <summary>
    /// A value to state whether we traverse up the directory structure. The default is false. <see cref="T:dotenv.net.DotEnvOptions"/>
    /// </summary>
    public bool ProbeForEnv { get; private set; }

    /// <summary>
    /// A value to state how far up the directory structure we should search for env files. <see cref="T:dotenv.net.DotEnvOptions"/>
    /// </summary>
    public int? ProbeLevelsToSearch { get; private set; }

    /// <summary>
    /// Whether the optional dotenv export syntax should be supported. The default is false.
    /// </summary>
    public bool SupportExportSyntax { get; private set; }

    /// <summary>
    /// Initialises a new instance of the <see cref="DotEnvOptions"/> class.
    /// </summary>
    /// <param name="ignoreExceptions">Whether to ignore exceptions during the loading process.</param>
    /// <param name="envFilePaths">The paths to the env files to load.</param>
    /// <param name="encoding">The encoding the env files are in.</param>
    /// <param name="trimValues">Whether to trim whitespace from the read values.</param>
    /// <param name="overwriteExistingVars">Whether to overwrite a given env var if it is already set.</param>
    /// <param name="probeForEnv">Whether to search up the directories looking for an env file.</param>
    /// <param name="probeLevelsToSearch">How high up the directory chain to search.</param>
    /// <param name="supportExportSyntax">Whether to support env vars in the export syntax.</param>
    public DotEnvOptions(bool ignoreExceptions = true, IEnumerable<string>? envFilePaths = null,
        Encoding? encoding = null, bool trimValues = false, bool overwriteExistingVars = true,
        bool probeForEnv = false, int? probeLevelsToSearch = null, bool supportExportSyntax = false)
    {
        if (ignoreExceptions)
            WithoutExceptions();
        else
            WithoutOverwriteExistingVars();

        WithEnvFiles((envFilePaths ?? []).ToArray());
        WithEncoding(encoding ?? Encoding.UTF8);

        if (trimValues)
            WithTrimValues();
        else
            WithoutTrimValues();

        if (overwriteExistingVars)
            WithOverwriteExistingVars();
        else
            WithoutOverwriteExistingVars();

        if (probeForEnv)
            WithProbeForEnv(probeLevelsToSearch ?? DefaultProbeAscendLimit);
        else
            WithoutProbeForEnv();

        if (supportExportSyntax)
            WithSupportExportSyntax();
        else
            WithoutSupportExportSyntax();
    }

    /// <summary>
    /// Enables exception throwing when errors occur.
    /// </summary>
    /// <returns>The current <see cref="DotEnvOptions"/> instance.</returns>
    public DotEnvOptions WithExceptions()
    {
        IgnoreExceptions = false;
        return this;
    }

    /// <summary>
    /// Disables exception throwing when errors occur.
    /// </summary>
    /// <returns>The current <see cref="DotEnvOptions"/> instance.</returns>
    public DotEnvOptions WithoutExceptions()
    {
        IgnoreExceptions = true;
        return this;
    }

    /// <summary>
    /// Enables searching up the directory tree for a .env file.
    /// </summary>
    /// <param name="probeLevelsToSearch">How high up the directory chain to search.</param>
    /// <returns>The current <see cref="DotEnvOptions"/> instance.</returns>
    /// <exception cref="InvalidOperationException">Thrown when EnvFiles is already set.</exception>
    public DotEnvOptions WithProbeForEnv(int probeLevelsToSearch = DefaultProbeAscendLimit)
    {
        if (EnvFilePaths?.FirstOrDefault() != DefaultEnvFileName)
            throw new InvalidOperationException("Cannot use ProbeForEnv when EnvFiles is set.");

        ProbeForEnv = true;
        ProbeLevelsToSearch = probeLevelsToSearch < 0 ? DefaultProbeAscendLimit : probeLevelsToSearch;
        return this;
    }

    /// <summary>
    /// Disables searching up the directory tree for a .env file.
    /// </summary>
    /// <returns>The current <see cref="DotEnvOptions"/> instance.</returns>
    public DotEnvOptions WithoutProbeForEnv()
    {
        ProbeForEnv = false;
        ProbeLevelsToSearch = DefaultProbeAscendLimit;
        return this;
    }

    /// <summary>
    /// Enables overwriting existing environment variables.
    /// </summary>
    /// <returns>The current <see cref="DotEnvOptions"/> instance.</returns>
    public DotEnvOptions WithOverwriteExistingVars()
    {
        OverwriteExistingVars = true;
        return this;
    }

    /// <summary>
    /// Disables overwriting existing environment variables.
    /// </summary>
    /// <returns>The current <see cref="DotEnvOptions"/> instance.</returns>
    public DotEnvOptions WithoutOverwriteExistingVars()
    {
        OverwriteExistingVars = false;
        return this;
    }

    /// <summary>
    /// Enables trimming of whitespace from retrieved values.
    /// </summary>
    /// <returns>The current <see cref="DotEnvOptions"/> instance.</returns>
    public DotEnvOptions WithTrimValues()
    {
        TrimValues = true;
        return this;
    }

    /// <summary>
    /// Disables trimming of whitespace from retrieved values.
    /// </summary>
    /// <returns>The current <see cref="DotEnvOptions"/> instance.</returns>
    public DotEnvOptions WithoutTrimValues()
    {
        TrimValues = false;
        return this;
    }

    /// <summary>
    /// Sets the encoding to be used when reading the env files.
    /// </summary>
    /// <param name="encoding">The encoding to use.</param>
    /// <returns>The current <see cref="DotEnvOptions"/> instance.</returns>
    /// <exception cref="ArgumentNullException">Thrown when encoding is null.</exception>
    public DotEnvOptions WithEncoding(Encoding encoding)
    {
        if (encoding == null)
            throw new ArgumentNullException(nameof(encoding), "Encoding cannot be null");

        Encoding = encoding;
        return this;
    }

    /// <summary>
    /// Enables support for the 'export' syntax in env files.
    /// </summary>
    /// <returns>The current <see cref="DotEnvOptions"/> instance.</returns>
    public DotEnvOptions WithSupportExportSyntax()
    {
        SupportExportSyntax = true;
        return this;
    }

    /// <summary>
    /// Disables support for the 'export' syntax in env files.
    /// </summary>
    /// <returns>The current <see cref="DotEnvOptions"/> instance.</returns>
    public DotEnvOptions WithoutSupportExportSyntax()
    {
        SupportExportSyntax = false;
        return this;
    }

    /// <summary>
    /// Sets the env files to be read.
    /// </summary>
    /// <param name="envFilePaths">The paths to the env files.</param>
    /// <returns>The current <see cref="DotEnvOptions"/> instance.</returns>
    /// <exception cref="InvalidOperationException">Thrown when ProbeForEnv is already set to true.</exception>
    /// <exception cref="ArgumentNullException">Thrown when envFilePaths is null.</exception>
    public DotEnvOptions WithEnvFiles(params string[] envFilePaths)
    {
        if (ProbeForEnv)
            throw new InvalidOperationException("EnvFiles paths cannot be set when ProbeForEnv is true");

        if (envFilePaths == null)
            throw new ArgumentNullException(nameof(envFilePaths), "EnvFilePaths cannot be null");

        EnvFilePaths = envFilePaths.Any() != true ? DefaultEnvPath : envFilePaths;
        return this;
    }

    /// <summary>
    /// Reads the env files and returns the values without writing to the system environment.
    /// </summary>
    /// <returns>A dictionary containing the read environment variables.</returns>
    public IDictionary<string, string> Read() => DotEnv.Read(this);

    /// <summary>
    /// Reads the env files and writes the values to the system environment variables.
    /// </summary>
    public void Load() => DotEnv.Load(this);
}