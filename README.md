# dotenv.net

[![CircleCI](https://circleci.com/gh/bolorundurowb/dotenv.net.svg?style=svg)](https://circleci.com/gh/bolorundurowb/dotenv.net) [![NuGet Badge](https://buildstats.info/nuget/dotenv.net)](https://www.nuget.org/packages/dotenv.net) [![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE) [![Book session on Codementor](https://cdn.codementor.io/badges/book_session_github.svg)](https://www.codementor.io/bolorundurowb?utm_source=github&utm_medium=button&utm_term=bolorundurowb&utm_campaign=github)

dotenv.net is a zero-dependency module that loads environment variables from a .env file into `Environment`.

## Usage

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

Create a file with no filename and an extension of `.env`. Add the file to the root of your project. Set the file `Copy To Output Directory` to `Always`.

As early as possible in your code
