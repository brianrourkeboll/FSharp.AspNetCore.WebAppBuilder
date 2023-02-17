# FSharp.AspNetCore.WebAppBuilder

The `webApp` computation expression lets you define ASP.NET Core web applications in a succinct and declarative way, minimizing code, maximizing readability, and providing a simple but thorough set of escape hatches, so you can always drop down to the raw ASP.NET Core APIs when you need to.

## Installation

Get it on NuGet (currently still in pre-release): [FSharp.AspNetCore.WebAppBuilder](https://www.nuget.org/packages/FSharp.AspNetCore.WebAppBuilder).

## Examples

Hello, world.

#### Program.fs

```fsharp
let app =
    webApp {
        get "/hello" (fun () -> "🌎")
        post "/chars" (fun char -> printf $"%c{char}")
        delete "/chars" (fun count -> printf $"""{String.replicate count "\b \b"}""")
    }

app.Run ()
```

Progressively [enhance your minimal API endpoints with additional metadata and configuration](/Examples/MinimalWebApp/Program.fs) while keeping it simple.

#### Program.fs

```fsharp
let app =
    webApp {
        get "/hello" [
            Status200OK, typeof<string>
            Status404NotFound, null
        ] (fun () ->
            if DateTime.Today.DayOfWeek = DayOfWeek.Monday then Results.NotFound ()
            else Results.Ok "🌎"
        ) (fun routeHandler ->
            routeHandler.WithOpenApi (fun op -> op.Description <- "Hello, 🌎 — unless it's Monday."; op)
            routeHandler.AllowAnonymous ()
        )
    }

app.Run ()
```

Wire up your dependencies in a succinct and declarative way.

#### Program.fs

```fsharp
let app =
    webApp {
        connectionString "SqlDb" SqlConnectionString
        configurationValue "AppSettings:SqlCommandTimeout" SqlCommandTimeout
        singleton Id.NewId
        singleton typeof<IDataAccess> typeof<DataAccess>

        get "/xs" (fun (dataAccess : IDataAccess) -> Results.Ok (dataAccess.GetAll ()))

        get "/xs/{id}" (fun (dataAccess : IDataAccess) id ->
            dataAccess.TryGet id
            |> Option.map Results.Ok
            |> Option.defaultWith Results.NotFound)

        post "/xs" (fun (dataAccess : IDataAccess) newId x ->
            let x = dataAccess.Create (newId (), x)
            Results.Created ($"/xs/{id}", x))

        delete "/xs/{id}" (fun (dataAccess : IDataAccess) id -> Results.NoContent (dataAccess.Delete id))
    }

app.Run ()
```

Easily evolve your code to use Swagger UI, [heavyweight controllers](/Examples/MinimalWebAppWithControllers/Controllers/ClownsController.fs), and enable [dead simple integration testing](/Examples/Tests/MinimalWebAppWithControllersTests.fs), keeping your app's definition clean and simple without giving up the freedom to use arbitrary first- or third-party APIs on the underlying `Microsoft.AspNetCore.Builder` constructs.

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

#### Tests.fs

```fsharp
[<Property>]
let ``POST /clowns returns 400 Bad Request if the shoe size is too small`` (Bad clown) =
    task {
        use app = Program.app (fun builder -> builder.WebHost.UseTestServer ())
        do! app.StartAsync ()
        use client = app.GetTestClient ()
        use! badRequest = client.PostAsJsonAsync ("clowns", clown)
        Assert.Equal (Status400BadRequest, badRequest.StatusCode)
        let! problem = badRequest.Content.ReadFromJsonAsync<ValidationProblemDetails> ()
        match Assert.Contains ("shoeSize", problem.Errors) with
        | [|_|] -> ()
        | unexpected -> failwith $"Expected a single error message but got: %A{unexpected}"
    }
    |> Async.AwaitTask
    |> Async.RunSynchronously
```

## The `webApp` computation expression

The library includes custom operations for each of the top-level properties of `Microsoft.AspNetCore.Builder.WebApplicationBuilder`, one for the builder itself, and one for the `Microsoft.AspNetCore.Builder.WebApplication` once it's been built:

- `configuration`, corresponding to `Microsoft.AspNetCore.Builder.WebApplicationBuilder.Configuration`.
- `logging`, corresponding to `Microsoft.AspNetCore.Builder.WebApplicationBuilder.Logging`.
- `services`, corresponding to `Microsoft.AspNetCore.Builder.WebApplicationBuilder.Services`.
- `host`, corresponding to `Microsoft.AspNetCore.Builder.WebApplicationBuilder.Host`.
- `webHost`, corresponding to `Microsoft.AspNetCore.Builder.WebApplicationBuilder.WebHost`.
- `builder`, corresponding to the `Microsoft.AspNetCore.Builder.WebApplicationBuilder` itself.
- `webApp`, corresponding to the `Microsoft.AspNetCore.Builder.WebApplication` after it's been built.

A few more specialized custom operations are provided to make certain common scenarios more concise, including:

- `environmentVariables`, for adding environment variables to the app's configuration.
- `jsonFile` (and `optionalJsonFile`), for adding a JSON configuration file to the app's configuration.
- `connectionString`, for adding a strongly-typed connection string to the app's dependency injection container.
- `configurationValue`, for adding a strongly-typed configuration value to the apps' dependency injection container.
- `singleton`, for adding a singleton instance of a dependency to the app's dependency injection container.
- `hostedService`, for adding a hosted background service to the app's dependency injection container.

The number of other special cases that could be added is practically infinite, corresponding to the countless first- and third-party `.Add*` extension methods and their even-more-countless overloads—`scoped`, `transient`, `serverSideBlazor`… A few more may be added, but it is unlikely to be many.

You can always call any extension method on `IServiceCollection` you like inside the `services` operation, for example:

```fsharp
services (fun services ->
    services.AddScoped<MyScopedService> ()
    services.AddHsts ()
    services.AddOptions ()
    // Etc., etc.
)
```

The same is true for any of the other top-level properties of `Microsoft.AspNetCore.Builder.WebApplicationBuilder`, or the built `Microsoft.AspNetCore.Builder.WebApplication`.

### Adding your own custom operations to the `webApp` builder

If you like, you can always add a custom operation corresponding to any first- or third-party API you choose by extending the `WebAppBuilder` yourself:

```fsharp
type WebAppBuilder with
    [<CustomOperation("scoped")>]
    member _.Scoped (builder : WebApplicationBuilder, serviceType : Type, implementationType : Type) =
        ignore <| builder.Services.AddScoped (serviceType, implementationType)
        builder
```

```fsharp
let app =
    webApp {
        scoped typeof<IScopedService> typeof<ScopedService>
    }
```

## Rationale

> ❔: Why use a computation expression? Why not write an F# wrapper using "functions and data"? I don't see anything monadic or applicative happening here.

Functions and data are absolutely more straightforward, robust, and maintainable than computation expressions with ad hoc custom operations—most of the time. But we're not designing a whole new web framework here: we're just trying to make the extensive functionality of the ASP.NET Core ecosystem a bit nicer to use in F#. There are two main things at play here:

1. Method overloads.

    The existing set of ASP.NET Core APIs, including the minimal APIs functionality, leans heavily on pervasive method overloading. Trying to wrap every overload in the ever-growing set of first- and third-party APIs using distinctly-named module functions, or else trying to plaster them over using some other kind of data structure, no matter how clever, would be a losing battle.

    Custom operations in computation expressions support method overloading while papering over some of the rough edges you run into consuming mutable builder-style APIs in F#.

2. The builder pattern.

    Again, APIs using the mutable builder pattern are just not much fun to consume in F#. In the language they were originally designed for, you don't need to explicitly discard the final return value, but in F# you do. Repeatedly. Sometimes 20 times in a single `Program.fs` file.

    A small amount of cleverness with custom operations (and a `#nowarn "20"`) really does work magic:

    ```fsharp
    services (fun s -> s.AddScoped<MyScopedService> ())
    ```

    The function that the `services` operation (or most of the others) takes can have any return type; it gets magically swallowed, because it doesn't matter. It shouldn't be so satisfying, but it is. Just try it the old way, piping it into `|> ignore` every time, and then come back.
