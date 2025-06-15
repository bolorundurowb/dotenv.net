using System;
using System.Collections.Generic;
using dotenv.net.Utilities;

namespace dotenv.net;

internal static class Writer
{
    public static void WriteToEnv(IDictionary<string, string> envVars, bool overwriteExistingVars)
    {
        foreach (var envVar in envVars)
            if (overwriteExistingVars || !EnvReader.HasValue(envVar.Key))
                Environment.SetEnvironmentVariable(envVar.Key, envVar.Value);
    }
}
