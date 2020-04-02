# dotenv.net

[![CircleCI](https://circleci.com/gh/bolorundurowb/dotenv.net.svg?style=svg)](https://circleci.com/gh/bolorundurowb/dotenv.net) [![NuGet Badge](https://buildstats.info/nuget/dotenv.net)](https://www.nuget.org/packages/dotenv.net) [![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE) [![Coverage Status](https://coveralls.io/repos/github/bolorundurowb/dotenv.net/badge.svg?branch=master)](https://coveralls.io/github/bolorundurowb/dotenv.net?branch=master)

dotenv.net is a zero-dependency module that loads environment variables from a .env environment variable file into `System.Environment`. It has built in support for the in-built dependency injection framework packaged with ASP.NET Core. It now comes packaged with an interface that allows for reading environment variables wihtout repeated calls to `Environment.GetEnvironmentVariable("KEY");`.  If you have ideas or issues, create an issue.

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

At times the `.env` would not be in the same directory as your assembly, in that case the following would apply

```csharp
using dotenv.net

...

var success = DotEnv.AutoConfig();
if(success)
{
  // env successfully found and read
}
```

this looks through your assembly's folder and three folders up for a `.env` file and loads that.

the values saved in your `.env` file would be availale in your application and can be accessed via:
 ```csharp
Environment.GetEnvironmentVariable("DB_HOST"); // would output 'localhost'
```