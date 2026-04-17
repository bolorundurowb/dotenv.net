# dotenv.net

[![Build, Test & Coverage](https://github.com/bolorundurowb/dotenv.net/actions/workflows/build-and-test.yml/badge.svg)](https://github.com/bolorundurowb/dotenv.net/actions/workflows/build-and-test.yml)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)
[![codecov](https://codecov.io/gh/bolorundurowb/dotenv.net/graph/badge.svg?token=Qw8xSiEHNp)](https://codecov.io/gh/bolorundurowb/dotenv.net)
![NuGet Version](https://img.shields.io/nuget/v/dotenv.net)

[![project icon](https://res.cloudinary.com/dg2dgzbt4/image/upload/v1587070177/external_assets/open_source/icons/dotenv.png)]()

**dotenv.net** is a lightweight library for loading environment variables from `.env` files in .NET applications. It
keeps sensitive configuration out of source code and supports a range of options to suit different project setups.

## Installation

Install via NuGet using any of the following methods:

**.NET CLI**

```bash
dotnet add package dotenv.net
```

**Package Manager Console**

```powershell
Install-Package dotenv.net
```

**PackageReference**

```xml

<PackageReference Include="dotenv.net" Version="4.x.x"/>
```

## Quick Start

```csharp
using dotenv.net;

// Load .env from the application directory into the system environment
DotEnv.Load();

// Read variables from .env without modifying the system environment
var envVars = DotEnv.Read();
Console.WriteLine(envVars["DATABASE_URL"]);
```

By default, both methods look for a `.env` file in the same directory as the application executable.

## Configuration Options

Both `Load()` and `Read()` accept a `DotEnvOptions` instance to customise behaviour.

```csharp
DotEnv.Load(options: new DotEnvOptions(
    ignoreExceptions: false,           // Throw on errors instead of silently failing (default: true)
    envFilePaths: ["./config/.env"],   // One or more paths to .env files (default: [".env"])
    encoding: Encoding.UTF8,           // File encoding (default: UTF-8)
    trimValues: true,                  // Strip whitespace from values (default: false)
    overwriteExistingVars: false,      // Skip vars already set in the environment (default: true)
    probeForEnv: true,                 // Search parent directories for a .env file (default: false)
    probeLevelsToSearch: 3,            // How many directory levels to ascend when probing (default: 4)
    supportExportSyntax: true          // Support `export KEY=VALUE` syntax (default: false)
));
```

> **Note:** `probeForEnv` and `envFilePaths` are mutually exclusive. Setting both will throw an
`InvalidOperationException`.

### Loading multiple `.env` files

```csharp
DotEnv.Load(options: new DotEnvOptions(
    envFilePaths: ["./config/.env", "./secrets/.env"]
));
```

When `overwriteExistingVars` is `false`, keys from earlier files take precedence over those in later files.

## Fluent API

`DotEnv.Fluent()` returns a `DotEnvOptions` instance that exposes a chainable builder API, useful when you prefer an
explicit, readable configuration style.

**Loading variables:**

```csharp
DotEnv.Fluent()
    .WithExceptions()
    .WithEnvFiles("./config/.env")
    .WithTrimValues()
    .WithEncoding(Encoding.UTF8)
    .WithOverwriteExistingVars()
    .WithProbeForEnv(probeLevelsToSearch: 6)
    .WithSupportExportSyntax()
    .Load();
```

**Reading variables without writing to the environment:**

```csharp
var envVars = DotEnv.Fluent()
    .WithoutExceptions()
    .WithEnvFiles()               // Defaults to .env
    .WithoutTrimValues()
    .WithEncoding(Encoding.UTF8)
    .WithoutOverwriteExistingVars()
    .WithoutProbeForEnv()
    .WithoutSupportExportSyntax()
    .Read();
```

### Fluent builder methods

| Method                           | Description                                        |
|----------------------------------|----------------------------------------------------|
| `WithExceptions()`               | Throw exceptions on errors                         |
| `WithoutExceptions()`            | Silently ignore errors (default)                   |
| `WithEnvFiles(params string[])`  | Specify one or more `.env` file paths              |
| `WithEncoding(Encoding)`         | Set file encoding                                  |
| `WithTrimValues()`               | Strip whitespace from values                       |
| `WithoutTrimValues()`            | Preserve whitespace in values (default)            |
| `WithOverwriteExistingVars()`    | Overwrite existing environment variables (default) |
| `WithoutOverwriteExistingVars()` | Preserve existing environment variables            |
| `WithProbeForEnv(int)`           | Search parent directories for a `.env` file        |
| `WithoutProbeForEnv()`           | Disable parent directory search (default)          |
| `WithSupportExportSyntax()`      | Support `export KEY=VALUE` syntax                  |
| `WithoutSupportExportSyntax()`   | Disable export syntax support (default)            |

## Reading Variables

`DotEnv.Read()` returns an `IDictionary<string, string>` of the parsed key-value pairs without writing anything to the
system environment. This is useful for inspecting values or selectively applying them.

```csharp
var envVars = DotEnv.Read();

if (envVars.TryGetValue("API_KEY", out var apiKey))
{
    // use apiKey
}
```

## EnvReader Utility

The `dotenv.net.Utilities` namespace provides `EnvReader`, a helper class for reading strongly-typed values directly
from the system environment (i.e., after calling `DotEnv.Load()`).

```csharp
using dotenv.net.Utilities;

var host = EnvReader.GetStringValue("DB_HOST");
var port = EnvReader.GetIntValue("DB_PORT");
var enabled = EnvReader.GetBooleanValue("FEATURE_FLAG");
```

For non-throwing alternatives, use the `TryGet*` methods:

```csharp
if (EnvReader.TryGetIntValue("DB_PORT", out var port))
{
    // port is valid
}
```

### Available methods

| Method                                              | Return Type | Throws if missing              |
|-----------------------------------------------------|-------------|--------------------------------|
| `HasValue(string key)`                              | `bool`      | No                             |
| `GetStringValue(string key)`                        | `string`    | Yes                            |
| `GetIntValue(string key)`                           | `int`       | Yes                            |
| `GetDoubleValue(string key)`                        | `double`    | Yes                            |
| `GetDecimalValue(string key)`                       | `decimal`   | Yes                            |
| `GetBooleanValue(string key)`                       | `bool`      | Yes                            |
| `TryGetStringValue(string key, out string value)`   | `bool`      | No, returns `null` on failure  |
| `TryGetIntValue(string key, out int value)`         | `bool`      | No, returns `0` on failure     |
| `TryGetDoubleValue(string key, out double value)`   | `bool`      | No, returns `0.0` on failure   |
| `TryGetDecimalValue(string key, out decimal value)` | `bool`      | No, returns `0.0m` on failure  |
| `TryGetBooleanValue(string key, out bool value)`    | `bool`      | No, returns `false` on failure |

## Contributing

Contributions are welcome. If you have a bug report, feature request, or improvement in mind,
please [open an issue](https://github.com/bolorundurowb/dotenv.net/issues) or submit a pull request.

To get started:

1. Fork the repository and create a feature branch.
2. Make your changes and ensure existing tests still pass.
3. Add tests for any new behaviour.
4. Open a pull request with a clear description of what was changed and why.

The project targets .NET and uses the standard `dotnet` CLI toolchain. Run the test suite with:

```bash
dotnet test
```

## Contributors

Thanks to everyone who has contributed to **dotenv.net**:

[@bolorundurowb](https://github.com/bolorundurowb) [@joliveros](https://github.com/joliveros) [@vizeke](https://github.com/vizeke)
[@merqlove](https://github.com/merqlove) [@tracker1](https://github.com/tracker1) [@NaturalWill](https://github.com/NaturalWill)
[@texyh](https://github.com/texyh) [@jonlabelle](https://github.com/jonlabelle) [@Gounlaf](https://github.com/Gounlaf)
[@DTTerastar](https://github.com/DTTerastar) [@Mondonno](https://github.com/Mondonno) [@caveman-dick](https://github.com/caveman-dick)
[@VijoPlays](https://github.com/VijoPlays) [@bobbyg603](https://github.com/bobbyg603)

## License

**dotenv.net** is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.

**Happy Coding!** 🚀