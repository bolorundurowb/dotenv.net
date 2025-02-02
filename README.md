# dotenv.net 🌟

[![CI - Build, Test & Coverage](https://github.com/bolorundurowb/dotenv.net/actions/workflows/build.yml/badge.svg)](https://github.com/bolorundurowb/dotenv.net/actions/workflows/build.yml)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)
[![Coverage Status](https://coveralls.io/repos/github/bolorundurowb/dotenv.net/badge.svg?branch=master)](https://coveralls.io/github/bolorundurowb/dotenv.net?branch=master)
![NuGet Version](https://img.shields.io/nuget/v/dotenv.net)


[![project icon](https://res.cloudinary.com/dg2dgzbt4/image/upload/v1587070177/external_assets/open_source/icons/dotenv.png)]()

**dotenv.net** is a lightweight and intuitive library designed to simplify the process of managing environment variables in .NET applications. By seamlessly integrating with `.env` files, it allows developers to maintain a clean and secure configuration setup, ensuring that sensitive information is kept out of source code. 🔒

Whether you're building a small project or a large-scale application, **dotenv.net** provides the tools you need to efficiently load, read, and manage environment variables, with support for dependency injection (DI) in popular DI systems. 🛠️

---

## Why Use dotenv.net? 🤔

- **Simple and Pain-Free** 🎯: Easily load and read `.env` files with minimal setup.
- **Flexible Configuration** 🔧: Customize how environment variables are loaded with a variety of options.
- **Dependency Injection Support** 🧩: Works seamlessly with popular DI frameworks.
- **Cross-Platform** 🌍: Fully compatible with .NET Core, .NET 5, and beyond.
- **Open Source** 💡: Actively maintained and supported by the community.

---

## Getting Started 🚀

### Installation 📦

You can install **dotenv.net** via NuGet:

- **Using the .NET CLI**:
  ```bash
  dotnet add package dotenv.net
  ```

- **Using Visual Studio Package Manager**:
  ```cmd
  Install-Package dotenv.net
  ```

- **Manual Installation** (via `.csproj`):
  ```xml
  <PackageReference Include="dotenv.net" Version="3.0.0"/>
  ```

---

## Usage 🛠️

### Basic Setup 🏗️

1. **Add the Namespace**:
   ```csharp
   using dotenv.net;
   ```

2. **Load Environment Variables**:
   ```csharp
   DotEnv.Load();
   ```

   This will automatically locate and load the `.env` file in the same directory as your application.

---

### Advanced Configuration ⚙️

**dotenv.net** offers a wide range of configuration options to tailor the loading process to your needs:

- **Specify Custom `.env` File Paths**:
  ```csharp
  DotEnv.Load(options: new DotEnvOptions(envFilePaths: new[] {"./path/to/env", "./path/to/second/env"}));
  ```

- **Enable Exception Handling**:
  ```csharp
  DotEnv.Load(options: new DotEnvOptions(ignoreExceptions: false));
  ```

- **Search for `.env` Files in Parent Directories**:
  ```csharp
  DotEnv.Load(options: new DotEnvOptions(probeForEnv: true, probeLevelsToSearch: 2));
  ```

- **Trim Whitespace from Values**:
  ```csharp
  DotEnv.Load(options: new DotEnvOptions(trimValues: true));
  ```

- **Skip Overwriting Existing Environment Variables**:
  ```csharp
  DotEnv.Load(options: new DotEnvOptions(overwriteExistingVars: false));
  ```

---

### Reading Environment Variables 📖

Use the `Read()` method to retrieve environment variables without modifying the system environment:

```csharp
var envVars = DotEnv.Read();
Console.WriteLine(envVars["KEY"]); // Outputs the value associated with 'KEY'
```

---

### Fluent API 🎨

For a more expressive syntax, **dotenv.net** provides a fluent API:

```csharp
// Load environment variables with custom options
DotEnv.Fluent()
    .WithExceptions()
    .WithEnvFiles("./path/to/env")
    .WithTrimValues()
    .WithEncoding(Encoding.ASCII)
    .WithOverwriteExistingVars()
    .WithProbeForEnv(probeLevelsToSearch: 6)
    .Load();

// Read environment variables
var envVars = DotEnv.Fluent()
    .WithoutExceptions()
    .WithEnvFiles() // Defaults to .env
    .WithoutTrimValues()
    .WithDefaultEncoding()
    .WithoutOverwriteExistingVars()
    .WithoutProbeForEnv()
    .Read();
```

---

### Environment Variable Helpers 🛠️

The `Utilities` namespace provides additional methods for reading environment variables in a typed manner:

```csharp
using dotenv.net.Utilities;

var stringValue = EnvReader.GetStringValue("KEY");
var intValue = EnvReader.GetIntValue("PORT");
var boolValue = EnvReader.GetBooleanValue("ENABLE_FEATURE");
```

#### Available Methods 📋

| Method Name                     | Description                                                                 | Return Type | Default (if applicable) |
|---------------------------------|-----------------------------------------------------------------------------|-------------|-------------------------|
| `HasValue(string key)`          | Checks if a value is set for the given key.                                 | `bool`      | `N/A`                  |
| `GetStringValue(string key)`    | Retrieves a string value by key. Throws an exception if not found.          | `string`    | `N/A`                  |
| `GetIntValue(string key)`       | Retrieves an integer value by key. Throws an exception if not found.        | `int`       | `N/A`                  |
| `GetDoubleValue(string key)`    | Retrieves a double value by key. Throws an exception if not found.          | `double`    | `N/A`                  |
| `GetDecimalValue(string key)`   | Retrieves a decimal value by key. Throws an exception if not found.         | `decimal`   | `N/A`                  |
| `GetBooleanValue(string key)`   | Retrieves a boolean value by key. Throws an exception if not found.         | `bool`      | `N/A`                  |
| `TryGetStringValue(string key, out string value)` | Safely retrieves a string value. Returns `true` if successful. | `bool`      | `null`                 |
| `TryGetIntValue(string key, out int value)`       | Safely retrieves an integer value. Returns `true` if successful. | `bool`      | `0`                    |
| `TryGetDoubleValue(string key, out double value)` | Safely retrieves a double value. Returns `true` if successful.  | `bool`      | `0.0`                  |
| `TryGetDecimalValue(string key, out decimal value)` | Safely retrieves a decimal value. Returns `true` if successful. | `bool`      | `0.0m`                 |
| `TryGetBooleanValue(string key, out bool value)`  | Safely retrieves a boolean value. Returns `true` if successful. | `bool`      | `false`                |

---

## Contributing 🤝

We welcome contributions from the community! If you have ideas, bug reports, or feature requests, please [open an issue](https://github.com/bolorundurowb/dotenv.net/issues) or submit a pull request.

### Special Thanks to Our Contributors 🙏

A huge shoutout to everyone who has contributed to **dotenv.net**:

[@bolorundurowb](https://github.com/bolorundurowb) [@joliveros](https://github.com/joliveros) [@vizeke](https://github.com/vizeke)  
[@merqlove](https://github.com/merqlove) [@tracker1](https://github.com/tracker1) [@NaturalWill](https://github.com/NaturalWill)  
[@texyh](https://github.com/texyh) [@jonlabelle](https://github.com/jonlabelle) [@Gounlaf](https://github.com/Gounlaf)  
[@DTTerastar](https://github.com/DTTerastar) [@Mondonno](https://github.com/Mondonno) [@caveman-dick](https://github.com/caveman-dick)  
[@VijoPlays](https://github.com/VijoPlays)  [bobbyg603](https://github.com/bobbyg603)

---

## License 📜

**dotenv.net** is licensed under the MIT License. See the [LICENSE](LICENSE) file for more details.

---

## Get Started Today! 🎉

Simplify your environment variable management with **dotenv.net**. Install the package, follow the quick start guide, and enjoy a cleaner, more secure configuration setup for your .NET applications.

**Happy Coding!** 🚀