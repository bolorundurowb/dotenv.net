# dotenv.net

[![CI - Build, Test & Coverage](https://github.com/bolorundurowb/dotenv.net/actions/workflows/build.yml/badge.svg)](https://github.com/bolorundurowb/dotenv.net/actions/workflows/build.yml)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE) 
[![Coverage Status](https://coveralls.io/repos/github/bolorundurowb/dotenv.net/badge.svg?branch=master)](https://coveralls.io/github/bolorundurowb/dotenv.net?branch=master)
[![NuGet Badge](https://buildstats.info/nuget/dotenv.net)](https://www.nuget.org/packages/dotenv.net)

[![project icon](https://res.cloudinary.com/dg2dgzbt4/image/upload/v1587070177/external_assets/open_source/icons/dotenv.png)]()

dotenv.net is a group of projects that aim to make the process of reading `.env` files as simple and pain-free as
possible in the dotnet ecosystem. It contains a core library that holds the env reading functionality and two libraries
that add dependency injection (DI) support for two popular DI systems. If you have ideas or issues, feel free to create
an issue.

## Important

Version 3 is a huge departure from the previous system of accessing the library functionality. Deprecated methods from
version 2.x have been removed as indicated. New functionality has been added but the reading and parsing logic has
mostly stayed unchanged meaning at it's heart, is still the same ol' reliable.

## Installation

If you are hardcore and want to go the manual route. Then add the following to your `csproj` file:

```xml
<PackageReference Include="dotenv.net" Version="3.0.0"/>
```

If you're using the Visual Studio package manager console, then run the following:

```cmd
Install-Package dotenv.net
```

If you are making use of the dotnet CLI, then run the following in your terminal:

```bash
dotnet add package dotenv.net
```

## Usage

Ensure you have declared the necessary namespace at the head of your class file:

```csharp
using dotenv.net;
```

### Load Environment Variables

Calling the `Load()` method with no parameters would locate and load the `.env` file in the same directory that the library is if one exists:

```csharp
DotEnv.Load();
```

If you want to be notified of exceptions that occur in the process of loading env files then you can specify that via the configuration options:

```csharp
DotEnv.Load(options: new DotEnvOptions(ignoreExceptions: false));
```

You can specify the env files to be loaded. One or more can be loaded. (NOTE: the order in which the env paths are provided is crucial. If there is a duplicate
key and value specified in an env file specified later in the list, that value would overwrite the earlier values read). *The default is `.env`*:

```csharp
DotEnv.Load(options: new DotEnvOptions(envFilePaths: new[] {"./path/to/env", "./path/to/second/env"}));
```

To search up from the executing library's directory for an env file. The directories would be searched upwards i.e given a directory path `/path/to/var`,
The `var` directory would be searched first, then the `to` directory and then the `path` directory. The options allow for probing the directories as well
as specifying how high up to search.  *The defaults are `false` and `4` directories up*:

```csharp
DotEnv.Load(options: new DotEnvOptions(probeForEnv: true, probeLevelsToSearch: 2)); // this would only search 2 directories up from the executing directory.
```

If the provided env files are not `UTF-8` encoded, then the encoding to use in reading the files can be specified. *The default is `UTF-8`*:

```csharp
using System.Text;
...
DotEnv.Load(options: new DotEnvOptions(encoding: Encoding.ASCII));
```

To trim extraneous whitespace from the values read from the file(s). *The default is `false`*:

```csharp
DotEnv.Load(options: new DotEnvOptions(trimValues: true));
```

To skip overwriting an environment variable if it is set. *The default is `true`*:

```csharp
DotEnv.Load(options: new DotEnvOptions(overwriteExistingVars: false));
```

<br>

### Read Environment Variables

The `Read()`  method returns a `IDictionary<string, string>` instance detailing the keys and associated values read from the env files provided. This
hase the added advantage of not modifying your system environment variables. The same options as apply to the `Load()` method, apply to the `Read()` method as well.

```csharp
var envVars = DotEnv.Read();
Console.WriteLine(envVars["KEY"]); // would print out whatever value was associated with the 'KEY'
```

### Defining env variables

Environment variables can be defined in various ways:

* \# I am comment
* hello=world
* SINGLE_QUOTES='single'
* DOUBLE_QUOTES="double"

Any variable starting with # will be ignored by the parser. 

Any variable containing no quotes or single quotes will be read as a variable.

Any variable containing double quotes will be considered a multi-line variable, and can thus either be finished in the same line - or another line.

<br>

### Fluent API

There is a fluent API analogue to the static methods documented above and can be terminated with a `Read()` or `Load()` call to return the env value or
write to the environment variables . 

```csharp
// to load env vars with the specified options
DotEnv.Fluent()
    .WithExceptions()
    .WithEnvFiles("./path/to/env")
    .WithTrimValues()
    .WithEncoding()
    .WithOverwriteExistingVars()
    .WithProbeForEnv(probeLevelsToSearch: 6)
    .Load();

// to read the env vars with the specified options
var envVars = DotEnv.Fluent()
    .WithoutExceptions()
    .WithEnvFiles() // revert to the default .env file
    .WithoutTrimValues()
    .WithDefaultEncoding()
    .WithoutOverwriteExistingVars()
    .WithoutProbeForEnv()
    .Read();
```

<br>

### Environment Variable Helpers

The `Utilities` namespace provides additional classes to aid in reading environment in a typed manner as well as other sundry assistive methods.
Ensure you have declared the necessary namespace at the head of your class file:

```csharp
using dotenv.net.Utilities;
...
var value = EnvReader.GetStringValue("KEY");
```

#### EnvReader Methods

|  Method Name  |  Description  |  Return Type  | Default (if applicable) |
| ------------- | ------------- | ------------- |:-----------------------:|
| HasValue(string key)      | States whether the given key has a value set or not. | `bool` | `N/A` |
| GetStringValue(string key)      | Retrieve a value from the current environment by the given key and throws an exception if not found. | `string` | `N/A` |
| GetIntValue(string key)      | Retrieve a value from the current environment by the given key and throws an exception if not found. | `int` | `N/A` |
| GetDoubleValue(string key)      | Retrieve a value from the current environment by the given key and throws an exception if not found. | `double` | `N/A` |
| GetDecimalValue(string key)      | Retrieve a value from the current environment by the given key and throws an exception if not found. | `decimal` | `N/A` |
| GetBooleanValue(string key)      | Retrieve a value from the current environment by the given key and throws an exception if not found. | `bool` | `N/A` |
| TryGetStringValue(string key, out string value)      | A safer method to use when retrieving values from the environment as it returns a boolean value stating whether it successfully retrieved the value required. | `bool` | `null` |
| TryGetIntValue(string key, out int value)      | A safer method to use when retrieving values from the environment as it returns a boolean value stating whether it successfully retrieved the value required. | `bool` | `0` |
| TryGetDoubleValue(string key, out double value)     | A safer method to use when retrieving values from the environment as it returns a boolean value stating whether it successfully retrieved the value required. | `bool` | `0.0` |
| TryGetDecimalValue(string key, out decimal value)      | A safer method to use when retrieving values from the environment as it returns a boolean value stating whether it successfully retrieved the value required. | `bool` | `0.0m` |
| TryGetBooleanValue(string key, out bool value)     | A safer method to use when retrieving values from the environment as it returns a boolean value stating whether it successfully retrieved the value required. | `bool` | `false` |

<br>

## Contributors

:clap: :clap: :clap: Huge thanks to those who have contributed to these libraries.

[@bolorundurowb](https://github.com/bolorundurowb) [@joliveros](https://github.com/joliveros) [@vizeke](https://github.com/vizeke)
[@merqlove](https://github.com/merqlove) [@tracker1](https://github.com/tracker1)  [@NaturalWill](https://github.com/NaturalWill)
[@texyh](https://github.com/texyh) [@jonlabelle](https://github.com/jonlabelle) [@Gounlaf](https://github.com/Gounlaf)
[@DTTerastar](https://github.com/DTTerastar) [@Mondonno](https://github.com/Mondonno) [@caveman-d**k](https://github.com/caveman-dick) 
[@VijoPlays](https://github.com/VijoPlays)
