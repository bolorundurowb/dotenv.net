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
    supportExportSyntax: true,         // Support `export KEY=VALUE` syntax (default: false)
    supportInlineComments: true,       // Strip `# comment` from unquoted values (default: true)
    supportVariableExpansion: true,    // Expand ${VAR} and $VAR variable references (default: true)
    environmentCascade: true,          // Load layered .env / .env.local / .env.{Environment} files (default: false)
    environmentName: "Development"     // Optional explicit environment for cascade loading
));
```

> **Note:** `probeForEnv` and custom `envFilePaths` are mutually exclusive. `environmentCascade` cannot be combined with custom `envFilePaths` or `envStreams`. `environmentCascade` can be used together with `probeForEnv`. Setting conflicting options will throw an `InvalidOperationException`.

### Loading multiple `.env` files

```csharp
DotEnv.Load(options: new DotEnvOptions(
    envFilePaths: ["./config/.env", "./secrets/.env"]
));
```

When `overwriteExistingVars` is `false`, keys from earlier files take precedence over those in later files.

### Environment cascading (hierarchical loading)

Opt-in hierarchical loading resolves layered `.env` files from the same directory as `.env` (or from the probed directory when `probeForEnv` is enabled). Later files override earlier files when `overwriteExistingVars` is `true` (the default):

1. `.env`
2. `.env.local`
3. `.env.{Environment}` (only when an environment name is known)
4. `.env.{Environment}.local` (only when an environment name is known)

The environment name is resolved in this order: an explicit name passed to `WithEnvironmentCascade`, then `ASPNETCORE_ENVIRONMENT`, `DOTNET_ENVIRONMENT`, and `DOTENV_ENV`. If none are set, only `.env` and `.env.local` are considered. Missing files in the cascade are skipped. Add `*.local` to `.gitignore` so machine-specific secrets are not committed.

```csharp
DotEnv.Fluent()
    .WithEnvironmentCascade()                 // uses ASPNETCORE_ENVIRONMENT / DOTNET_ENVIRONMENT / DOTENV_ENV
    .Load();

DotEnv.Fluent()
    .WithEnvironmentCascade("Production")     // explicit environment name
    .WithProbeForEnv()                        // cascade + probe is allowed
    .Load();
```

`WithEnvironmentCascade()` cannot be combined with custom `WithEnvFiles(...)` paths or `WithEnvStreams(...)`.

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

| Method                              | Description                                               |
|-------------------------------------|-----------------------------------------------------------|
| `WithExceptions()`                  | Throw exceptions on errors                                |
| `WithoutExceptions()`               | Silently ignore errors (default)                          |
| `WithEnvFiles(params string[])`     | Specify one or more `.env` file paths                     |
| `WithEncoding(Encoding)`            | Set file encoding                                         |
| `WithTrimValues()`                  | Strip whitespace from values                              |
| `WithoutTrimValues()`               | Preserve whitespace in values (default)                   |
| `WithOverwriteExistingVars()`       | Overwrite existing environment variables (default)        |
| `WithoutOverwriteExistingVars()`    | Preserve existing environment variables                   |
| `WithProbeForEnv(int)`              | Search parent directories for a `.env` file               |
| `WithoutProbeForEnv()`              | Disable parent directory search (default)                 |
| `WithSupportExportSyntax()`         | Support `export KEY=VALUE` syntax                         |
| `WithoutSupportExportSyntax()`      | Disable export syntax support (default)                   |
| `WithSupportInlineComments()`       | Strip `# comment` from unquoted values (default)          |
| `WithoutSupportInlineComments()`    | Preserve inline comments in values                        |
| `WithSupportVariableExpansion()`    | Enable variable expansion and interpolation               |
| `WithoutSupportVariableExpansion()` | Disable variable expansion and interpolation (default)    |
| `WithVariableExpansion()`           | Alias for `WithSupportVariableExpansion()`                |
| `WithoutVariableExpansion()`        | Alias for `WithoutSupportVariableExpansion()`             |
| `WithEnvironmentCascade(string?)`   | Load `.env`, `.env.local`, and environment-specific files |
| `WithoutEnvironmentCascade()`       | Disable hierarchical loading (default)                    |

