using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dotenv.net;

public class DotEnvOptions
{
    private static readonly string[] DefaultEnvPath = { DefaultEnvFileName };
    internal const string DefaultEnvFileName = ".env";
    internal const int DefaultProbeDepth = 4;

    /// <summary>
    /// A value to state whether to throw or swallow exceptions. The default is true. <see cref="T:dotenv.net.DotEnvOptions"/>
    /// </summary>
    public bool IgnoreExceptions { get; private set; }

    /// <summary>
    /// The paths to the env files. The default is [.env] <see cref="T:dotenv.net.DotEnvOptions"/>
    /// </summary>
    public IEnumerable<string> EnvFilePaths { get; private set; }

    /// <summary>
    /// The Encoding that the env file was created with. The default is UTF-8. <see cref="T:dotenv.net.DotEnvOptions"/>
    /// </summary>
    public Encoding Encoding { get; private set; }

    /// <summary>
    /// A value to state whether to trim whitespace from the values retrieved. The default is false. <see cref="T:dotenv.net.DotEnvOptions"/>
    /// </summary>
    public bool TrimValues { get; private set; }

    /// <summary>
    /// A value to state whether to override the env variable if it has been set. the default is true. <see cref="T:dotenv.net.DotEnvOptions"/>
    /// </summary>
    public bool OverwriteExistingVars { get; private set; }

    /// <summary>
    /// A value to state whether we traverse up the directory structure. The default is false. <see cref="T:dotenv.net.DotEnvOptions"/>
    /// </summary>
    public bool ProbeForEnv { get; private set; }

    /// <summary>
    /// A value to state how far up the directory structure we should search for env files. <see cref="T:dotenv.net.DotEnvOptions"/>
    /// </summary>
    public int ProbeLevelsToSearch { get; private set; }

    /// <summary>
    /// Default constructor for the dot env options
    /// </summary>
    /// <param name="ignoreExceptions">Whether or not to ignore exceptions</param>
    /// <param name="encoding">The encoding the env files are in</param>
    /// <param name="trimValues">Whether or not to trim whitespace from the read values</param>
    /// <param name="overwriteExistingVars">Whether or not to overwrite </param>
    /// <param name="probeForEnv">Whether or not to search up the directories looking for an env file</param>
    /// <param name="probeLevelsToSearch">How high up the directory chain to search</param>
    /// <param name="envFilePaths">The env file paths to load</param>
    public DotEnvOptions(bool ignoreExceptions = true, IEnumerable<string> envFilePaths = null,
        Encoding encoding = null, bool trimValues = false, bool overwriteExistingVars = true,
        bool probeForEnv = false, int probeLevelsToSearch = DefaultProbeDepth)
    {
        IgnoreExceptions = ignoreExceptions;
        EnvFilePaths = envFilePaths?.Any() != true ? DefaultEnvPath : envFilePaths;
        Encoding = encoding ?? Encoding.UTF8;
        TrimValues = trimValues;
        OverwriteExistingVars = overwriteExistingVars;
        ProbeForEnv = probeForEnv;
        ProbeLevelsToSearch = probeLevelsToSearch;
    }

    /// <summary>
    /// Ignore exceptions thrown
    /// </summary>
    /// <returns>configured dot env options</returns>
    public DotEnvOptions WithExceptions()
    {
        IgnoreExceptions = false;
        return this;
    }

    /// <summary>
    /// Throw exceptions when triggered
    /// </summary>
    /// <returns>configured dot env options</returns>
    public DotEnvOptions WithoutExceptions()
    {
        IgnoreExceptions = true;
        return this;
    }

    /// <summary>
    /// Search up the directory for a .env file. By default searches up 4 directories.
    /// </summary>
    /// <returns>configured dot env options</returns>
    public DotEnvOptions WithProbeForEnv(int probeLevelsToSearch = DefaultProbeDepth)
    {
        ProbeForEnv = true;
        ProbeLevelsToSearch = probeLevelsToSearch;
        return this;
    }

    /// <summary>
    /// Rely on the provided env files. By default is false.
    /// </summary>
    /// <returns>configured dot env options</returns>
    public DotEnvOptions WithoutProbeForEnv()
    {
        ProbeForEnv = false;
        ProbeLevelsToSearch = DefaultProbeDepth;
        return this;
    }

    /// <summary>
    /// Overwrite an environment variable even if it has been set
    /// </summary>
    /// <returns>configured dot env options</returns>
    public DotEnvOptions WithOverwriteExistingVars()
    {
        OverwriteExistingVars = true;
        return this;
    }

    /// <summary>
    /// Only write an environment variable if it hasnt been et
    /// </summary>
    /// <returns>configured dot env options</returns>
    public DotEnvOptions WithoutOverwriteExistingVars()
    {
        OverwriteExistingVars = false;
        return this;
    }

    /// <summary>
    /// Trim whitespace from the values read
    /// </summary>
    /// <returns>configured dot env options</returns>
    public DotEnvOptions WithTrimValues()
    {
        TrimValues = true;
        return this;
    }

    /// <summary>
    /// Leave read values as is
    /// </summary>
    /// <returns>configured dot env options</returns>
    public DotEnvOptions WithoutTrimValues()
    {
        TrimValues = false;
        return this;
    }

    /// <summary>
    /// Change the encoding for reading the env files
    /// </summary>
    /// <returns>configured dot env options</returns>
    public DotEnvOptions WithEncoding(Encoding encoding)
    {
        Encoding = encoding ?? Encoding.UTF8;
        return this;
    }

    /// <summary>
    /// Revert to the default encoding for reading the env files. The default encoding is UTF-8
    /// </summary>
    /// <returns>configured dot env options</returns>
    public DotEnvOptions WithDefaultEncoding()
    {
        Encoding = Encoding.UTF8;
        return this;
    }

    /// <summary>
    /// Set the env files to be read, if none is provided, we revert to the default '.env'
    /// </summary>
    /// <returns>configured dot env options</returns>
    public DotEnvOptions WithEnvFiles(params string[] envFilePaths)
    {
        EnvFilePaths = envFilePaths?.Any() != true ? DefaultEnvPath : envFilePaths;
        return this;
    }

    /// <summary>
    /// Return the values in the env files without writing to the environment
    /// </summary>
    /// <returns>configured dot env options</returns>
    public IDictionary<string, string> Read() => DotEnv.Read(this);

    /// <summary>
    /// Read the env files and write to the syetm environment variables
    /// </summary>
    public void Load() => DotEnv.Load(this);
}
