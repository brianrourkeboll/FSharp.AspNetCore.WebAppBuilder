# FSharp.AspNetCore.WebAppBuilder

This library provides a lightweight computation expression for succinctly defining ASP.NET Core web applications, including support for minimal APIs. The goal is to optimize readability and discourage sprawl in common scenarios while including escape hatches so that you never need to work around the library.

## The `webApp` computation expression

The `webApp` computation expression exposes a number of top-level operations using custom keywords to enable declaratively defining your application. 

There are custom keywords for each of the top-level properties of `Microsoft.AspNetCore.Builder.WebApplicationBuilder`, one for the builder itself, and one for the `Microsoft.AspNetCore.Builder.WebApplication` once it's been built:

- `configuration`, corresponding to `Microsoft.AspNetCore.Builder.WebApplicationBuilder.Configuration`
- `logging`, corresponding to `Microsoft.AspNetCore.Builder.WebApplicationBuilder.Logging`
- `services`, corresponding to `Microsoft.AspNetCore.Builder.WebApplicationBuilder.Services`
- `host`, corresponding to `Microsoft.AspNetCore.Builder.WebApplicationBuilder.Host`
- `webHost`, corresponding to `Microsoft.AspNetCore.Builder.WebApplicationBuilder.WebHost`
- `builder`, corresponding to the `Microsoft.AspNetCore.Builder.WebApplicationBuilder` itself
- `webApp`, corresponding to the `Microsoft.AspNetCore.Builder.WebApplication` after it's been built

The library includes a few more specialized custom keywords to make certain common scenarios more succinct, including:

- `environmentVariables`, for adding environment variables to the app's configuration
- `jsonFile`, for adding a JSON configuration file to the app's configuration
- `singleton`, for adding a singleton instance of a dependency to the app's dependency injection container
- `hostedService`, for adding a hosted service to the app's dependency injection container
- `connectionString`, for adding a strongly-typed connection string to the app's dependency injection container
- `configurationValue`, for adding a strongly-typed configuration value to the apps' dependency injection container

I might add one or two more if I have a strong use case 🙃.

## Examples

Defining a hello-world app is dead-simple:

#### Program.fs

```fsharp
let app =
    webApp {
        get "/hello" (fun () -> "🌎")
    }

app.Run ()
```

Defining a full CRUD app with Swagger UI, [controllers](/Examples/MinimalWebAppWithControllers/Controllers/ClownsController.fs), and support for [integration testing](/Examples/Tests/MinimalWebAppWithControllersTests.fs) doesn't take much more:

#### Program.fs

```fsharp
let app builderConfig =
    webApp {
        connectionString "SqlDb" SqlConnectionString
        configurationValue "AppSettings:SqlCommandTimeout" SqlCommandTimeout
        singleton Id.NewId
        singleton typeof<IDataAccess> typeof<DataAccess>
        logging (fun logging -> logging.AddConsole ())

        services (fun services ->
            services.AddEndpointsApiExplorer ()
            services.AddSwaggerGen ()
            services.AddControllers ())

        buildWith builderConfig

        webApp (fun app ->
            app.UseSwagger ()
            app.UseSwaggerUI ()
            app.MapControllers ())
    }

(app ignore).Run ()
```

[Minimal APIs are supported](/Examples/MinimalWebApp/Program.fs), including the ability to apply arbitrary configuration to your `RouteHandlerBuilder`s:

#### Program.fs

```fsharp
let app =
    webApp {
        get [
            Status200OK,       typeof<string>
            Status404NotFound, null
        ] "/hello" (fun () ->
            if DateTime.Today.DayOfWeek = DayOfWeek.Monday then Results.NotFound ()
            else Results.Ok "🌎"
        ) (fun routeHandler ->
            routeHandler.WithOpenApi (fun op -> op.Description <- "😶😶😶"; op)
            routeHandler.AllowAnonymous ()
        )
    }

app.Run ()
```