## Variable Expansion & Interpolation

**dotenv.net** supports variable expansion (substitution), allowing values to reference other variables or system environment settings using `${VAR}` or `$VAR` syntax:

```bash
BASE_URL=https://api.example.com
API_ENDPOINT=${BASE_URL}/v1
PORT=8080
DATABASE_URL=postgres://${USER}:${PASSWORD}@localhost:${PORT}/db
```

### Syntax and Features

- **Braced variables**: `${VAR}` expands to the value of `VAR`.
- **Short variables**: `$VAR` expands `VAR` matching `[A-Za-z_][A-Za-z0-9_]*`. Dollar signs not followed by a valid identifier (e.g. `$100` or a trailing `$`) are preserved literally.
- **Default fallbacks**:
  - `${VAR:-default}`: Evaluates to `default` if `VAR` is unset or empty.
  - `${VAR-default}`: Evaluates to `default` if `VAR` is unset (preserves an explicitly set empty value).
- **Nested expressions**: Fallbacks can be nested, e.g. `${CUSTOM_URL:-${DEFAULT_HOST:-localhost}:3000}`.
- **Resolution hierarchy**: Variables are resolved sequentially in document order against earlier parsed keys, cascading to `System.Environment` if not set in the `.env` file. Missing variables without a default resolve to an empty string.

### Quoting and Escaping

- **Single quotes (`'...'`)**: Treated as raw literals. Variables inside single quotes are never expanded (e.g. `'${NOT_EXPANDED}'` remains literal `${NOT_EXPANDED}`).
- **Double quotes (`"..."`) and unquoted values**: Variable expansion is enabled.
- **Backslash escaping**: Preceding a dollar sign with a backslash prevents expansion (e.g. `\${VAR}` evaluates to `${VAR}`, and `\$VAR` evaluates to `$VAR`).

### Circular Dependency Protection

Circular references (such as direct `A=${A}` or indirect `A=${B}`, `B=${A}`) are detected automatically:
- When `ignoreExceptions` is `false`, an `InvalidOperationException` is thrown identifying the cycle path.
- When `ignoreExceptions` is `true`, the cyclic reference resolves safely to an empty string without crashing or causing a stack overflow.

To enable variable expansion:

```csharp
DotEnv.Fluent()
    .WithVariableExpansion()
    .Load();
```

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

## Microsoft.Extensions.Configuration Integration

The `dotenv.net.Configuration` package adds native `IConfigurationProvider` support, so `.env` files flow into
`IConfiguration`, `IOptions<T>`, dependency injection, and ASP.NET Core `WebApplicationBuilder` — no manual plumbing.

**Installation**

```bash
dotnet add package dotenv.net.Configuration
```

**Usage**

```csharp
using dotenv.net.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddDotNetEnv();                       // loads .env with default options
builder.Configuration.AddDotNetEnv(options => options
    .WithEncoding(Encoding.UTF8)
    .WithEnvFiles(".env", ".env.development")
    .WithTrimValues());
```

Values are then available everywhere configuration is used:

```csharp
// Direct access
var connectionString = builder.Configuration["ConnectionStrings:Default"];

// Bound to strongly-typed options
builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));
```

The `AddDotNetEnv` overloads accept `DotEnvOptions` directly or a `Action<DotEnvOptions>` configuration delegate, so
all existing options (multiple files, streams, probe, cascade, variable expansion, etc.) are supported. Sources are
applied in the order they are added; later sources override earlier ones.

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
[@VijoPlays](https://github.com/VijoPlays) [@bobbyg603](https://github.com/bobbyg603) [@Moha-sami](https://github.com/Moha-sami)

## License

**dotenv.net** is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.

**Happy Coding!** 🚀