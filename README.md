# dotenv.net

[![CircleCI](https://circleci.com/gh/bolorundurowb/dotenv.net.svg?style=svg)](https://circleci.com/gh/bolorundurowb/dotenv.net) [![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE) [![Coverage Status](https://coveralls.io/repos/github/bolorundurowb/dotenv.net/badge.svg?branch=master)](https://coveralls.io/github/bolorundurowb/dotenv.net?branch=master) [![Made in Nigeria](https://img.shields.io/badge/made%20in-nigeria-008751.svg?style=flat-square)](https://github.com/acekyd/made-in-nigeria)

[![project icon](https://res.cloudinary.com/dg2dgzbt4/image/upload/v1587070177/external_assets/open_source/icons/dotenv.png)]()

dotenv.net is a group of projects that aim to make the process of reading `.env` files as simple and pain-free as possible in the dotnet ecosystem. It contains a core library that holds the env reading functionality and two libraries that add dependency injection (DI) support for two popular DI systems. If you have ideas or issues, feel free to create an issue.

## Contributors

Big ups to those who have contributed to these libraries. :clap:

[@bolorundurowb](https://github.com/bolorundurowb) [@joliveros](https://github.com/joliveros) [@vizeke](https://github.com/vizeke) [@merqlove](https://github.com/merqlove) [@tracker1](https://github.com/tracker1)  [@NaturalWill](https://github.com/NaturalWill)  [@texyh](https://github.com/texyh)


## How to use this documentation

Documentation specific to each project can be found in the README files for the specific projects while common features would be documented

###  Options


#### AutoConfig

You can specify if you want the library to search for .env files up the directory chain

```csharp
DotEnv.AutoConfig(); //silent if no .env file found up the chain
```

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

With `v1.0.6` and above an interface `IEnvReader` has been introduced that specifies methods that help with reading typed values from the environment easily. The library has a default implementation `EnvReader`.

### Using `EnvReader` 

```csharp
using dotenv.net.Utilities;
...

var envReader = new EnvReader();
var value = envReader.GetValue("KEY");
```

### IEnvReader Methods

#### string GetStringValue(string key)

Default: `null`

Retrieve a value from the current environment by the given key and throws an exception if not found.

#### int GetIntValue(string key)

Default: `0`

Retrieve a value from the current environment by the given key and throws an exception if not found.

#### double GetDoubleValue(string key)

Default: `0.0`

Retrieve a value from the current environment by the given key and throws an exception if not found.

#### decimal GetDecimalValue(string key)

Default: `0.0m`

Retrieve a value from the current environment by the given key and throws an exception if not found.

#### bool GetBooleanValue(string key)

Default: `false`

Retrieve a value from the current environment by the given key and throws an exception if not found.


#### bool TryGetStringValue(string key, out string value)

Default: `null`

A safer method to use when retrieving values from the environment as it returns a boolean value stating whether it successfully retrieved the value required.

#### bool TryGetIntValue(string key, out int value)

Default: `0`

A safer method to use when retrieving values from the environment as it returns a boolean value stating whether it successfully retrieved the value required.

#### bool TryGetDoubleValue(string key, out double value)

Default: `0.0`

A safer method to use when retrieving values from the environment as it returns a boolean value stating whether it successfully retrieved the value required.

#### bool TryGetDecimalValue(string key, out decimal value)

Default: `0.0m`

A safer method to use when retrieving values from the environment as it returns a boolean value stating whether it successfully retrieved the value required.

#### bool TryGetBooleanValue(string key, out bool value)

Default: `false`

A safer method to use when retrieving values from the environment as it returns a boolean value stating whether it successfully retrieved the value required.
