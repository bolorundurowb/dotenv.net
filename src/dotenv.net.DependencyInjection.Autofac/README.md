# dotenv.net

[![NuGet Badge](https://buildstats.info/nuget/dotenv.net.DependencyInjection.Autofac)](https://www.nuget.org/packages/dotenv.net.DependencyInjection.Autofac)

## Usage

### Conventional

First install the library as a dependency in your application from nuget

```
Install-Package dotenv.net.DependencyInjection.Autofac
```

or

```
dotnet add package dotenv.net.DependencyInjection.Autofac
```

or for paket

```
paket add dotenv.net.DependencyInjection.Autofac
```

## Reading the Environment Variables

If using with ASP.NET Core or any other system that uses `ContainerBuilder` for its dependency injection, in the `Startup.cs` file

``` csharp
public void ConfigureContainer(ContainerBuilder containerBuilder)
{
  ...

  // configure dotenv
  containerBuilder.AddEnv(builder => {
      builder
      .AddEnvFile("/custom/path/to/your/env/vars")
      .AddThrowOnError(false)
      .AddEncoding(Encoding.ASCII);
  });
}
```

## Injecting the EnvReader

If using with ASP.NET Core or any other system that uses `ContainerBuilder` for its dependency injection, in the `Startup.cs` file

``` csharp
public void ConfigureContainer(ContainerBuilder containerBuilder)
{
    ...

    // inject the env reader
    containerBuilder.AddEnvReader();
}
```

In a controller or service you can then inject the reader

```csharp
...

public class StuffController: ControllerBase(IEnvReader envReader)
{
  // you can use the reader now to get specific variables by key
}
```