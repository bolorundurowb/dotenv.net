# dotenv.net

[![CircleCI](https://circleci.com/gh/bolorundurowb/dotenv.net.svg?style=svg)](https://circleci.com/gh/bolorundurowb/dotenv.net) [![NuGet Badge](https://buildstats.info/nuget/dotenv.net)](https://www.nuget.org/packages/dotenv.net) [![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE) [![Coverage Status](https://coveralls.io/repos/github/bolorundurowb/dotenv.net/badge.svg?branch=)](https://coveralls.io/github/bolorundurowb/dotenv.net?branch=)

dotenv.net is a zero-dependency module that loads environment variables from a .env environment variable file into `System.Environment`. It has built in support for the in-built dependency injection framework packaged with ASP.NET Core. It now comes packaged with an interface that allows for reading environment variables wihtout repeated calls to `Environment.GetEnvironmentVariable("KEY");`.  If you have ideas or issues, create an issue.

## Contributors

Big ups to those who have contributed to this library. :clap:

[@bolorundurowb](https://github.com/bolorundurowb) [@joliveros](https://github.com/joliveros) [@vizeke](https://github.com/vizeke) [@merqlove](https://github.com/merqlove) [@tracker1](https://github.com/tracker1)  [@NaturalWill](https://github.com/NaturalWill)  [@texyh](https://github.com/texyh)

## Usage

### Conventional

First install the library as a dependency in your application from nuget

```
Install-Package dotenv.net
```

or

```
dotnet add package dotenv.net
```

or for paket

```
paket add dotenv.net
```

Create a file with no filename and an extension of `.env`.

A sample `.env` file would look like this:
```text
DB_HOST=localhost
DB_USER=root
DB_PASS=s1mpl3
```

in the `Startup.cs` file or as early as possible in your code add the following:

```csharp
using dotenv.net;


...


DotEnv.Config();
```

the values saved in your `.env` file would be avaibale in your application and can be accessed via
 ```csharp
Environment.GetEnvironmentVariable("DB_HOST"); // would output 'localhost'
```

## Using with DI (`IServiceCollection`)

If using with ASP.NET Core or any other system that uses `IServiceCollection` for its dependency injection, in the `Startup.cs` file

``` csharp
public void ConfigureServices(IServiceCollection services)
{
    ...

    // configure dotenv
    services.AddEnv(builder => {
        builder
        .AddEnvFile("/custom/path/to/your/env/vars")
        .AddThrowOnError(false)
        .AddEncoding(Encoding.ASCII);
    });
}
```

With this, your application would have the env file variables imported.

### Options

#### ThrowError

Default: `true`

You can specify if you want the library to error out if any issue arises or fail silently.

```csharp
DotEnv.Config(false); //fails silently
```

#### Path

Default: `.env`

You can specify a custom path if your file containing environment variables is
named or located differently.

```csharp
DotEnv.Config(true, "/custom/path/to/your/env/vars");
```

#### Encoding

Default: `Encoding.UTF8`

You may specify the encoding of your file containing environment variables
using this option.

```csharp
DotEnv.Config(true, ".env", Encoding.Unicode);
```

#### Trim Values

Default: `true`

You may specify whether or not you want the values retrieved to be trimmed i.e have all leading and trailing whitepaces removed.

```csharp
DotEnv.Config(true, ".env", Encoding.Unicode, false);
```

## Support For `IEnvReader`

With `v1.0.6` and above an interface `IEnvReader` has been introduced that specifies methods that help with reading values from the environment easily. The library has a default implementation `EnvReader` that can be added to the default ASP.NET Core DI framework (`IServiceCollection`).

### Using `EnvReader`

```csharp
using dotenv.net.Utilities;
...

var envReader = new EnvReader();
var value = envReader.GetValue("KEY");
```

### Using `IEnvReader` with DI

In the `StartUp.cs` file, in the `ConfigureServices` method

```csharp
...

public void ConfigureServices(IServiceCollection services)
{
    ...
    services.AddEnvReader();
    ...
}
```

In the rest of your application, the `IEnvReader` interface can get injected and used. For example, in a `SampleController` class for example:

```csharp
public class SampleController
{
    private readonly IEnvReader _envReader;
    
    public SampleController(IEnvReader envReader)
    {
        _envReader = envReader;
    }
}
```

### IEnvReader Methods

#### string GetValue(string key)

Default: `null`

Retrieve a value from the current environment by the given key and return `null` if a value does not exist for that key.

#### T GetValue<T>(string key)

Default: `default(T)`

A generic method that allows for a typed value to be retrieved from the environment variables and returns the default for the type. This functionality is limited to `struct`s currently.

#### bool TryGetValue(string key, out string value)

Default: `null`

A safer method to use when retrieving values from the environment as it returns a boolean value stating whether it successfully retrieved the value required.

#### bool TryGetValue<T>(string key, out T value)

Default: `default(T)`

A safer method to use when retrieving values from the environment as it returns a boolean value stating whether it successfully retrieved and coverted the value required.
