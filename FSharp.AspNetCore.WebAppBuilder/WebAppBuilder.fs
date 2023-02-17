namespace FSharp.AspNetCore.Builder

open System
open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.Logging

/// Creates a web application using computation expression syntax.
[<Sealed>]
type WebAppBuilder (args : string array) =
    member _.Yield _ = WebApplication.CreateBuilder args

    /// <summary>
    /// Applies the given action to the <see cref="T:Microsoft.AspNetCore.Builder.WebApplicationBuilder"/>
    /// being used to build the app.
    /// </summary>
    [<CustomOperation("builder")>]
    member _.Builder (builder : WebApplicationBuilder, f) =
        ignore (f builder)
        builder

    /// <summary>
    /// Applies the given action to the <see cref="P:Microsoft.AspNetCore.Builder.WebApplicationBuilder.Configuration"/>
    /// property of the <see cref="T:Microsoft.AspNetCore.Builder.WebApplicationBuilder"/> being used to build the app.
    /// </summary>
    [<CustomOperation("configuration")>]
    member _.Configuration (builder : WebApplicationBuilder, f) =
        ignore (f builder.Configuration)
        builder

    /// <summary>
    /// Applies the given action to the <see cref="P:Microsoft.AspNetCore.Builder.WebApplicationBuilder.Logging"/>
    /// property of the <see cref="T:Microsoft.AspNetCore.Builder.WebApplicationBuilder"/> being used to build the app.
    /// </summary>
    [<CustomOperation("logging")>]
    member _.Logging (builder : WebApplicationBuilder, f) =
        ignore (f builder.Logging)
        builder

    /// <summary>
    /// Applies the given action to the <see cref="P:Microsoft.AspNetCore.Builder.WebApplicationBuilder.Services"/>
    /// property of the <see cref="T:Microsoft.AspNetCore.Builder.WebApplicationBuilder"/> being used to build the app.
    /// </summary>
    [<CustomOperation("services")>]
    member _.Services (builder : WebApplicationBuilder, f) =
        ignore (f builder.Services)
        builder

    /// <summary>
    /// Applies the given action to the <see cref="P:Microsoft.AspNetCore.Builder.WebApplicationBuilder.Host"/>
    /// property of the <see cref="T:Microsoft.AspNetCore.Builder.WebApplicationBuilder"/> being used to build the app.
    /// </summary>
    [<CustomOperation("host")>]
    member _.Host (builder : WebApplicationBuilder, f) =
        ignore (f builder.Host)
        builder

    /// <summary>
    /// Applies the given action to the <see cref="P:Microsoft.AspNetCore.Builder.WebApplicationBuilder.WebHost"/>
    /// property of the <see cref="T:Microsoft.AspNetCore.Builder.WebApplicationBuilder"/> being used to build the app.
    /// </summary>
    [<CustomOperation("webHost")>]
    member _.WebHost (builder : WebApplicationBuilder, f) =
        ignore (f builder.WebHost)
        builder

    /// <summary>
    /// Adds the ability to read configuration from environment variables.
    /// </summary>
    [<CustomOperation("environmentVariables")>]
    member _.EnvironmentVariables (builder : WebApplicationBuilder) =
        ignore <| builder.Configuration.AddEnvironmentVariables ()
        builder

    /// <summary>
    /// Adds a required JSON configuration file to the <see cref="T:Microsoft.AspNetCore.Builder.WebApplicationBuilder"/>'s
    /// <see cref="P:Microsoft.AspNetCore.Builder.WebApplicationBuilder.Configuration"/>.
    /// </summary>
    [<CustomOperation("jsonFile")>]
    member _.JsonFile (builder : WebApplicationBuilder, path : string) =
        ignore <| builder.Configuration.AddJsonFile (path, optional=false)
        builder

    /// <summary>
    /// Adds an optional JSON configuration file to the <see cref="T:Microsoft.AspNetCore.Builder.WebApplicationBuilder"/>'s
    /// <see cref="P:Microsoft.AspNetCore.Builder.WebApplicationBuilder.Configuration"/>.
    /// </summary>
    [<CustomOperation("optionalJsonFile")>]
    member _.OptionalJsonFile (builder : WebApplicationBuilder, path : string) =
        ignore <| builder.Configuration.AddJsonFile (path, optional=true)
        builder

    [<CustomOperation("consoleLogging")>]
    member _.ConsoleLogging (builder : WebApplicationBuilder) =
        ignore <| builder.Logging.AddConsole ()

    /// <summary>
    /// Adds a singleton service of the type specified in <paramref name="serviceType"/> to the
    /// app's service collection.
    /// </summary>
    [<CustomOperation("singleton")>]
    member _.Singleton (builder : WebApplicationBuilder, serviceType : Type) =
        ignore <| builder.Services.AddSingleton serviceType
        builder

    /// <summary>
    /// Adds a singleton service of the type specified in <paramref name="serviceType"/> with
    /// an implementation of the type specified in <paramref name="implementationType"/> to the
    /// app's service collection.
    /// </summary>
    [<CustomOperation("singleton")>]
    member _.Singleton (builder : WebApplicationBuilder, serviceType : Type, implementationType : Type) =
        ignore <| builder.Services.AddSingleton (serviceType, implementationType)
        builder

    /// <summary>
    /// Adds a singleton service of the type specified in <paramref name="serviceType"/> using
    /// an implementation provided by applying the given <paramref name="implementationFactory"/> to the
    /// app's service provider.
    /// </summary>
    [<CustomOperation("singleton")>]
    member _.Singleton (builder : WebApplicationBuilder, serviceType : Type, implementationFactory : IServiceProvider -> 'TImplementation) =
        ignore <| builder.Services.AddSingleton (serviceType=serviceType, implementationFactory=(implementationFactory >> box))
        builder

    /// <summary>
    /// Adds a singleton service of the type specified in <paramref name="serviceType"/> using
    /// the given <paramref name="implementationInstance"/>.
    /// </summary>
    [<CustomOperation("singleton")>]
    member _.Singleton (builder : WebApplicationBuilder, serviceType : Type, implementationInstance : obj) =
        ignore <| builder.Services.AddSingleton (serviceType=serviceType, implementationInstance=implementationInstance)
        builder

    /// <summary>
    /// Adds a singleton service using
    /// the given <paramref name="implementationInstance"/>.
    /// </summary>
    [<CustomOperation("singleton")>]
    member _.Singleton (builder : WebApplicationBuilder, implementationInstance : 'TService) =
        ignore <| builder.Services.AddSingleton<'TService> (implementationInstance=implementationInstance)
        builder

    /// <summary>
    /// Adds a singleton service produced by applying the given <paramref name="configure"/> function
    /// to the <see cref="T:Microsoft.AspNetCore.Builder.WebApplicationBuilder"/>'s
    /// <see cref="P:Microsoft.AspNetCore.Builder.WebApplicationBuilder.Configuration"/> property.
    /// </summary>
    [<CustomOperation("configure")>]
    member _.Configure (builder : WebApplicationBuilder, configure : IConfiguration -> 'ConfiguredValue when 'ConfiguredValue : not struct) =
        ignore <| builder.Services.AddSingleton (implementationInstance=configure builder.Configuration)
        builder

    /// <summary>
    /// Adds a strongly-typed connection string to the app's service collection.
    /// </summary>
    [<CustomOperation("connectionString")>]
    member this.ConnectionString (builder : WebApplicationBuilder, name : string, ctor : string -> 'ConnectionString when 'ConnectionString : not struct) =
        this.Configure (builder, fun config ->
            let connectionString = config.GetConnectionString name

            if String.IsNullOrWhiteSpace connectionString then
                invalidOp $"No connection string '{name}' found."

            ctor connectionString)

    /// <summary>
    /// Adds a strongly-typed configuration value to the app's service collection.
    /// </summary>
    [<CustomOperation("configurationValue")>]
    member this.ConfigurationValue (builder : WebApplicationBuilder, key : string, ctor : 'Value -> 'ConfiguredValue when 'ConfiguredValue : not struct) =
        this.Configure (builder, fun config ->
            let value = config.GetRequiredSection key

            if String.IsNullOrWhiteSpace value.Value then
                invalidOp $"No value found for key '{key}'."

            ctor (value.Get<'Value> ()))

    /// Adds a hosted service.
    [<CustomOperation("hostedService")>]
    member _.HostedService (builder : WebApplicationBuilder, serviceType : Type) =
        if not (serviceType.IsAssignableTo typeof<IHostedService>) then
            invalidOp $"The type '{serviceType.Name}' does not implement {nameof IHostedService}."

        ignore <| builder.Services.AddSingleton (typeof<IHostedService>, serviceType)
        builder

    /// Adds a hosted service.
    [<CustomOperation("hostedService")>]
    member _.HostedService (builder : WebApplicationBuilder, svc) =
        ignore <| builder.Services.AddHostedService (fun sp -> svc sp)
        builder

    /// <summary>
    /// Calls <see cref="M:Microsoft.AspNetCore.Builder.WebApplicationBuilder.Build"/> on the web application builder,
    /// turning it into a <see cref="T:Microsoft.AspNetCore.Builder.WebApplication"/>.
    /// </summary>
    [<CustomOperation("build")>]
    member _.Build (builder : WebApplicationBuilder, ()) =
        builder.Build ()

    /// <summary>
    /// Applies the given function to the web application builder
    /// immediately before calling <see cref="M:Microsoft.AspNetCore.Builder.WebApplicationBuilder.Build"/> on it,
    /// turning it into a <see cref="T:Microsoft.AspNetCore.Builder.WebApplication"/>.
    /// </summary>
    [<CustomOperation("buildWith")>]
    member this.BuildWith (builder : WebApplicationBuilder, f) =
        this.Build (this.Builder (builder, f), ())

    /// <summary>
    /// Applies the given action to the built <see cref="T:Microsoft.AspNetCore.Builder.WebApplication"/>.
    /// </summary>
    [<CustomOperation("webApp")>]
    member _.WebApp (app : WebApplication, f) =
        ignore (f app)
        app

    /// <summary>
    /// Calls <see cref="M:Microsoft.AspNetCore.Builder.WebApplicationBuilder.Build"/> and
    /// applies the given action to the resulting <see cref="T:Microsoft.AspNetCore.Builder.WebApplication"/>.
    /// </summary>
    [<CustomOperation("webApp")>]
    member this.WebApp (builder : WebApplicationBuilder, f) = this.WebApp (builder.Build (), f)

    member this.Run (builder : WebApplicationBuilder) = this.Run (builder.Build ())

    member _.Run (app : WebApplication) = app

type Pattern = string

module internal Pattern =
    open System.Globalization

    let toGroupName (pattern : Pattern) =
        if isNull pattern then nullArg (nameof pattern) else
        pattern.Split ('/', StringSplitOptions.RemoveEmptyEntries)
        |> Array.tryFindBack (not << String.exists (function '{' | '}' -> true | _ -> false))
        |> Option.bind (fun last -> last.Split ('?', StringSplitOptions.RemoveEmptyEntries) |> Array.tryHead)
        |> Option.map (fun groupName -> if groupName |> String.exists Char.IsUpper then CultureInfo.CurrentCulture.TextInfo.ToTitleCase groupName else groupName)
        |> Option.toObj

open Microsoft.AspNetCore.Http

module internal RouteHandlerBuilder =
    let withTag (tag : string) (routeHandlerBuilder : RouteHandlerBuilder) = routeHandlerBuilder.WithTags tag

    let produces produces (routeHandlerBuilder : RouteHandlerBuilder) =
        (routeHandlerBuilder, produces) ||> List.fold (fun route (statusCode, responseTy) ->
            route.Produces (statusCode=statusCode, responseType=responseTy, contentType=null, additionalContentTypes=[||]))

[<AutoOpen>]
module Gets =
    type WebAppBuilder with
        /// Adds an HTTP GET endpoint.
        [<CustomOperation("get")>]
        member _.Get (app : WebApplication, pattern : Pattern, handler : Func<'a, 'b>, ?configure : RouteHandlerBuilder -> _) =
            let handler : Delegate =
                if typeof<'a>.Equals typeof<unit> then upcast Func<'b> (fun () -> (Unchecked.unbox<Func<unit, 'b>> (box handler)).Invoke ())
                else upcast handler

            app.MapGet (pattern, handler=handler)
            |> RouteHandlerBuilder.withTag (Pattern.toGroupName pattern)
            |> defaultArg (configure |> Option.map ((<<) ignore)) ignore

            app

        /// Adds an HTTP GET endpoint.
        [<CustomOperation("get")>]
        member _.Get (app : WebApplication, pattern : Pattern, handler : Func<_, _, _>, ?configure : RouteHandlerBuilder -> _) =
            app.MapGet (pattern, handler=handler)
            |> RouteHandlerBuilder.withTag (Pattern.toGroupName pattern)
            |> defaultArg (configure |> Option.map ((<<) ignore)) ignore
            app

        /// Adds an HTTP GET endpoint.
        [<CustomOperation("get")>]
        member _.Get (app : WebApplication, pattern : Pattern, handler : Func<_, _, _, _>, ?configure : RouteHandlerBuilder -> _) =
            app.MapGet (pattern, handler=handler)
            |> RouteHandlerBuilder.withTag (Pattern.toGroupName pattern)
            |> defaultArg (configure |> Option.map ((<<) ignore)) ignore
            app

        /// Adds an HTTP GET endpoint.
        [<CustomOperation("get")>]
        member _.Get (app : WebApplication, pattern : Pattern, handler : Func<_, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) =
            app.MapGet (pattern, handler=handler)
            |> RouteHandlerBuilder.withTag (Pattern.toGroupName pattern)
            |> defaultArg (configure |> Option.map ((<<) ignore)) ignore
            app

        /// Adds an HTTP GET endpoint.
        [<CustomOperation("get")>]
        member _.Get (app : WebApplication, pattern : Pattern, handler : Func<_, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) =
            app.MapGet (pattern, handler=handler)
            |> RouteHandlerBuilder.withTag (Pattern.toGroupName pattern)
            |> defaultArg (configure |> Option.map ((<<) ignore)) ignore
            app

        /// Adds an HTTP GET endpoint.
        [<CustomOperation("get")>]
        member _.Get (app : WebApplication, pattern : Pattern, handler : Func<_, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) =
            app.MapGet (pattern, handler=handler)
            |> RouteHandlerBuilder.withTag (Pattern.toGroupName pattern)
            |> defaultArg (configure |> Option.map ((<<) ignore)) ignore
            app

        /// Adds an HTTP GET endpoint.
        [<CustomOperation("get")>]
        member _.Get (app : WebApplication, pattern : Pattern, handler : Func<_, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) =
            app.MapGet (pattern, handler=handler)
            |> RouteHandlerBuilder.withTag (Pattern.toGroupName pattern)
            |> defaultArg (configure |> Option.map ((<<) ignore)) ignore
            app

        /// Adds an HTTP GET endpoint.
        [<CustomOperation("get")>]
        member _.Get (app : WebApplication, pattern : Pattern, handler : Func<_, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) =
            app.MapGet (pattern, handler=handler)
            |> RouteHandlerBuilder.withTag (Pattern.toGroupName pattern)
            |> defaultArg (configure |> Option.map ((<<) ignore)) ignore
            app

        /// Adds an HTTP GET endpoint.
        [<CustomOperation("get")>]
        member _.Get (app : WebApplication, pattern : Pattern, handler : Func<_, _, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) =
            app.MapGet (pattern, handler=handler)
            |> RouteHandlerBuilder.withTag (Pattern.toGroupName pattern)
            |> defaultArg (configure |> Option.map ((<<) ignore)) ignore
            app

        /// Adds an HTTP GET endpoint.
        [<CustomOperation("get")>]
        member _.Get (app : WebApplication, pattern : Pattern, handler : Func<_, _, _, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) =
            app.MapGet (pattern, handler=handler)
            |> RouteHandlerBuilder.withTag (Pattern.toGroupName pattern)
            |> defaultArg (configure |> Option.map ((<<) ignore)) ignore
            app

        /// Adds an HTTP GET endpoint.
        [<CustomOperation("get")>]
        member _.Get (app : WebApplication, pattern : Pattern, handler : Func<_, _, _, _, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) =
            app.MapGet (pattern, handler=handler)
            |> RouteHandlerBuilder.withTag (Pattern.toGroupName pattern)
            |> defaultArg (configure |> Option.map ((<<) ignore)) ignore
            app

        /// Adds an HTTP GET endpoint.
        [<CustomOperation("get")>]
        member _.Get (app : WebApplication, pattern : Pattern, handler : Func<_, _, _, _, _, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) =
            app.MapGet (pattern, handler=handler)
            |> RouteHandlerBuilder.withTag (Pattern.toGroupName pattern)
            |> defaultArg (configure |> Option.map ((<<) ignore)) ignore
            app

        /// Adds an HTTP GET endpoint.
        [<CustomOperation("get")>]
        member _.Get (app : WebApplication, pattern : Pattern, handler : Func<_, _, _, _, _, _, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) =
            app.MapGet (pattern, handler=handler)
            |> RouteHandlerBuilder.withTag (Pattern.toGroupName pattern)
            |> defaultArg (configure |> Option.map ((<<) ignore)) ignore
            app

        /// Adds an HTTP GET endpoint.
        [<CustomOperation("get")>]
        member _.Get (app : WebApplication, pattern : Pattern, handler : Func<_, _, _, _, _, _, _, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) =
            app.MapGet (pattern, handler=handler)
            |> RouteHandlerBuilder.withTag (Pattern.toGroupName pattern)
            |> defaultArg (configure |> Option.map ((<<) ignore)) ignore
            app

        /// Adds an HTTP GET endpoint.
        [<CustomOperation("get")>]
        member _.Get (app : WebApplication, pattern : Pattern, handler : Func<_, _, _, _, _, _, _, _, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) =
            app.MapGet (pattern, handler=handler)
            |> RouteHandlerBuilder.withTag (Pattern.toGroupName pattern)
            |> defaultArg (configure |> Option.map ((<<) ignore)) ignore
            app

        /// Adds an HTTP GET endpoint.
        [<CustomOperation("get")>]
        member _.Get (app : WebApplication, pattern : Pattern, handler : Func<_, _, _, _, _, _, _, _, _, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) =
            app.MapGet (pattern, handler=handler)
            |> RouteHandlerBuilder.withTag (Pattern.toGroupName pattern)
            |> defaultArg (configure |> Option.map ((<<) ignore)) ignore
            app

        /// Adds an HTTP GET endpoint.
        [<CustomOperation("get")>]
        member _.Get (app : WebApplication, pattern : Pattern, produces : (int * Type) list, handler : Func<'a, 'b>, ?configure : RouteHandlerBuilder -> _) =
            let handler : Delegate =
                if typeof<'a>.Equals typeof<unit> then upcast Func<'b> (fun () -> (Unchecked.unbox<Func<unit, 'b>> (box handler)).Invoke ())
                else upcast handler

            app.MapGet (pattern, handler=handler)
            |> RouteHandlerBuilder.withTag (Pattern.toGroupName pattern)
            |> RouteHandlerBuilder.produces produces
            |> defaultArg (configure |> Option.map ((<<) ignore)) ignore
            app

        /// Adds an HTTP GET endpoint.
        [<CustomOperation("get")>]
        member _.Get (app : WebApplication, pattern : Pattern, produces : (int * Type) list, handler : Func<_, _, _>, ?configure : RouteHandlerBuilder -> _) =
            app.MapGet (pattern, handler=handler)
            |> RouteHandlerBuilder.withTag (Pattern.toGroupName pattern)
            |> RouteHandlerBuilder.produces produces
            |> defaultArg (configure |> Option.map ((<<) ignore)) ignore
            app

        /// Adds an HTTP GET endpoint.
        [<CustomOperation("get")>]
        member _.Get (app : WebApplication, pattern : Pattern, produces : (int * Type) list, handler : Func<_, _, _, _>, ?configure : RouteHandlerBuilder -> _) =
            app.MapGet (pattern, handler=handler)
            |> RouteHandlerBuilder.withTag (Pattern.toGroupName pattern)
            |> RouteHandlerBuilder.produces produces
            |> defaultArg (configure |> Option.map ((<<) ignore)) ignore
            app

        /// Adds an HTTP GET endpoint.
        [<CustomOperation("get")>]
        member _.Get (app : WebApplication, pattern : Pattern, produces : (int * Type) list, handler : Func<_, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) =
            app.MapGet (pattern, handler=handler)
            |> RouteHandlerBuilder.withTag (Pattern.toGroupName pattern)
            |> RouteHandlerBuilder.produces produces
            |> defaultArg (configure |> Option.map ((<<) ignore)) ignore
            app

        /// Adds an HTTP GET endpoint.
        [<CustomOperation("get")>]
        member _.Get (app : WebApplication, pattern : Pattern, produces : (int * Type) list, handler : Func<_, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) =
            app.MapGet (pattern, handler=handler)
            |> RouteHandlerBuilder.withTag (Pattern.toGroupName pattern)
            |> RouteHandlerBuilder.produces produces
            |> defaultArg (configure |> Option.map ((<<) ignore)) ignore
            app

        /// Adds an HTTP GET endpoint.
        [<CustomOperation("get")>]
        member _.Get (app : WebApplication, pattern : Pattern, produces : (int * Type) list, handler : Func<_, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) =
            app.MapGet (pattern, handler=handler)
            |> RouteHandlerBuilder.withTag (Pattern.toGroupName pattern)
            |> RouteHandlerBuilder.produces produces
            |> defaultArg (configure |> Option.map ((<<) ignore)) ignore
            app

        /// Adds an HTTP GET endpoint.
        [<CustomOperation("get")>]
        member _.Get (app : WebApplication, pattern : Pattern, produces : (int * Type) list, handler : Func<_, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) =
            app.MapGet (pattern, handler=handler)
            |> RouteHandlerBuilder.withTag (Pattern.toGroupName pattern)
            |> RouteHandlerBuilder.produces produces
            |> defaultArg (configure |> Option.map ((<<) ignore)) ignore
            app

        /// Adds an HTTP GET endpoint.
        [<CustomOperation("get")>]
        member _.Get (app : WebApplication, pattern : Pattern, produces : (int * Type) list, handler : Func<_, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) =
            app.MapGet (pattern, handler=handler)
            |> RouteHandlerBuilder.withTag (Pattern.toGroupName pattern)
            |> RouteHandlerBuilder.produces produces
            |> defaultArg (configure |> Option.map ((<<) ignore)) ignore
            app

        /// Adds an HTTP GET endpoint.
        [<CustomOperation("get")>]
        member _.Get (app : WebApplication, pattern : Pattern, produces : (int * Type) list, handler : Func<_, _, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) =
            app.MapGet (pattern, handler=handler)
            |> RouteHandlerBuilder.withTag (Pattern.toGroupName pattern)
            |> RouteHandlerBuilder.produces produces
            |> defaultArg (configure |> Option.map ((<<) ignore)) ignore
            app

        /// Adds an HTTP GET endpoint.
        [<CustomOperation("get")>]
        member _.Get (app : WebApplication, pattern : Pattern, produces : (int * Type) list, handler : Func<_, _, _, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) =
            app.MapGet (pattern, handler=handler)
            |> RouteHandlerBuilder.withTag (Pattern.toGroupName pattern)
            |> RouteHandlerBuilder.produces produces
            |> defaultArg (configure |> Option.map ((<<) ignore)) ignore
            app

        /// Adds an HTTP GET endpoint.
        [<CustomOperation("get")>]
        member _.Get (app : WebApplication, pattern : Pattern, produces : (int * Type) list, handler : Func<_, _, _, _, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) =
            app.MapGet (pattern, handler=handler)
            |> RouteHandlerBuilder.withTag (Pattern.toGroupName pattern)
            |> RouteHandlerBuilder.produces produces
            |> defaultArg (configure |> Option.map ((<<) ignore)) ignore
            app

        /// Adds an HTTP GET endpoint.
        [<CustomOperation("get")>]
        member _.Get (app : WebApplication, pattern : Pattern, produces : (int * Type) list, handler : Func<_, _, _, _, _, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) =
            app.MapGet (pattern, handler=handler)
            |> RouteHandlerBuilder.withTag (Pattern.toGroupName pattern)
            |> RouteHandlerBuilder.produces produces
            |> defaultArg (configure |> Option.map ((<<) ignore)) ignore
            app

        /// Adds an HTTP GET endpoint.
        [<CustomOperation("get")>]
        member _.Get (app : WebApplication, pattern : Pattern, produces : (int * Type) list, handler : Func<_, _, _, _, _, _, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) =
            app.MapGet (pattern, handler=handler)
            |> RouteHandlerBuilder.withTag (Pattern.toGroupName pattern)
            |> RouteHandlerBuilder.produces produces
            |> defaultArg (configure |> Option.map ((<<) ignore)) ignore
            app

        /// Adds an HTTP GET endpoint.
        [<CustomOperation("get")>]
        member _.Get (app : WebApplication, pattern : Pattern, produces : (int * Type) list, handler : Func<_, _, _, _, _, _, _, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) =
            app.MapGet (pattern, handler=handler)
            |> RouteHandlerBuilder.withTag (Pattern.toGroupName pattern)
            |> RouteHandlerBuilder.produces produces
            |> defaultArg (configure |> Option.map ((<<) ignore)) ignore
            app

        /// Adds an HTTP GET endpoint.
        [<CustomOperation("get")>]
        member _.Get (app : WebApplication, pattern : Pattern, produces : (int * Type) list, handler : Func<_, _, _, _, _, _, _, _, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) =
            app.MapGet (pattern, handler=handler)
            |> RouteHandlerBuilder.withTag (Pattern.toGroupName pattern)
            |> RouteHandlerBuilder.produces produces
            |> defaultArg (configure |> Option.map ((<<) ignore)) ignore
            app

        /// Adds an HTTP GET endpoint.
        [<CustomOperation("get")>]
        member _.Get (app : WebApplication, pattern : Pattern, produces : (int * Type) list, handler : Func<_, _, _, _, _, _, _, _, _, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) =
            app.MapGet (pattern, handler=handler)
            |> RouteHandlerBuilder.withTag (Pattern.toGroupName pattern)
            |> RouteHandlerBuilder.produces produces
            |> defaultArg (configure |> Option.map ((<<) ignore)) ignore
            app

        /// Adds an HTTP GET endpoint.
        [<CustomOperation("get")>]
        member this.Get (builder : WebApplicationBuilder, pattern : Pattern, handler : Func<_, _>, ?configure : RouteHandlerBuilder -> _) = this.Get (builder.Build (), pattern, handler)

        /// Adds an HTTP GET endpoint.
        [<CustomOperation("get")>]
        member this.Get (builder : WebApplicationBuilder, pattern : Pattern, handler : Func<_, _, _>, ?configure : RouteHandlerBuilder -> _) = this.Get (builder.Build (), pattern, handler)

        /// Adds an HTTP GET endpoint.
        [<CustomOperation("get")>]
        member this.Get (builder : WebApplicationBuilder, pattern : Pattern, handler : Func<_, _, _, _>, ?configure : RouteHandlerBuilder -> _) = this.Get (builder.Build (), pattern, handler)

        /// Adds an HTTP GET endpoint.
        [<CustomOperation("get")>]
        member this.Get (builder : WebApplicationBuilder, pattern : Pattern, handler : Func<_, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) = this.Get (builder.Build (), pattern, handler)

        /// Adds an HTTP GET endpoint.
        [<CustomOperation("get")>]
        member this.Get (builder : WebApplicationBuilder, pattern : Pattern, handler : Func<_, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) = this.Get (builder.Build (), pattern, handler)

        /// Adds an HTTP GET endpoint.
        [<CustomOperation("get")>]
        member this.Get (builder : WebApplicationBuilder, pattern : Pattern, handler : Func<_, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) = this.Get (builder.Build (), pattern, handler)

        /// Adds an HTTP GET endpoint.
        [<CustomOperation("get")>]
        member this.Get (builder : WebApplicationBuilder, pattern : Pattern, handler : Func<_, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) = this.Get (builder.Build (), pattern, handler)

        /// Adds an HTTP GET endpoint.
        [<CustomOperation("get")>]
        member this.Get (builder : WebApplicationBuilder, pattern : Pattern, handler : Func<_, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) = this.Get (builder.Build (), pattern, handler)

        /// Adds an HTTP GET endpoint.
        [<CustomOperation("get")>]
        member this.Get (builder : WebApplicationBuilder, pattern : Pattern, handler : Func<_, _, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) = this.Get (builder.Build (), pattern, handler)

        /// Adds an HTTP GET endpoint.
        [<CustomOperation("get")>]
        member this.Get (builder : WebApplicationBuilder, pattern : Pattern, handler : Func<_, _, _, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) = this.Get (builder.Build (), pattern, handler)

        /// Adds an HTTP GET endpoint.
        [<CustomOperation("get")>]
        member this.Get (builder : WebApplicationBuilder, pattern : Pattern, handler : Func<_, _, _, _, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) = this.Get (builder.Build (), pattern, handler)

        /// Adds an HTTP GET endpoint.
        [<CustomOperation("get")>]
        member this.Get (builder : WebApplicationBuilder, pattern : Pattern, handler : Func<_, _, _, _, _, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) = this.Get (builder.Build (), pattern, handler)

        /// Adds an HTTP GET endpoint.
        [<CustomOperation("get")>]
        member this.Get (builder : WebApplicationBuilder, pattern : Pattern, handler : Func<_, _, _, _, _, _, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) = this.Get (builder.Build (), pattern, handler)

        /// Adds an HTTP GET endpoint.
        [<CustomOperation("get")>]
        member this.Get (builder : WebApplicationBuilder, pattern : Pattern, handler : Func<_, _, _, _, _, _, _, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) = this.Get (builder.Build (), pattern, handler)

        /// Adds an HTTP GET endpoint.
        [<CustomOperation("get")>]
        member this.Get (builder : WebApplicationBuilder, pattern : Pattern, handler : Func<_, _, _, _, _, _, _, _, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) = this.Get (builder.Build (), pattern, handler)

        /// Adds an HTTP GET endpoint.
        [<CustomOperation("get")>]
        member this.Get (builder : WebApplicationBuilder, pattern : Pattern, handler : Func<_, _, _, _, _, _, _, _, _, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) = this.Get (builder.Build (), pattern, handler)

        /// Adds an HTTP GET endpoint.
        [<CustomOperation("get")>]
        member this.Get (builder : WebApplicationBuilder, pattern : Pattern, produces : (int * Type) list, handler : Func<_, _>, ?configure : RouteHandlerBuilder -> _) = this.Get (builder.Build (), pattern, produces, handler)

        /// Adds an HTTP GET endpoint.
        [<CustomOperation("get")>]
        member this.Get (builder : WebApplicationBuilder, pattern : Pattern, produces : (int * Type) list, handler : Func<_, _, _>, ?configure : RouteHandlerBuilder -> _) = this.Get (builder.Build (), pattern, produces, handler)

        /// Adds an HTTP GET endpoint.
        [<CustomOperation("get")>]
        member this.Get (builder : WebApplicationBuilder, pattern : Pattern, produces : (int * Type) list, handler : Func<_, _, _, _>, ?configure : RouteHandlerBuilder -> _) = this.Get (builder.Build (), pattern, produces, handler)

        /// Adds an HTTP GET endpoint.
        [<CustomOperation("get")>]
        member this.Get (builder : WebApplicationBuilder, pattern : Pattern, produces : (int * Type) list, handler : Func<_, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) = this.Get (builder.Build (), pattern, produces, handler)

        /// Adds an HTTP GET endpoint.
        [<CustomOperation("get")>]
        member this.Get (builder : WebApplicationBuilder, pattern : Pattern, produces : (int * Type) list, handler : Func<_, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) = this.Get (builder.Build (), pattern, produces, handler)

        /// Adds an HTTP GET endpoint.
        [<CustomOperation("get")>]
        member this.Get (builder : WebApplicationBuilder, pattern : Pattern, produces : (int * Type) list, handler : Func<_, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) = this.Get (builder.Build (), pattern, produces, handler)

        /// Adds an HTTP GET endpoint.
        [<CustomOperation("get")>]
        member this.Get (builder : WebApplicationBuilder, pattern : Pattern, produces : (int * Type) list, handler : Func<_, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) = this.Get (builder.Build (), pattern, produces, handler)

        /// Adds an HTTP GET endpoint.
        [<CustomOperation("get")>]
        member this.Get (builder : WebApplicationBuilder, pattern : Pattern, produces : (int * Type) list, handler : Func<_, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) = this.Get (builder.Build (), pattern, produces, handler)

        /// Adds an HTTP GET endpoint.
        [<CustomOperation("get")>]
        member this.Get (builder : WebApplicationBuilder, pattern : Pattern, produces : (int * Type) list, handler : Func<_, _, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) = this.Get (builder.Build (), pattern, produces, handler)

        /// Adds an HTTP GET endpoint.
        [<CustomOperation("get")>]
        member this.Get (builder : WebApplicationBuilder, pattern : Pattern, produces : (int * Type) list, handler : Func<_, _, _, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) = this.Get (builder.Build (), pattern, produces, handler)

        /// Adds an HTTP GET endpoint.
        [<CustomOperation("get")>]
        member this.Get (builder : WebApplicationBuilder, pattern : Pattern, produces : (int * Type) list, handler : Func<_, _, _, _, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) = this.Get (builder.Build (), pattern, produces, handler)

        /// Adds an HTTP GET endpoint.
        [<CustomOperation("get")>]
        member this.Get (builder : WebApplicationBuilder, pattern : Pattern, produces : (int * Type) list, handler : Func<_, _, _, _, _, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) = this.Get (builder.Build (), pattern, produces, handler)

        /// Adds an HTTP GET endpoint.
        [<CustomOperation("get")>]
        member this.Get (builder : WebApplicationBuilder, pattern : Pattern, produces : (int * Type) list, handler : Func<_, _, _, _, _, _, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) = this.Get (builder.Build (), pattern, produces, handler)

        /// Adds an HTTP GET endpoint.
        [<CustomOperation("get")>]
        member this.Get (builder : WebApplicationBuilder, pattern : Pattern, produces : (int * Type) list, handler : Func<_, _, _, _, _, _, _, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) = this.Get (builder.Build (), pattern, produces, handler)

        /// Adds an HTTP GET endpoint.
        [<CustomOperation("get")>]
        member this.Get (builder : WebApplicationBuilder, pattern : Pattern, produces : (int * Type) list, handler : Func<_, _, _, _, _, _, _, _, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) = this.Get (builder.Build (), pattern, produces, handler)

        /// Adds an HTTP GET endpoint.
        [<CustomOperation("get")>]
        member this.Get (builder : WebApplicationBuilder, pattern : Pattern, produces : (int * Type) list, handler : Func<_, _, _, _, _, _, _, _, _, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) = this.Get (builder.Build (), pattern, produces, handler)

[<AutoOpen>]
module Posts =
    type WebAppBuilder with
        /// Adds an HTTP POST endpoint.
        [<CustomOperation("post")>]
        member _.Post (app : WebApplication, pattern : Pattern, handler : Func<'a, 'b>, ?configure : RouteHandlerBuilder -> _) =
            let handler : Delegate =
                if typeof<'a>.Equals typeof<unit> then upcast Func<'b> (fun () -> (Unchecked.unbox<Func<unit, 'b>> (box handler)).Invoke ())
                else upcast handler

            app.MapPost (pattern, handler=handler)
            |> RouteHandlerBuilder.withTag (Pattern.toGroupName pattern)
            |> defaultArg (configure |> Option.map ((<<) ignore)) ignore

            app

        /// Adds an HTTP POST endpoint.
        [<CustomOperation("post")>]
        member _.Post (app : WebApplication, pattern : Pattern, handler : Func<_, _, _>, ?configure : RouteHandlerBuilder -> _) =
            app.MapPost (pattern, handler=handler)
            |> RouteHandlerBuilder.withTag (Pattern.toGroupName pattern)
            |> defaultArg (configure |> Option.map ((<<) ignore)) ignore
            app

        /// Adds an HTTP POST endpoint.
        [<CustomOperation("post")>]
        member _.Post (app : WebApplication, pattern : Pattern, handler : Func<_, _, _, _>, ?configure : RouteHandlerBuilder -> _) =
            app.MapPost (pattern, handler=handler)
            |> RouteHandlerBuilder.withTag (Pattern.toGroupName pattern)
            |> defaultArg (configure |> Option.map ((<<) ignore)) ignore
            app

        /// Adds an HTTP POST endpoint.
        [<CustomOperation("post")>]
        member _.Post (app : WebApplication, pattern : Pattern, handler : Func<_, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) =
            app.MapPost (pattern, handler=handler)
            |> RouteHandlerBuilder.withTag (Pattern.toGroupName pattern)
            |> defaultArg (configure |> Option.map ((<<) ignore)) ignore
            app

        /// Adds an HTTP POST endpoint.
        [<CustomOperation("post")>]
        member _.Post (app : WebApplication, pattern : Pattern, handler : Func<_, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) =
            app.MapPost (pattern, handler=handler)
            |> RouteHandlerBuilder.withTag (Pattern.toGroupName pattern)
            |> defaultArg (configure |> Option.map ((<<) ignore)) ignore
            app

        /// Adds an HTTP POST endpoint.
        [<CustomOperation("post")>]
        member _.Post (app : WebApplication, pattern : Pattern, handler : Func<_, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) =
            app.MapPost (pattern, handler=handler)
            |> RouteHandlerBuilder.withTag (Pattern.toGroupName pattern)
            |> defaultArg (configure |> Option.map ((<<) ignore)) ignore
            app

        /// Adds an HTTP POST endpoint.
        [<CustomOperation("post")>]
        member _.Post (app : WebApplication, pattern : Pattern, handler : Func<_, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) =
            app.MapPost (pattern, handler=handler)
            |> RouteHandlerBuilder.withTag (Pattern.toGroupName pattern)
            |> defaultArg (configure |> Option.map ((<<) ignore)) ignore
            app

        /// Adds an HTTP POST endpoint.
        [<CustomOperation("post")>]
        member _.Post (app : WebApplication, pattern : Pattern, handler : Func<_, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) =
            app.MapPost (pattern, handler=handler)
            |> RouteHandlerBuilder.withTag (Pattern.toGroupName pattern)
            |> defaultArg (configure |> Option.map ((<<) ignore)) ignore
            app

        /// Adds an HTTP POST endpoint.
        [<CustomOperation("post")>]
        member _.Post (app : WebApplication, pattern : Pattern, handler : Func<_, _, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) =
            app.MapPost (pattern, handler=handler)
            |> RouteHandlerBuilder.withTag (Pattern.toGroupName pattern)
            |> defaultArg (configure |> Option.map ((<<) ignore)) ignore
            app

        /// Adds an HTTP POST endpoint.
        [<CustomOperation("post")>]
        member _.Post (app : WebApplication, pattern : Pattern, handler : Func<_, _, _, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) =
            app.MapPost (pattern, handler=handler)
            |> RouteHandlerBuilder.withTag (Pattern.toGroupName pattern)
            |> defaultArg (configure |> Option.map ((<<) ignore)) ignore
            app

        /// Adds an HTTP POST endpoint.
        [<CustomOperation("post")>]
        member _.Post (app : WebApplication, pattern : Pattern, handler : Func<_, _, _, _, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) =
            app.MapPost (pattern, handler=handler)
            |> RouteHandlerBuilder.withTag (Pattern.toGroupName pattern)
            |> defaultArg (configure |> Option.map ((<<) ignore)) ignore
            app

        /// Adds an HTTP POST endpoint.
        [<CustomOperation("post")>]
        member _.Post (app : WebApplication, pattern : Pattern, handler : Func<_, _, _, _, _, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) =
            app.MapPost (pattern, handler=handler)
            |> RouteHandlerBuilder.withTag (Pattern.toGroupName pattern)
            |> defaultArg (configure |> Option.map ((<<) ignore)) ignore
            app

        /// Adds an HTTP POST endpoint.
        [<CustomOperation("post")>]
        member _.Post (app : WebApplication, pattern : Pattern, handler : Func<_, _, _, _, _, _, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) =
            app.MapPost (pattern, handler=handler)
            |> RouteHandlerBuilder.withTag (Pattern.toGroupName pattern)
            |> defaultArg (configure |> Option.map ((<<) ignore)) ignore
            app

        /// Adds an HTTP POST endpoint.
        [<CustomOperation("post")>]
        member _.Post (app : WebApplication, pattern : Pattern, handler : Func<_, _, _, _, _, _, _, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) =
            app.MapPost (pattern, handler=handler)
            |> RouteHandlerBuilder.withTag (Pattern.toGroupName pattern)
            |> defaultArg (configure |> Option.map ((<<) ignore)) ignore
            app

        /// Adds an HTTP POST endpoint.
        [<CustomOperation("post")>]
        member _.Post (app : WebApplication, pattern : Pattern, handler : Func<_, _, _, _, _, _, _, _, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) =
            app.MapPost (pattern, handler=handler)
            |> RouteHandlerBuilder.withTag (Pattern.toGroupName pattern)
            |> defaultArg (configure |> Option.map ((<<) ignore)) ignore
            app

        /// Adds an HTTP POST endpoint.
        [<CustomOperation("post")>]
        member _.Post (app : WebApplication, pattern : Pattern, handler : Func<_, _, _, _, _, _, _, _, _, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) =
            app.MapPost (pattern, handler=handler)
            |> RouteHandlerBuilder.withTag (Pattern.toGroupName pattern)
            |> defaultArg (configure |> Option.map ((<<) ignore)) ignore
            app

        /// Adds an HTTP POST endpoint.
        [<CustomOperation("post")>]
        member _.Post (app : WebApplication, pattern : Pattern, produces : (int * Type) list, handler : Func<'a, 'b>, ?configure : RouteHandlerBuilder -> _) =
            let handler : Delegate =
                if typeof<'a>.Equals typeof<unit> then upcast Func<'b> (fun () -> (Unchecked.unbox<Func<unit, 'b>> (box handler)).Invoke ())
                else upcast handler

            app.MapPost (pattern, handler=handler)
            |> RouteHandlerBuilder.withTag (Pattern.toGroupName pattern)
            |> RouteHandlerBuilder.produces produces
            |> defaultArg (configure |> Option.map ((<<) ignore)) ignore

            app

        /// Adds an HTTP POST endpoint.
        [<CustomOperation("post")>]
        member _.Post (app : WebApplication, pattern : Pattern, produces : (int * Type) list, handler : Func<_, _, _>, ?configure : RouteHandlerBuilder -> _) =
            app.MapPost (pattern, handler=handler)
            |> RouteHandlerBuilder.withTag (Pattern.toGroupName pattern)
            |> RouteHandlerBuilder.produces produces
            |> defaultArg (configure |> Option.map ((<<) ignore)) ignore
            app

        /// Adds an HTTP POST endpoint.
        [<CustomOperation("post")>]
        member _.Post (app : WebApplication, pattern : Pattern, produces : (int * Type) list, handler : Func<_, _, _, _>, ?configure : RouteHandlerBuilder -> _) =
            app.MapPost (pattern, handler=handler)
            |> RouteHandlerBuilder.withTag (Pattern.toGroupName pattern)
            |> RouteHandlerBuilder.produces produces
            |> defaultArg (configure |> Option.map ((<<) ignore)) ignore
            app

        /// Adds an HTTP POST endpoint.
        [<CustomOperation("post")>]
        member _.Post (app : WebApplication, pattern : Pattern, produces : (int * Type) list, handler : Func<_, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) =
            app.MapPost (pattern, handler=handler)
            |> RouteHandlerBuilder.withTag (Pattern.toGroupName pattern)
            |> RouteHandlerBuilder.produces produces
            |> defaultArg (configure |> Option.map ((<<) ignore)) ignore
            app

        /// Adds an HTTP POST endpoint.
        [<CustomOperation("post")>]
        member _.Post (app : WebApplication, pattern : Pattern, produces : (int * Type) list, handler : Func<_, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) =
            app.MapPost (pattern, handler=handler)
            |> RouteHandlerBuilder.withTag (Pattern.toGroupName pattern)
            |> RouteHandlerBuilder.produces produces
            |> defaultArg (configure |> Option.map ((<<) ignore)) ignore
            app

        /// Adds an HTTP POST endpoint.
        [<CustomOperation("post")>]
        member _.Post (app : WebApplication, pattern : Pattern, produces : (int * Type) list, handler : Func<_, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) =
            app.MapPost (pattern, handler=handler)
            |> RouteHandlerBuilder.withTag (Pattern.toGroupName pattern)
            |> RouteHandlerBuilder.produces produces
            |> defaultArg (configure |> Option.map ((<<) ignore)) ignore
            app

        /// Adds an HTTP POST endpoint.
        [<CustomOperation("post")>]
        member _.Post (app : WebApplication, pattern : Pattern, produces : (int * Type) list, handler : Func<_, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) =
            app.MapPost (pattern, handler=handler)
            |> RouteHandlerBuilder.withTag (Pattern.toGroupName pattern)
            |> RouteHandlerBuilder.produces produces
            |> defaultArg (configure |> Option.map ((<<) ignore)) ignore
            app

        /// Adds an HTTP POST endpoint.
        [<CustomOperation("post")>]
        member _.Post (app : WebApplication, pattern : Pattern, produces : (int * Type) list, handler : Func<_, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) =
            app.MapPost (pattern, handler=handler)
            |> RouteHandlerBuilder.withTag (Pattern.toGroupName pattern)
            |> RouteHandlerBuilder.produces produces
            |> defaultArg (configure |> Option.map ((<<) ignore)) ignore
            app

        /// Adds an HTTP POST endpoint.
        [<CustomOperation("post")>]
        member _.Post (app : WebApplication, pattern : Pattern, produces : (int * Type) list, handler : Func<_, _, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) =
            app.MapPost (pattern, handler=handler)
            |> RouteHandlerBuilder.withTag (Pattern.toGroupName pattern)
            |> RouteHandlerBuilder.produces produces
            |> defaultArg (configure |> Option.map ((<<) ignore)) ignore
            app

        /// Adds an HTTP POST endpoint.
        [<CustomOperation("post")>]
        member _.Post (app : WebApplication, pattern : Pattern, produces : (int * Type) list, handler : Func<_, _, _, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) =
            app.MapPost (pattern, handler=handler)
            |> RouteHandlerBuilder.withTag (Pattern.toGroupName pattern)
            |> RouteHandlerBuilder.produces produces
            |> defaultArg (configure |> Option.map ((<<) ignore)) ignore
            app

        /// Adds an HTTP POST endpoint.
        [<CustomOperation("post")>]
        member _.Post (app : WebApplication, pattern : Pattern, produces : (int * Type) list, handler : Func<_, _, _, _, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) =
            app.MapPost (pattern, handler=handler)
            |> RouteHandlerBuilder.withTag (Pattern.toGroupName pattern)
            |> RouteHandlerBuilder.produces produces
            |> defaultArg (configure |> Option.map ((<<) ignore)) ignore
            app

        /// Adds an HTTP POST endpoint.
        [<CustomOperation("post")>]
        member _.Post (app : WebApplication, pattern : Pattern, produces : (int * Type) list, handler : Func<_, _, _, _, _, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) =
            app.MapPost (pattern, handler=handler)
            |> RouteHandlerBuilder.withTag (Pattern.toGroupName pattern)
            |> RouteHandlerBuilder.produces produces
            |> defaultArg (configure |> Option.map ((<<) ignore)) ignore
            app

        /// Adds an HTTP POST endpoint.
        [<CustomOperation("post")>]
        member _.Post (app : WebApplication, pattern : Pattern, produces : (int * Type) list, handler : Func<_, _, _, _, _, _, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) =
            app.MapPost (pattern, handler=handler)
            |> RouteHandlerBuilder.withTag (Pattern.toGroupName pattern)
            |> RouteHandlerBuilder.produces produces
            |> defaultArg (configure |> Option.map ((<<) ignore)) ignore
            app

        /// Adds an HTTP POST endpoint.
        [<CustomOperation("post")>]
        member _.Post (app : WebApplication, pattern : Pattern, produces : (int * Type) list, handler : Func<_, _, _, _, _, _, _, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) =
            app.MapPost (pattern, handler=handler)
            |> RouteHandlerBuilder.withTag (Pattern.toGroupName pattern)
            |> RouteHandlerBuilder.produces produces
            |> defaultArg (configure |> Option.map ((<<) ignore)) ignore
            app

        /// Adds an HTTP POST endpoint.
        [<CustomOperation("post")>]
        member _.Post (app : WebApplication, pattern : Pattern, produces : (int * Type) list, handler : Func<_, _, _, _, _, _, _, _, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) =
            app.MapPost (pattern, handler=handler)
            |> RouteHandlerBuilder.withTag (Pattern.toGroupName pattern)
            |> RouteHandlerBuilder.produces produces
            |> defaultArg (configure |> Option.map ((<<) ignore)) ignore
            app

        /// Adds an HTTP POST endpoint.
        [<CustomOperation("post")>]
        member _.Post (app : WebApplication, pattern : Pattern, produces : (int * Type) list, handler : Func<_, _, _, _, _, _, _, _, _, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) =
            app.MapPost (pattern, handler=handler)
            |> RouteHandlerBuilder.withTag (Pattern.toGroupName pattern)
            |> RouteHandlerBuilder.produces produces
            |> defaultArg (configure |> Option.map ((<<) ignore)) ignore
            app

        /// Adds an HTTP POST endpoint.
        [<CustomOperation("post")>]
        member this.Post (builder : WebApplicationBuilder, pattern : Pattern, handler : Func<_, _>, ?configure : RouteHandlerBuilder -> _) = this.Post (builder.Build (), pattern, handler)

        /// Adds an HTTP POST endpoint.
        [<CustomOperation("post")>]
        member this.Post (builder : WebApplicationBuilder, pattern : Pattern, handler : Func<_, _, _>, ?configure : RouteHandlerBuilder -> _) = this.Post (builder.Build (), pattern, handler)

        /// Adds an HTTP POST endpoint.
        [<CustomOperation("post")>]
        member this.Post (builder : WebApplicationBuilder, pattern : Pattern, handler : Func<_, _, _, _>, ?configure : RouteHandlerBuilder -> _) = this.Post (builder.Build (), pattern, handler)

        /// Adds an HTTP POST endpoint.
        [<CustomOperation("post")>]
        member this.Post (builder : WebApplicationBuilder, pattern : Pattern, handler : Func<_, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) = this.Post (builder.Build (), pattern, handler)

        /// Adds an HTTP POST endpoint.
        [<CustomOperation("post")>]
        member this.Post (builder : WebApplicationBuilder, pattern : Pattern, handler : Func<_, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) = this.Post (builder.Build (), pattern, handler)

        /// Adds an HTTP POST endpoint.
        [<CustomOperation("post")>]
        member this.Post (builder : WebApplicationBuilder, pattern : Pattern, handler : Func<_, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) = this.Post (builder.Build (), pattern, handler)

        /// Adds an HTTP POST endpoint.
        [<CustomOperation("post")>]
        member this.Post (builder : WebApplicationBuilder, pattern : Pattern, handler : Func<_, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) = this.Post (builder.Build (), pattern, handler)

        /// Adds an HTTP POST endpoint.
        [<CustomOperation("post")>]
        member this.Post (builder : WebApplicationBuilder, pattern : Pattern, handler : Func<_, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) = this.Post (builder.Build (), pattern, handler)

        /// Adds an HTTP POST endpoint.
        [<CustomOperation("post")>]
        member this.Post (builder : WebApplicationBuilder, pattern : Pattern, handler : Func<_, _, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) = this.Post (builder.Build (), pattern, handler)

        /// Adds an HTTP POST endpoint.
        [<CustomOperation("post")>]
        member this.Post (builder : WebApplicationBuilder, pattern : Pattern, handler : Func<_, _, _, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) = this.Post (builder.Build (), pattern, handler)

        /// Adds an HTTP POST endpoint.
        [<CustomOperation("post")>]
        member this.Post (builder : WebApplicationBuilder, pattern : Pattern, handler : Func<_, _, _, _, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) = this.Post (builder.Build (), pattern, handler)

        /// Adds an HTTP POST endpoint.
        [<CustomOperation("post")>]
        member this.Post (builder : WebApplicationBuilder, pattern : Pattern, handler : Func<_, _, _, _, _, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) = this.Post (builder.Build (), pattern, handler)

        /// Adds an HTTP POST endpoint.
        [<CustomOperation("post")>]
        member this.Post (builder : WebApplicationBuilder, pattern : Pattern, handler : Func<_, _, _, _, _, _, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) = this.Post (builder.Build (), pattern, handler)

        /// Adds an HTTP POST endpoint.
        [<CustomOperation("post")>]
        member this.Post (builder : WebApplicationBuilder, pattern : Pattern, handler : Func<_, _, _, _, _, _, _, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) = this.Post (builder.Build (), pattern, handler)

        /// Adds an HTTP POST endpoint.
        [<CustomOperation("post")>]
        member this.Post (builder : WebApplicationBuilder, pattern : Pattern, handler : Func<_, _, _, _, _, _, _, _, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) = this.Post (builder.Build (), pattern, handler)

        /// Adds an HTTP POST endpoint.
        [<CustomOperation("post")>]
        member this.Post (builder : WebApplicationBuilder, pattern : Pattern, handler : Func<_, _, _, _, _, _, _, _, _, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) = this.Post (builder.Build (), pattern, handler)

        /// Adds an HTTP POST endpoint.
        [<CustomOperation("post")>]
        member this.Post (builder : WebApplicationBuilder, pattern : Pattern, produces : (int * Type) list, handler : Func<_, _>, ?configure : RouteHandlerBuilder -> _) = this.Post (builder.Build (), pattern, produces, handler)

        /// Adds an HTTP POST endpoint.
        [<CustomOperation("post")>]
        member this.Post (builder : WebApplicationBuilder, pattern : Pattern, produces : (int * Type) list, handler : Func<_, _, _>, ?configure : RouteHandlerBuilder -> _) = this.Post (builder.Build (), pattern, produces, handler)

        /// Adds an HTTP POST endpoint.
        [<CustomOperation("post")>]
        member this.Post (builder : WebApplicationBuilder, pattern : Pattern, produces : (int * Type) list, handler : Func<_, _, _, _>, ?configure : RouteHandlerBuilder -> _) = this.Post (builder.Build (), pattern, produces, handler)

        /// Adds an HTTP POST endpoint.
        [<CustomOperation("post")>]
        member this.Post (builder : WebApplicationBuilder, pattern : Pattern, produces : (int * Type) list, handler : Func<_, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) = this.Post (builder.Build (), pattern, produces, handler)

        /// Adds an HTTP POST endpoint.
        [<CustomOperation("post")>]
        member this.Post (builder : WebApplicationBuilder, pattern : Pattern, produces : (int * Type) list, handler : Func<_, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) = this.Post (builder.Build (), pattern, produces, handler)

        /// Adds an HTTP POST endpoint.
        [<CustomOperation("post")>]
        member this.Post (builder : WebApplicationBuilder, pattern : Pattern, produces : (int * Type) list, handler : Func<_, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) = this.Post (builder.Build (), pattern, produces, handler)

        /// Adds an HTTP POST endpoint.
        [<CustomOperation("post")>]
        member this.Post (builder : WebApplicationBuilder, pattern : Pattern, produces : (int * Type) list, handler : Func<_, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) = this.Post (builder.Build (), pattern, produces, handler)

        /// Adds an HTTP POST endpoint.
        [<CustomOperation("post")>]
        member this.Post (builder : WebApplicationBuilder, pattern : Pattern, produces : (int * Type) list, handler : Func<_, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) = this.Post (builder.Build (), pattern, produces, handler)

        /// Adds an HTTP POST endpoint.
        [<CustomOperation("post")>]
        member this.Post (builder : WebApplicationBuilder, pattern : Pattern, produces : (int * Type) list, handler : Func<_, _, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) = this.Post (builder.Build (), pattern, produces, handler)

        /// Adds an HTTP POST endpoint.
        [<CustomOperation("post")>]
        member this.Post (builder : WebApplicationBuilder, pattern : Pattern, produces : (int * Type) list, handler : Func<_, _, _, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) = this.Post (builder.Build (), pattern, produces, handler)

        /// Adds an HTTP POST endpoint.
        [<CustomOperation("post")>]
        member this.Post (builder : WebApplicationBuilder, pattern : Pattern, produces : (int * Type) list, handler : Func<_, _, _, _, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) = this.Post (builder.Build (), pattern, produces, handler)

        /// Adds an HTTP POST endpoint.
        [<CustomOperation("post")>]
        member this.Post (builder : WebApplicationBuilder, pattern : Pattern, produces : (int * Type) list, handler : Func<_, _, _, _, _, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) = this.Post (builder.Build (), pattern, produces, handler)

        /// Adds an HTTP POST endpoint.
        [<CustomOperation("post")>]
        member this.Post (builder : WebApplicationBuilder, pattern : Pattern, produces : (int * Type) list, handler : Func<_, _, _, _, _, _, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) = this.Post (builder.Build (), pattern, produces, handler)

        /// Adds an HTTP POST endpoint.
        [<CustomOperation("post")>]
        member this.Post (builder : WebApplicationBuilder, pattern : Pattern, produces : (int * Type) list, handler : Func<_, _, _, _, _, _, _, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) = this.Post (builder.Build (), pattern, produces, handler)

        /// Adds an HTTP POST endpoint.
        [<CustomOperation("post")>]
        member this.Post (builder : WebApplicationBuilder, pattern : Pattern, produces : (int * Type) list, handler : Func<_, _, _, _, _, _, _, _, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) = this.Post (builder.Build (), pattern, produces, handler)

        /// Adds an HTTP POST endpoint.
        [<CustomOperation("post")>]
        member this.Post (builder : WebApplicationBuilder, pattern : Pattern, produces : (int * Type) list, handler : Func<_, _, _, _, _, _, _, _, _, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) = this.Post (builder.Build (), pattern, produces, handler)

[<AutoOpen>]
module Puts =
    type WebAppBuilder with
        /// Adds an HTTP PUT endpoint.
        [<CustomOperation("put")>]
        member _.Put (app : WebApplication, pattern : Pattern, handler : Func<'a, 'b>, ?configure : RouteHandlerBuilder -> _) =
            let handler : Delegate =
                if typeof<'a>.Equals typeof<unit> then upcast Func<'b> (fun () -> (Unchecked.unbox<Func<unit, 'b>> (box handler)).Invoke ())
                else upcast handler

            app.MapPut (pattern, handler=handler)
            |> RouteHandlerBuilder.withTag (Pattern.toGroupName pattern)
            |> defaultArg (configure |> Option.map ((<<) ignore)) ignore

            app

        /// Adds an HTTP PUT endpoint.
        [<CustomOperation("put")>]
        member _.Put (app : WebApplication, pattern : Pattern, handler : Func<_, _, _>, ?configure : RouteHandlerBuilder -> _) =
            app.MapPut (pattern, handler=handler)
            |> RouteHandlerBuilder.withTag (Pattern.toGroupName pattern)
            |> defaultArg (configure |> Option.map ((<<) ignore)) ignore
            app

        /// Adds an HTTP PUT endpoint.
        [<CustomOperation("put")>]
        member _.Put (app : WebApplication, pattern : Pattern, handler : Func<_, _, _, _>, ?configure : RouteHandlerBuilder -> _) =
            app.MapPut (pattern, handler=handler)
            |> RouteHandlerBuilder.withTag (Pattern.toGroupName pattern)
            |> defaultArg (configure |> Option.map ((<<) ignore)) ignore
            app

        /// Adds an HTTP PUT endpoint.
        [<CustomOperation("put")>]
        member _.Put (app : WebApplication, pattern : Pattern, handler : Func<_, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) =
            app.MapPut (pattern, handler=handler)
            |> RouteHandlerBuilder.withTag (Pattern.toGroupName pattern)
            |> defaultArg (configure |> Option.map ((<<) ignore)) ignore
            app

        /// Adds an HTTP PUT endpoint.
        [<CustomOperation("put")>]
        member _.Put (app : WebApplication, pattern : Pattern, handler : Func<_, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) =
            app.MapPut (pattern, handler=handler)
            |> RouteHandlerBuilder.withTag (Pattern.toGroupName pattern)
            |> defaultArg (configure |> Option.map ((<<) ignore)) ignore
            app

        /// Adds an HTTP PUT endpoint.
        [<CustomOperation("put")>]
        member _.Put (app : WebApplication, pattern : Pattern, handler : Func<_, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) =
            app.MapPut (pattern, handler=handler)
            |> RouteHandlerBuilder.withTag (Pattern.toGroupName pattern)
            |> defaultArg (configure |> Option.map ((<<) ignore)) ignore
            app

        /// Adds an HTTP PUT endpoint.
        [<CustomOperation("put")>]
        member _.Put (app : WebApplication, pattern : Pattern, handler : Func<_, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) =
            app.MapPut (pattern, handler=handler)
            |> RouteHandlerBuilder.withTag (Pattern.toGroupName pattern)
            |> defaultArg (configure |> Option.map ((<<) ignore)) ignore
            app

        /// Adds an HTTP PUT endpoint.
        [<CustomOperation("put")>]
        member _.Put (app : WebApplication, pattern : Pattern, handler : Func<_, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) =
            app.MapPut (pattern, handler=handler)
            |> RouteHandlerBuilder.withTag (Pattern.toGroupName pattern)
            |> defaultArg (configure |> Option.map ((<<) ignore)) ignore
            app

        /// Adds an HTTP PUT endpoint.
        [<CustomOperation("put")>]
        member _.Put (app : WebApplication, pattern : Pattern, handler : Func<_, _, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) =
            app.MapPut (pattern, handler=handler)
            |> RouteHandlerBuilder.withTag (Pattern.toGroupName pattern)
            |> defaultArg (configure |> Option.map ((<<) ignore)) ignore
            app

        /// Adds an HTTP PUT endpoint.
        [<CustomOperation("put")>]
        member _.Put (app : WebApplication, pattern : Pattern, handler : Func<_, _, _, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) =
            app.MapPut (pattern, handler=handler)
            |> RouteHandlerBuilder.withTag (Pattern.toGroupName pattern)
            |> defaultArg (configure |> Option.map ((<<) ignore)) ignore
            app

        /// Adds an HTTP PUT endpoint.
        [<CustomOperation("put")>]
        member _.Put (app : WebApplication, pattern : Pattern, handler : Func<_, _, _, _, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) =
            app.MapPut (pattern, handler=handler)
            |> RouteHandlerBuilder.withTag (Pattern.toGroupName pattern)
            |> defaultArg (configure |> Option.map ((<<) ignore)) ignore
            app

        /// Adds an HTTP PUT endpoint.
        [<CustomOperation("put")>]
        member _.Put (app : WebApplication, pattern : Pattern, handler : Func<_, _, _, _, _, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) =
            app.MapPut (pattern, handler=handler)
            |> RouteHandlerBuilder.withTag (Pattern.toGroupName pattern)
            |> defaultArg (configure |> Option.map ((<<) ignore)) ignore
            app

        /// Adds an HTTP PUT endpoint.
        [<CustomOperation("put")>]
        member _.Put (app : WebApplication, pattern : Pattern, handler : Func<_, _, _, _, _, _, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) =
            app.MapPut (pattern, handler=handler)
            |> RouteHandlerBuilder.withTag (Pattern.toGroupName pattern)
            |> defaultArg (configure |> Option.map ((<<) ignore)) ignore
            app

        /// Adds an HTTP PUT endpoint.
        [<CustomOperation("put")>]
        member _.Put (app : WebApplication, pattern : Pattern, handler : Func<_, _, _, _, _, _, _, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) =
            app.MapPut (pattern, handler=handler)
            |> RouteHandlerBuilder.withTag (Pattern.toGroupName pattern)
            |> defaultArg (configure |> Option.map ((<<) ignore)) ignore
            app

        /// Adds an HTTP PUT endpoint.
        [<CustomOperation("put")>]
        member _.Put (app : WebApplication, pattern : Pattern, handler : Func<_, _, _, _, _, _, _, _, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) =
            app.MapPut (pattern, handler=handler)
            |> RouteHandlerBuilder.withTag (Pattern.toGroupName pattern)
            |> defaultArg (configure |> Option.map ((<<) ignore)) ignore
            app

        /// Adds an HTTP PUT endpoint.
        [<CustomOperation("put")>]
        member _.Put (app : WebApplication, pattern : Pattern, handler : Func<_, _, _, _, _, _, _, _, _, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) =
            app.MapPut (pattern, handler=handler)
            |> RouteHandlerBuilder.withTag (Pattern.toGroupName pattern)
            |> defaultArg (configure |> Option.map ((<<) ignore)) ignore
            app

        /// Adds an HTTP PUT endpoint.
        [<CustomOperation("put")>]
        member _.Put (app : WebApplication, pattern : Pattern, produces : (int * Type) list, handler : Func<'a, 'b>, ?configure : RouteHandlerBuilder -> _) =
            let handler : Delegate =
                if typeof<'a>.Equals typeof<unit> then upcast Func<'b> (fun () -> (Unchecked.unbox<Func<unit, 'b>> (box handler)).Invoke ())
                else upcast handler

            app.MapPut (pattern, handler=handler)
            |> RouteHandlerBuilder.withTag (Pattern.toGroupName pattern)
            |> RouteHandlerBuilder.produces produces
            |> defaultArg (configure |> Option.map ((<<) ignore)) ignore

            app

        /// Adds an HTTP PUT endpoint.
        [<CustomOperation("put")>]
        member _.Put (app : WebApplication, pattern : Pattern, produces : (int * Type) list, handler : Func<_, _, _>, ?configure : RouteHandlerBuilder -> _) =
            app.MapPut (pattern, handler=handler)
            |> RouteHandlerBuilder.withTag (Pattern.toGroupName pattern)
            |> RouteHandlerBuilder.produces produces
            |> defaultArg (configure |> Option.map ((<<) ignore)) ignore
            app

        /// Adds an HTTP PUT endpoint.
        [<CustomOperation("put")>]
        member _.Put (app : WebApplication, pattern : Pattern, produces : (int * Type) list, handler : Func<_, _, _, _>, ?configure : RouteHandlerBuilder -> _) =
            app.MapPut (pattern, handler=handler)
            |> RouteHandlerBuilder.withTag (Pattern.toGroupName pattern)
            |> RouteHandlerBuilder.produces produces
            |> defaultArg (configure |> Option.map ((<<) ignore)) ignore
            app

        /// Adds an HTTP PUT endpoint.
        [<CustomOperation("put")>]
        member _.Put (app : WebApplication, pattern : Pattern, produces : (int * Type) list, handler : Func<_, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) =
            app.MapPut (pattern, handler=handler)
            |> RouteHandlerBuilder.withTag (Pattern.toGroupName pattern)
            |> RouteHandlerBuilder.produces produces
            |> defaultArg (configure |> Option.map ((<<) ignore)) ignore
            app

        /// Adds an HTTP PUT endpoint.
        [<CustomOperation("put")>]
        member _.Put (app : WebApplication, pattern : Pattern, produces : (int * Type) list, handler : Func<_, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) =
            app.MapPut (pattern, handler=handler)
            |> RouteHandlerBuilder.withTag (Pattern.toGroupName pattern)
            |> RouteHandlerBuilder.produces produces
            |> defaultArg (configure |> Option.map ((<<) ignore)) ignore
            app

        /// Adds an HTTP PUT endpoint.
        [<CustomOperation("put")>]
        member _.Put (app : WebApplication, pattern : Pattern, produces : (int * Type) list, handler : Func<_, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) =
            app.MapPut (pattern, handler=handler)
            |> RouteHandlerBuilder.withTag (Pattern.toGroupName pattern)
            |> RouteHandlerBuilder.produces produces
            |> defaultArg (configure |> Option.map ((<<) ignore)) ignore
            app

        /// Adds an HTTP PUT endpoint.
        [<CustomOperation("put")>]
        member _.Put (app : WebApplication, pattern : Pattern, produces : (int * Type) list, handler : Func<_, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) =
            app.MapPut (pattern, handler=handler)
            |> RouteHandlerBuilder.withTag (Pattern.toGroupName pattern)
            |> RouteHandlerBuilder.produces produces
            |> defaultArg (configure |> Option.map ((<<) ignore)) ignore
            app

        /// Adds an HTTP PUT endpoint.
        [<CustomOperation("put")>]
        member _.Put (app : WebApplication, pattern : Pattern, produces : (int * Type) list, handler : Func<_, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) =
            app.MapPut (pattern, handler=handler)
            |> RouteHandlerBuilder.withTag (Pattern.toGroupName pattern)
            |> RouteHandlerBuilder.produces produces
            |> defaultArg (configure |> Option.map ((<<) ignore)) ignore
            app

        /// Adds an HTTP PUT endpoint.
        [<CustomOperation("put")>]
        member _.Put (app : WebApplication, pattern : Pattern, produces : (int * Type) list, handler : Func<_, _, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) =
            app.MapPut (pattern, handler=handler)
            |> RouteHandlerBuilder.withTag (Pattern.toGroupName pattern)
            |> RouteHandlerBuilder.produces produces
            |> defaultArg (configure |> Option.map ((<<) ignore)) ignore
            app

        /// Adds an HTTP PUT endpoint.
        [<CustomOperation("put")>]
        member _.Put (app : WebApplication, pattern : Pattern, produces : (int * Type) list, handler : Func<_, _, _, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) =
            app.MapPut (pattern, handler=handler)
            |> RouteHandlerBuilder.withTag (Pattern.toGroupName pattern)
            |> RouteHandlerBuilder.produces produces
            |> defaultArg (configure |> Option.map ((<<) ignore)) ignore
            app

        /// Adds an HTTP PUT endpoint.
        [<CustomOperation("put")>]
        member _.Put (app : WebApplication, pattern : Pattern, produces : (int * Type) list, handler : Func<_, _, _, _, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) =
            app.MapPut (pattern, handler=handler)
            |> RouteHandlerBuilder.withTag (Pattern.toGroupName pattern)
            |> RouteHandlerBuilder.produces produces
            |> defaultArg (configure |> Option.map ((<<) ignore)) ignore
            app

        /// Adds an HTTP PUT endpoint.
        [<CustomOperation("put")>]
        member _.Put (app : WebApplication, pattern : Pattern, produces : (int * Type) list, handler : Func<_, _, _, _, _, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) =
            app.MapPut (pattern, handler=handler)
            |> RouteHandlerBuilder.withTag (Pattern.toGroupName pattern)
            |> RouteHandlerBuilder.produces produces
            |> defaultArg (configure |> Option.map ((<<) ignore)) ignore
            app

        /// Adds an HTTP PUT endpoint.
        [<CustomOperation("put")>]
        member _.Put (app : WebApplication, pattern : Pattern, produces : (int * Type) list, handler : Func<_, _, _, _, _, _, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) =
            app.MapPut (pattern, handler=handler)
            |> RouteHandlerBuilder.withTag (Pattern.toGroupName pattern)
            |> RouteHandlerBuilder.produces produces
            |> defaultArg (configure |> Option.map ((<<) ignore)) ignore
            app

        /// Adds an HTTP PUT endpoint.
        [<CustomOperation("put")>]
        member _.Put (app : WebApplication, pattern : Pattern, produces : (int * Type) list, handler : Func<_, _, _, _, _, _, _, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) =
            app.MapPut (pattern, handler=handler)
            |> RouteHandlerBuilder.withTag (Pattern.toGroupName pattern)
            |> RouteHandlerBuilder.produces produces
            |> defaultArg (configure |> Option.map ((<<) ignore)) ignore
            app

        /// Adds an HTTP PUT endpoint.
        [<CustomOperation("put")>]
        member _.Put (app : WebApplication, pattern : Pattern, produces : (int * Type) list, handler : Func<_, _, _, _, _, _, _, _, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) =
            app.MapPut (pattern, handler=handler)
            |> RouteHandlerBuilder.withTag (Pattern.toGroupName pattern)
            |> RouteHandlerBuilder.produces produces
            |> defaultArg (configure |> Option.map ((<<) ignore)) ignore
            app

        /// Adds an HTTP PUT endpoint.
        [<CustomOperation("put")>]
        member _.Put (app : WebApplication, pattern : Pattern, produces : (int * Type) list, handler : Func<_, _, _, _, _, _, _, _, _, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) =
            app.MapPut (pattern, handler=handler)
            |> RouteHandlerBuilder.withTag (Pattern.toGroupName pattern)
            |> RouteHandlerBuilder.produces produces
            |> defaultArg (configure |> Option.map ((<<) ignore)) ignore
            app

        /// Adds an HTTP PUT endpoint.
        [<CustomOperation("put")>]
        member this.Put (builder : WebApplicationBuilder, pattern : Pattern, handler : Func<_, _>, ?configure : RouteHandlerBuilder -> _) = this.Put (builder.Build (), pattern, handler)

        /// Adds an HTTP PUT endpoint.
        [<CustomOperation("put")>]
        member this.Put (builder : WebApplicationBuilder, pattern : Pattern, handler : Func<_, _, _>, ?configure : RouteHandlerBuilder -> _) = this.Put (builder.Build (), pattern, handler)

        /// Adds an HTTP PUT endpoint.
        [<CustomOperation("put")>]
        member this.Put (builder : WebApplicationBuilder, pattern : Pattern, handler : Func<_, _, _, _>, ?configure : RouteHandlerBuilder -> _) = this.Put (builder.Build (), pattern, handler)

        /// Adds an HTTP PUT endpoint.
        [<CustomOperation("put")>]
        member this.Put (builder : WebApplicationBuilder, pattern : Pattern, handler : Func<_, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) = this.Put (builder.Build (), pattern, handler)

        /// Adds an HTTP PUT endpoint.
        [<CustomOperation("put")>]
        member this.Put (builder : WebApplicationBuilder, pattern : Pattern, handler : Func<_, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) = this.Put (builder.Build (), pattern, handler)

        /// Adds an HTTP PUT endpoint.
        [<CustomOperation("put")>]
        member this.Put (builder : WebApplicationBuilder, pattern : Pattern, handler : Func<_, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) = this.Put (builder.Build (), pattern, handler)

        /// Adds an HTTP PUT endpoint.
        [<CustomOperation("put")>]
        member this.Put (builder : WebApplicationBuilder, pattern : Pattern, handler : Func<_, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) = this.Put (builder.Build (), pattern, handler)

        /// Adds an HTTP PUT endpoint.
        [<CustomOperation("put")>]
        member this.Put (builder : WebApplicationBuilder, pattern : Pattern, handler : Func<_, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) = this.Put (builder.Build (), pattern, handler)

        /// Adds an HTTP PUT endpoint.
        [<CustomOperation("put")>]
        member this.Put (builder : WebApplicationBuilder, pattern : Pattern, handler : Func<_, _, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) = this.Put (builder.Build (), pattern, handler)

        /// Adds an HTTP PUT endpoint.
        [<CustomOperation("put")>]
        member this.Put (builder : WebApplicationBuilder, pattern : Pattern, handler : Func<_, _, _, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) = this.Put (builder.Build (), pattern, handler)

        /// Adds an HTTP PUT endpoint.
        [<CustomOperation("put")>]
        member this.Put (builder : WebApplicationBuilder, pattern : Pattern, handler : Func<_, _, _, _, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) = this.Put (builder.Build (), pattern, handler)

        /// Adds an HTTP PUT endpoint.
        [<CustomOperation("put")>]
        member this.Put (builder : WebApplicationBuilder, pattern : Pattern, handler : Func<_, _, _, _, _, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) = this.Put (builder.Build (), pattern, handler)

        /// Adds an HTTP PUT endpoint.
        [<CustomOperation("put")>]
        member this.Put (builder : WebApplicationBuilder, pattern : Pattern, handler : Func<_, _, _, _, _, _, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) = this.Put (builder.Build (), pattern, handler)

        /// Adds an HTTP PUT endpoint.
        [<CustomOperation("put")>]
        member this.Put (builder : WebApplicationBuilder, pattern : Pattern, handler : Func<_, _, _, _, _, _, _, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) = this.Put (builder.Build (), pattern, handler)

        /// Adds an HTTP PUT endpoint.
        [<CustomOperation("put")>]
        member this.Put (builder : WebApplicationBuilder, pattern : Pattern, handler : Func<_, _, _, _, _, _, _, _, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) = this.Put (builder.Build (), pattern, handler)

        /// Adds an HTTP PUT endpoint.
        [<CustomOperation("put")>]
        member this.Put (builder : WebApplicationBuilder, pattern : Pattern, handler : Func<_, _, _, _, _, _, _, _, _, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) = this.Put (builder.Build (), pattern, handler)

        /// Adds an HTTP PUT endpoint.
        [<CustomOperation("put")>]
        member this.Put (builder : WebApplicationBuilder, pattern : Pattern, produces : (int * Type) list, handler : Func<_, _>, ?configure : RouteHandlerBuilder -> _) = this.Put (builder.Build (), pattern, produces, handler)

        /// Adds an HTTP PUT endpoint.
        [<CustomOperation("put")>]
        member this.Put (builder : WebApplicationBuilder, pattern : Pattern, produces : (int * Type) list, handler : Func<_, _, _>, ?configure : RouteHandlerBuilder -> _) = this.Put (builder.Build (), pattern, produces, handler)

        /// Adds an HTTP PUT endpoint.
        [<CustomOperation("put")>]
        member this.Put (builder : WebApplicationBuilder, pattern : Pattern, produces : (int * Type) list, handler : Func<_, _, _, _>, ?configure : RouteHandlerBuilder -> _) = this.Put (builder.Build (), pattern, produces, handler)

        /// Adds an HTTP PUT endpoint.
        [<CustomOperation("put")>]
        member this.Put (builder : WebApplicationBuilder, pattern : Pattern, produces : (int * Type) list, handler : Func<_, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) = this.Put (builder.Build (), pattern, produces, handler)

        /// Adds an HTTP PUT endpoint.
        [<CustomOperation("put")>]
        member this.Put (builder : WebApplicationBuilder, pattern : Pattern, produces : (int * Type) list, handler : Func<_, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) = this.Put (builder.Build (), pattern, produces, handler)

        /// Adds an HTTP PUT endpoint.
        [<CustomOperation("put")>]
        member this.Put (builder : WebApplicationBuilder, pattern : Pattern, produces : (int * Type) list, handler : Func<_, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) = this.Put (builder.Build (), pattern, produces, handler)

        /// Adds an HTTP PUT endpoint.
        [<CustomOperation("put")>]
        member this.Put (builder : WebApplicationBuilder, pattern : Pattern, produces : (int * Type) list, handler : Func<_, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) = this.Put (builder.Build (), pattern, produces, handler)

        /// Adds an HTTP PUT endpoint.
        [<CustomOperation("put")>]
        member this.Put (builder : WebApplicationBuilder, pattern : Pattern, produces : (int * Type) list, handler : Func<_, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) = this.Put (builder.Build (), pattern, produces, handler)

        /// Adds an HTTP PUT endpoint.
        [<CustomOperation("put")>]
        member this.Put (builder : WebApplicationBuilder, pattern : Pattern, produces : (int * Type) list, handler : Func<_, _, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) = this.Put (builder.Build (), pattern, produces, handler)

        /// Adds an HTTP PUT endpoint.
        [<CustomOperation("put")>]
        member this.Put (builder : WebApplicationBuilder, pattern : Pattern, produces : (int * Type) list, handler : Func<_, _, _, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) = this.Put (builder.Build (), pattern, produces, handler)

        /// Adds an HTTP PUT endpoint.
        [<CustomOperation("put")>]
        member this.Put (builder : WebApplicationBuilder, pattern : Pattern, produces : (int * Type) list, handler : Func<_, _, _, _, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) = this.Put (builder.Build (), pattern, produces, handler)

        /// Adds an HTTP PUT endpoint.
        [<CustomOperation("put")>]
        member this.Put (builder : WebApplicationBuilder, pattern : Pattern, produces : (int * Type) list, handler : Func<_, _, _, _, _, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) = this.Put (builder.Build (), pattern, produces, handler)

        /// Adds an HTTP PUT endpoint.
        [<CustomOperation("put")>]
        member this.Put (builder : WebApplicationBuilder, pattern : Pattern, produces : (int * Type) list, handler : Func<_, _, _, _, _, _, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) = this.Put (builder.Build (), pattern, produces, handler)

        /// Adds an HTTP PUT endpoint.
        [<CustomOperation("put")>]
        member this.Put (builder : WebApplicationBuilder, pattern : Pattern, produces : (int * Type) list, handler : Func<_, _, _, _, _, _, _, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) = this.Put (builder.Build (), pattern, produces, handler)

        /// Adds an HTTP PUT endpoint.
        [<CustomOperation("put")>]
        member this.Put (builder : WebApplicationBuilder, pattern : Pattern, produces : (int * Type) list, handler : Func<_, _, _, _, _, _, _, _, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) = this.Put (builder.Build (), pattern, produces, handler)

        /// Adds an HTTP PUT endpoint.
        [<CustomOperation("put")>]
        member this.Put (builder : WebApplicationBuilder, pattern : Pattern, produces : (int * Type) list, handler : Func<_, _, _, _, _, _, _, _, _, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) = this.Put (builder.Build (), pattern, produces, handler)

[<AutoOpen>]
module Deletes =
    type WebAppBuilder with
        /// Adds an HTTP DELETE endpoint.
        [<CustomOperation("delete")>]
        member _.Delete (app : WebApplication, pattern : Pattern, handler : Func<'a, 'b>, ?configure : RouteHandlerBuilder -> _) =
            let handler : Delegate =
                if typeof<'a>.Equals typeof<unit> then upcast Func<'b> (fun () -> (Unchecked.unbox<Func<unit, 'b>> (box handler)).Invoke ())
                else upcast handler

            app.MapDelete (pattern, handler=handler)
            |> RouteHandlerBuilder.withTag (Pattern.toGroupName pattern)
            |> defaultArg (configure |> Option.map ((<<) ignore)) ignore

            app

        /// Adds an HTTP DELETE endpoint.
        [<CustomOperation("delete")>]
        member _.Delete (app : WebApplication, pattern : Pattern, handler : Func<_, _, _>, ?configure : RouteHandlerBuilder -> _) =
            app.MapDelete (pattern, handler=handler)
            |> RouteHandlerBuilder.withTag (Pattern.toGroupName pattern)
            |> defaultArg (configure |> Option.map ((<<) ignore)) ignore
            app

        /// Adds an HTTP DELETE endpoint.
        [<CustomOperation("delete")>]
        member _.Delete (app : WebApplication, pattern : Pattern, handler : Func<_, _, _, _>, ?configure : RouteHandlerBuilder -> _) =
            app.MapDelete (pattern, handler=handler)
            |> RouteHandlerBuilder.withTag (Pattern.toGroupName pattern)
            |> defaultArg (configure |> Option.map ((<<) ignore)) ignore
            app

        /// Adds an HTTP DELETE endpoint.
        [<CustomOperation("delete")>]
        member _.Delete (app : WebApplication, pattern : Pattern, handler : Func<_, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) =
            app.MapDelete (pattern, handler=handler)
            |> RouteHandlerBuilder.withTag (Pattern.toGroupName pattern)
            |> defaultArg (configure |> Option.map ((<<) ignore)) ignore
            app

        /// Adds an HTTP DELETE endpoint.
        [<CustomOperation("delete")>]
        member _.Delete (app : WebApplication, pattern : Pattern, handler : Func<_, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) =
            app.MapDelete (pattern, handler=handler)
            |> RouteHandlerBuilder.withTag (Pattern.toGroupName pattern)
            |> defaultArg (configure |> Option.map ((<<) ignore)) ignore
            app

        /// Adds an HTTP DELETE endpoint.
        [<CustomOperation("delete")>]
        member _.Delete (app : WebApplication, pattern : Pattern, handler : Func<_, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) =
            app.MapDelete (pattern, handler=handler)
            |> RouteHandlerBuilder.withTag (Pattern.toGroupName pattern)
            |> defaultArg (configure |> Option.map ((<<) ignore)) ignore
            app

        /// Adds an HTTP DELETE endpoint.
        [<CustomOperation("delete")>]
        member _.Delete (app : WebApplication, pattern : Pattern, handler : Func<_, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) =
            app.MapDelete (pattern, handler=handler)
            |> RouteHandlerBuilder.withTag (Pattern.toGroupName pattern)
            |> defaultArg (configure |> Option.map ((<<) ignore)) ignore
            app

        /// Adds an HTTP DELETE endpoint.
        [<CustomOperation("delete")>]
        member _.Delete (app : WebApplication, pattern : Pattern, handler : Func<_, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) =
            app.MapDelete (pattern, handler=handler)
            |> RouteHandlerBuilder.withTag (Pattern.toGroupName pattern)
            |> defaultArg (configure |> Option.map ((<<) ignore)) ignore
            app

        /// Adds an HTTP DELETE endpoint.
        [<CustomOperation("delete")>]
        member _.Delete (app : WebApplication, pattern : Pattern, handler : Func<_, _, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) =
            app.MapDelete (pattern, handler=handler)
            |> RouteHandlerBuilder.withTag (Pattern.toGroupName pattern)
            |> defaultArg (configure |> Option.map ((<<) ignore)) ignore
            app

        /// Adds an HTTP DELETE endpoint.
        [<CustomOperation("delete")>]
        member _.Delete (app : WebApplication, pattern : Pattern, handler : Func<_, _, _, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) =
            app.MapDelete (pattern, handler=handler)
            |> RouteHandlerBuilder.withTag (Pattern.toGroupName pattern)
            |> defaultArg (configure |> Option.map ((<<) ignore)) ignore
            app

        /// Adds an HTTP DELETE endpoint.
        [<CustomOperation("delete")>]
        member _.Delete (app : WebApplication, pattern : Pattern, handler : Func<_, _, _, _, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) =
            app.MapDelete (pattern, handler=handler)
            |> RouteHandlerBuilder.withTag (Pattern.toGroupName pattern)
            |> defaultArg (configure |> Option.map ((<<) ignore)) ignore
            app

        /// Adds an HTTP DELETE endpoint.
        [<CustomOperation("delete")>]
        member _.Delete (app : WebApplication, pattern : Pattern, handler : Func<_, _, _, _, _, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) =
            app.MapDelete (pattern, handler=handler)
            |> RouteHandlerBuilder.withTag (Pattern.toGroupName pattern)
            |> defaultArg (configure |> Option.map ((<<) ignore)) ignore
            app

        /// Adds an HTTP DELETE endpoint.
        [<CustomOperation("delete")>]
        member _.Delete (app : WebApplication, pattern : Pattern, handler : Func<_, _, _, _, _, _, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) =
            app.MapDelete (pattern, handler=handler)
            |> RouteHandlerBuilder.withTag (Pattern.toGroupName pattern)
            |> defaultArg (configure |> Option.map ((<<) ignore)) ignore
            app

        /// Adds an HTTP DELETE endpoint.
        [<CustomOperation("delete")>]
        member _.Delete (app : WebApplication, pattern : Pattern, handler : Func<_, _, _, _, _, _, _, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) =
            app.MapDelete (pattern, handler=handler)
            |> RouteHandlerBuilder.withTag (Pattern.toGroupName pattern)
            |> defaultArg (configure |> Option.map ((<<) ignore)) ignore
            app

        /// Adds an HTTP DELETE endpoint.
        [<CustomOperation("delete")>]
        member _.Delete (app : WebApplication, pattern : Pattern, handler : Func<_, _, _, _, _, _, _, _, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) =
            app.MapDelete (pattern, handler=handler)
            |> RouteHandlerBuilder.withTag (Pattern.toGroupName pattern)
            |> defaultArg (configure |> Option.map ((<<) ignore)) ignore
            app

        /// Adds an HTTP DELETE endpoint.
        [<CustomOperation("delete")>]
        member _.Delete (app : WebApplication, pattern : Pattern, handler : Func<_, _, _, _, _, _, _, _, _, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) =
            app.MapDelete (pattern, handler=handler)
            |> RouteHandlerBuilder.withTag (Pattern.toGroupName pattern)
            |> defaultArg (configure |> Option.map ((<<) ignore)) ignore
            app

        /// Adds an HTTP DELETE endpoint.
        [<CustomOperation("delete")>]
        member _.Delete (app : WebApplication, pattern : Pattern, produces : (int * Type) list, handler : Func<'a, 'b>, ?configure : RouteHandlerBuilder -> _) =
            let handler : Delegate =
                if typeof<'a>.Equals typeof<unit> then upcast Func<'b> (fun () -> (Unchecked.unbox<Func<unit, 'b>> (box handler)).Invoke ())
                else upcast handler

            app.MapDelete (pattern, handler=handler)
            |> RouteHandlerBuilder.withTag (Pattern.toGroupName pattern)
            |> RouteHandlerBuilder.produces produces
            |> defaultArg (configure |> Option.map ((<<) ignore)) ignore

            app

        /// Adds an HTTP DELETE endpoint.
        [<CustomOperation("delete")>]
        member _.Delete (app : WebApplication, pattern : Pattern, produces : (int * Type) list, handler : Func<_, _, _>, ?configure : RouteHandlerBuilder -> _) =
            app.MapDelete (pattern, handler=handler)
            |> RouteHandlerBuilder.withTag (Pattern.toGroupName pattern)
            |> RouteHandlerBuilder.produces produces
            |> defaultArg (configure |> Option.map ((<<) ignore)) ignore
            app

        /// Adds an HTTP DELETE endpoint.
        [<CustomOperation("delete")>]
        member _.Delete (app : WebApplication, pattern : Pattern, produces : (int * Type) list, handler : Func<_, _, _, _>, ?configure : RouteHandlerBuilder -> _) =
            app.MapDelete (pattern, handler=handler)
            |> RouteHandlerBuilder.withTag (Pattern.toGroupName pattern)
            |> RouteHandlerBuilder.produces produces
            |> defaultArg (configure |> Option.map ((<<) ignore)) ignore
            app

        /// Adds an HTTP DELETE endpoint.
        [<CustomOperation("delete")>]
        member _.Delete (app : WebApplication, pattern : Pattern, produces : (int * Type) list, handler : Func<_, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) =
            app.MapDelete (pattern, handler=handler)
            |> RouteHandlerBuilder.withTag (Pattern.toGroupName pattern)
            |> RouteHandlerBuilder.produces produces
            |> defaultArg (configure |> Option.map ((<<) ignore)) ignore
            app

        /// Adds an HTTP DELETE endpoint.
        [<CustomOperation("delete")>]
        member _.Delete (app : WebApplication, pattern : Pattern, produces : (int * Type) list, handler : Func<_, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) =
            app.MapDelete (pattern, handler=handler)
            |> RouteHandlerBuilder.withTag (Pattern.toGroupName pattern)
            |> RouteHandlerBuilder.produces produces
            |> defaultArg (configure |> Option.map ((<<) ignore)) ignore
            app

        /// Adds an HTTP DELETE endpoint.
        [<CustomOperation("delete")>]
        member _.Delete (app : WebApplication, pattern : Pattern, produces : (int * Type) list, handler : Func<_, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) =
            app.MapDelete (pattern, handler=handler)
            |> RouteHandlerBuilder.withTag (Pattern.toGroupName pattern)
            |> RouteHandlerBuilder.produces produces
            |> defaultArg (configure |> Option.map ((<<) ignore)) ignore
            app

        /// Adds an HTTP DELETE endpoint.
        [<CustomOperation("delete")>]
        member _.Delete (app : WebApplication, pattern : Pattern, produces : (int * Type) list, handler : Func<_, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) =
            app.MapDelete (pattern, handler=handler)
            |> RouteHandlerBuilder.withTag (Pattern.toGroupName pattern)
            |> RouteHandlerBuilder.produces produces
            |> defaultArg (configure |> Option.map ((<<) ignore)) ignore
            app

        /// Adds an HTTP DELETE endpoint.
        [<CustomOperation("delete")>]
        member _.Delete (app : WebApplication, pattern : Pattern, produces : (int * Type) list, handler : Func<_, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) =
            app.MapDelete (pattern, handler=handler)
            |> RouteHandlerBuilder.withTag (Pattern.toGroupName pattern)
            |> RouteHandlerBuilder.produces produces
            |> defaultArg (configure |> Option.map ((<<) ignore)) ignore
            app

        /// Adds an HTTP DELETE endpoint.
        [<CustomOperation("delete")>]
        member _.Delete (app : WebApplication, pattern : Pattern, produces : (int * Type) list, handler : Func<_, _, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) =
            app.MapDelete (pattern, handler=handler)
            |> RouteHandlerBuilder.withTag (Pattern.toGroupName pattern)
            |> RouteHandlerBuilder.produces produces
            |> defaultArg (configure |> Option.map ((<<) ignore)) ignore
            app

        /// Adds an HTTP DELETE endpoint.
        [<CustomOperation("delete")>]
        member _.Delete (app : WebApplication, pattern : Pattern, produces : (int * Type) list, handler : Func<_, _, _, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) =
            app.MapDelete (pattern, handler=handler)
            |> RouteHandlerBuilder.withTag (Pattern.toGroupName pattern)
            |> RouteHandlerBuilder.produces produces
            |> defaultArg (configure |> Option.map ((<<) ignore)) ignore
            app

        /// Adds an HTTP DELETE endpoint.
        [<CustomOperation("delete")>]
        member _.Delete (app : WebApplication, pattern : Pattern, produces : (int * Type) list, handler : Func<_, _, _, _, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) =
            app.MapDelete (pattern, handler=handler)
            |> RouteHandlerBuilder.withTag (Pattern.toGroupName pattern)
            |> RouteHandlerBuilder.produces produces
            |> defaultArg (configure |> Option.map ((<<) ignore)) ignore
            app

        /// Adds an HTTP DELETE endpoint.
        [<CustomOperation("delete")>]
        member _.Delete (app : WebApplication, pattern : Pattern, produces : (int * Type) list, handler : Func<_, _, _, _, _, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) =
            app.MapDelete (pattern, handler=handler)
            |> RouteHandlerBuilder.withTag (Pattern.toGroupName pattern)
            |> RouteHandlerBuilder.produces produces
            |> defaultArg (configure |> Option.map ((<<) ignore)) ignore
            app

        /// Adds an HTTP DELETE endpoint.
        [<CustomOperation("delete")>]
        member _.Delete (app : WebApplication, pattern : Pattern, produces : (int * Type) list, handler : Func<_, _, _, _, _, _, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) =
            app.MapDelete (pattern, handler=handler)
            |> RouteHandlerBuilder.withTag (Pattern.toGroupName pattern)
            |> RouteHandlerBuilder.produces produces
            |> defaultArg (configure |> Option.map ((<<) ignore)) ignore
            app

        /// Adds an HTTP DELETE endpoint.
        [<CustomOperation("delete")>]
        member _.Delete (app : WebApplication, pattern : Pattern, produces : (int * Type) list, handler : Func<_, _, _, _, _, _, _, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) =
            app.MapDelete (pattern, handler=handler)
            |> RouteHandlerBuilder.withTag (Pattern.toGroupName pattern)
            |> RouteHandlerBuilder.produces produces
            |> defaultArg (configure |> Option.map ((<<) ignore)) ignore
            app

        /// Adds an HTTP DELETE endpoint.
        [<CustomOperation("delete")>]
        member _.Delete (app : WebApplication, pattern : Pattern, produces : (int * Type) list, handler : Func<_, _, _, _, _, _, _, _, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) =
            app.MapDelete (pattern, handler=handler)
            |> RouteHandlerBuilder.withTag (Pattern.toGroupName pattern)
            |> RouteHandlerBuilder.produces produces
            |> defaultArg (configure |> Option.map ((<<) ignore)) ignore
            app

        /// Adds an HTTP DELETE endpoint.
        [<CustomOperation("delete")>]
        member _.Delete (app : WebApplication, pattern : Pattern, produces : (int * Type) list, handler : Func<_, _, _, _, _, _, _, _, _, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) =
            app.MapDelete (pattern, handler=handler)
            |> RouteHandlerBuilder.withTag (Pattern.toGroupName pattern)
            |> RouteHandlerBuilder.produces produces
            |> defaultArg (configure |> Option.map ((<<) ignore)) ignore
            app

        /// Adds an HTTP DELETE endpoint.
        [<CustomOperation("delete")>]
        member this.Delete (builder : WebApplicationBuilder, pattern : Pattern, handler : Func<_, _>, ?configure : RouteHandlerBuilder -> _) = this.Delete (builder.Build (), pattern, handler)

        /// Adds an HTTP DELETE endpoint.
        [<CustomOperation("delete")>]
        member this.Delete (builder : WebApplicationBuilder, pattern : Pattern, handler : Func<_, _, _>, ?configure : RouteHandlerBuilder -> _) = this.Delete (builder.Build (), pattern, handler)

        /// Adds an HTTP DELETE endpoint.
        [<CustomOperation("delete")>]
        member this.Delete (builder : WebApplicationBuilder, pattern : Pattern, handler : Func<_, _, _, _>, ?configure : RouteHandlerBuilder -> _) = this.Delete (builder.Build (), pattern, handler)

        /// Adds an HTTP DELETE endpoint.
        [<CustomOperation("delete")>]
        member this.Delete (builder : WebApplicationBuilder, pattern : Pattern, handler : Func<_, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) = this.Delete (builder.Build (), pattern, handler)

        /// Adds an HTTP DELETE endpoint.
        [<CustomOperation("delete")>]
        member this.Delete (builder : WebApplicationBuilder, pattern : Pattern, handler : Func<_, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) = this.Delete (builder.Build (), pattern, handler)

        /// Adds an HTTP DELETE endpoint.
        [<CustomOperation("delete")>]
        member this.Delete (builder : WebApplicationBuilder, pattern : Pattern, handler : Func<_, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) = this.Delete (builder.Build (), pattern, handler)

        /// Adds an HTTP DELETE endpoint.
        [<CustomOperation("delete")>]
        member this.Delete (builder : WebApplicationBuilder, pattern : Pattern, handler : Func<_, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) = this.Delete (builder.Build (), pattern, handler)

        /// Adds an HTTP DELETE endpoint.
        [<CustomOperation("delete")>]
        member this.Delete (builder : WebApplicationBuilder, pattern : Pattern, handler : Func<_, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) = this.Delete (builder.Build (), pattern, handler)

        /// Adds an HTTP DELETE endpoint.
        [<CustomOperation("delete")>]
        member this.Delete (builder : WebApplicationBuilder, pattern : Pattern, handler : Func<_, _, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) = this.Delete (builder.Build (), pattern, handler)

        /// Adds an HTTP DELETE endpoint.
        [<CustomOperation("delete")>]
        member this.Delete (builder : WebApplicationBuilder, pattern : Pattern, handler : Func<_, _, _, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) = this.Delete (builder.Build (), pattern, handler)

        /// Adds an HTTP DELETE endpoint.
        [<CustomOperation("delete")>]
        member this.Delete (builder : WebApplicationBuilder, pattern : Pattern, handler : Func<_, _, _, _, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) = this.Delete (builder.Build (), pattern, handler)

        /// Adds an HTTP DELETE endpoint.
        [<CustomOperation("delete")>]
        member this.Delete (builder : WebApplicationBuilder, pattern : Pattern, handler : Func<_, _, _, _, _, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) = this.Delete (builder.Build (), pattern, handler)

        /// Adds an HTTP DELETE endpoint.
        [<CustomOperation("delete")>]
        member this.Delete (builder : WebApplicationBuilder, pattern : Pattern, handler : Func<_, _, _, _, _, _, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) = this.Delete (builder.Build (), pattern, handler)

        /// Adds an HTTP DELETE endpoint.
        [<CustomOperation("delete")>]
        member this.Delete (builder : WebApplicationBuilder, pattern : Pattern, handler : Func<_, _, _, _, _, _, _, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) = this.Delete (builder.Build (), pattern, handler)

        /// Adds an HTTP DELETE endpoint.
        [<CustomOperation("delete")>]
        member this.Delete (builder : WebApplicationBuilder, pattern : Pattern, handler : Func<_, _, _, _, _, _, _, _, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) = this.Delete (builder.Build (), pattern, handler)

        /// Adds an HTTP DELETE endpoint.
        [<CustomOperation("delete")>]
        member this.Delete (builder : WebApplicationBuilder, pattern : Pattern, handler : Func<_, _, _, _, _, _, _, _, _, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) = this.Delete (builder.Build (), pattern, handler)

        /// Adds an HTTP DELETE endpoint.
        [<CustomOperation("delete")>]
        member this.Delete (builder : WebApplicationBuilder, pattern : Pattern, produces : (int * Type) list, handler : Func<_, _>, ?configure : RouteHandlerBuilder -> _) = this.Delete (builder.Build (), pattern, produces, handler)

        /// Adds an HTTP DELETE endpoint.
        [<CustomOperation("delete")>]
        member this.Delete (builder : WebApplicationBuilder, pattern : Pattern, produces : (int * Type) list, handler : Func<_, _, _>, ?configure : RouteHandlerBuilder -> _) = this.Delete (builder.Build (), pattern, produces, handler)

        /// Adds an HTTP DELETE endpoint.
        [<CustomOperation("delete")>]
        member this.Delete (builder : WebApplicationBuilder, pattern : Pattern, produces : (int * Type) list, handler : Func<_, _, _, _>, ?configure : RouteHandlerBuilder -> _) = this.Delete (builder.Build (), pattern, produces, handler)

        /// Adds an HTTP DELETE endpoint.
        [<CustomOperation("delete")>]
        member this.Delete (builder : WebApplicationBuilder, pattern : Pattern, produces : (int * Type) list, handler : Func<_, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) = this.Delete (builder.Build (), pattern, produces, handler)

        /// Adds an HTTP DELETE endpoint.
        [<CustomOperation("delete")>]
        member this.Delete (builder : WebApplicationBuilder, pattern : Pattern, produces : (int * Type) list, handler : Func<_, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) = this.Delete (builder.Build (), pattern, produces, handler)

        /// Adds an HTTP DELETE endpoint.
        [<CustomOperation("delete")>]
        member this.Delete (builder : WebApplicationBuilder, pattern : Pattern, produces : (int * Type) list, handler : Func<_, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) = this.Delete (builder.Build (), pattern, produces, handler)

        /// Adds an HTTP DELETE endpoint.
        [<CustomOperation("delete")>]
        member this.Delete (builder : WebApplicationBuilder, pattern : Pattern, produces : (int * Type) list, handler : Func<_, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) = this.Delete (builder.Build (), pattern, produces, handler)

        /// Adds an HTTP DELETE endpoint.
        [<CustomOperation("delete")>]
        member this.Delete (builder : WebApplicationBuilder, pattern : Pattern, produces : (int * Type) list, handler : Func<_, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) = this.Delete (builder.Build (), pattern, produces, handler)

        /// Adds an HTTP DELETE endpoint.
        [<CustomOperation("delete")>]
        member this.Delete (builder : WebApplicationBuilder, pattern : Pattern, produces : (int * Type) list, handler : Func<_, _, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) = this.Delete (builder.Build (), pattern, produces, handler)

        /// Adds an HTTP DELETE endpoint.
        [<CustomOperation("delete")>]
        member this.Delete (builder : WebApplicationBuilder, pattern : Pattern, produces : (int * Type) list, handler : Func<_, _, _, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) = this.Delete (builder.Build (), pattern, produces, handler)

        /// Adds an HTTP DELETE endpoint.
        [<CustomOperation("delete")>]
        member this.Delete (builder : WebApplicationBuilder, pattern : Pattern, produces : (int * Type) list, handler : Func<_, _, _, _, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) = this.Delete (builder.Build (), pattern, produces, handler)

        /// Adds an HTTP DELETE endpoint.
        [<CustomOperation("delete")>]
        member this.Delete (builder : WebApplicationBuilder, pattern : Pattern, produces : (int * Type) list, handler : Func<_, _, _, _, _, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) = this.Delete (builder.Build (), pattern, produces, handler)

        /// Adds an HTTP DELETE endpoint.
        [<CustomOperation("delete")>]
        member this.Delete (builder : WebApplicationBuilder, pattern : Pattern, produces : (int * Type) list, handler : Func<_, _, _, _, _, _, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) = this.Delete (builder.Build (), pattern, produces, handler)

        /// Adds an HTTP DELETE endpoint.
        [<CustomOperation("delete")>]
        member this.Delete (builder : WebApplicationBuilder, pattern : Pattern, produces : (int * Type) list, handler : Func<_, _, _, _, _, _, _, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) = this.Delete (builder.Build (), pattern, produces, handler)

        /// Adds an HTTP DELETE endpoint.
        [<CustomOperation("delete")>]
        member this.Delete (builder : WebApplicationBuilder, pattern : Pattern, produces : (int * Type) list, handler : Func<_, _, _, _, _, _, _, _, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) = this.Delete (builder.Build (), pattern, produces, handler)

        /// Adds an HTTP DELETE endpoint.
        [<CustomOperation("delete")>]
        member this.Delete (builder : WebApplicationBuilder, pattern : Pattern, produces : (int * Type) list, handler : Func<_, _, _, _, _, _, _, _, _, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) = this.Delete (builder.Build (), pattern, produces, handler)

#if NET7_0_OR_GREATER
[<AutoOpen>]
module Patches =
    type WebAppBuilder with
        /// Adds an HTTP PATCH endpoint.
        [<CustomOperation("patch")>]
        member _.Patch (app : WebApplication, pattern : Pattern, handler : Func<'a, 'b>, ?configure : RouteHandlerBuilder -> _) =
            let handler : Delegate =
                if typeof<'a>.Equals typeof<unit> then upcast Func<'b> (fun () -> (Unchecked.unbox<Func<unit, 'b>> (box handler)).Invoke ())
                else upcast handler

            app.MapPatch (pattern, handler=handler)
            |> RouteHandlerBuilder.withTag (Pattern.toGroupName pattern)
            |> defaultArg (configure |> Option.map ((<<) ignore)) ignore

            app

        /// Adds an HTTP PATCH endpoint.
        [<CustomOperation("patch")>]
        member _.Patch (app : WebApplication, pattern : Pattern, handler : Func<_, _, _>, ?configure : RouteHandlerBuilder -> _) =
            app.MapPatch (pattern, handler=handler)
            |> RouteHandlerBuilder.withTag (Pattern.toGroupName pattern)
            |> defaultArg (configure |> Option.map ((<<) ignore)) ignore
            app

        /// Adds an HTTP PATCH endpoint.
        [<CustomOperation("patch")>]
        member _.Patch (app : WebApplication, pattern : Pattern, handler : Func<_, _, _, _>, ?configure : RouteHandlerBuilder -> _) =
            app.MapPatch (pattern, handler=handler)
            |> RouteHandlerBuilder.withTag (Pattern.toGroupName pattern)
            |> defaultArg (configure |> Option.map ((<<) ignore)) ignore
            app

        /// Adds an HTTP PATCH endpoint.
        [<CustomOperation("patch")>]
        member _.Patch (app : WebApplication, pattern : Pattern, handler : Func<_, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) =
            app.MapPatch (pattern, handler=handler)
            |> RouteHandlerBuilder.withTag (Pattern.toGroupName pattern)
            |> defaultArg (configure |> Option.map ((<<) ignore)) ignore
            app

        /// Adds an HTTP PATCH endpoint.
        [<CustomOperation("patch")>]
        member _.Patch (app : WebApplication, pattern : Pattern, handler : Func<_, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) =
            app.MapPatch (pattern, handler=handler)
            |> RouteHandlerBuilder.withTag (Pattern.toGroupName pattern)
            |> defaultArg (configure |> Option.map ((<<) ignore)) ignore
            app

        /// Adds an HTTP PATCH endpoint.
        [<CustomOperation("patch")>]
        member _.Patch (app : WebApplication, pattern : Pattern, handler : Func<_, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) =
            app.MapPatch (pattern, handler=handler)
            |> RouteHandlerBuilder.withTag (Pattern.toGroupName pattern)
            |> defaultArg (configure |> Option.map ((<<) ignore)) ignore
            app

        /// Adds an HTTP PATCH endpoint.
        [<CustomOperation("patch")>]
        member _.Patch (app : WebApplication, pattern : Pattern, handler : Func<_, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) =
            app.MapPatch (pattern, handler=handler)
            |> RouteHandlerBuilder.withTag (Pattern.toGroupName pattern)
            |> defaultArg (configure |> Option.map ((<<) ignore)) ignore
            app

        /// Adds an HTTP PATCH endpoint.
        [<CustomOperation("patch")>]
        member _.Patch (app : WebApplication, pattern : Pattern, handler : Func<_, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) =
            app.MapPatch (pattern, handler=handler)
            |> RouteHandlerBuilder.withTag (Pattern.toGroupName pattern)
            |> defaultArg (configure |> Option.map ((<<) ignore)) ignore
            app

        /// Adds an HTTP PATCH endpoint.
        [<CustomOperation("patch")>]
        member _.Patch (app : WebApplication, pattern : Pattern, handler : Func<_, _, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) =
            app.MapPatch (pattern, handler=handler)
            |> RouteHandlerBuilder.withTag (Pattern.toGroupName pattern)
            |> defaultArg (configure |> Option.map ((<<) ignore)) ignore
            app

        /// Adds an HTTP PATCH endpoint.
        [<CustomOperation("patch")>]
        member _.Patch (app : WebApplication, pattern : Pattern, handler : Func<_, _, _, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) =
            app.MapPatch (pattern, handler=handler)
            |> RouteHandlerBuilder.withTag (Pattern.toGroupName pattern)
            |> defaultArg (configure |> Option.map ((<<) ignore)) ignore
            app

        /// Adds an HTTP PATCH endpoint.
        [<CustomOperation("patch")>]
        member _.Patch (app : WebApplication, pattern : Pattern, handler : Func<_, _, _, _, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) =
            app.MapPatch (pattern, handler=handler)
            |> RouteHandlerBuilder.withTag (Pattern.toGroupName pattern)
            |> defaultArg (configure |> Option.map ((<<) ignore)) ignore
            app

        /// Adds an HTTP PATCH endpoint.
        [<CustomOperation("patch")>]
        member _.Patch (app : WebApplication, pattern : Pattern, handler : Func<_, _, _, _, _, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) =
            app.MapPatch (pattern, handler=handler)
            |> RouteHandlerBuilder.withTag (Pattern.toGroupName pattern)
            |> defaultArg (configure |> Option.map ((<<) ignore)) ignore
            app

        /// Adds an HTTP PATCH endpoint.
        [<CustomOperation("patch")>]
        member _.Patch (app : WebApplication, pattern : Pattern, handler : Func<_, _, _, _, _, _, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) =
            app.MapPatch (pattern, handler=handler)
            |> RouteHandlerBuilder.withTag (Pattern.toGroupName pattern)
            |> defaultArg (configure |> Option.map ((<<) ignore)) ignore
            app

        /// Adds an HTTP PATCH endpoint.
        [<CustomOperation("patch")>]
        member _.Patch (app : WebApplication, pattern : Pattern, handler : Func<_, _, _, _, _, _, _, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) =
            app.MapPatch (pattern, handler=handler)
            |> RouteHandlerBuilder.withTag (Pattern.toGroupName pattern)
            |> defaultArg (configure |> Option.map ((<<) ignore)) ignore
            app

        /// Adds an HTTP PATCH endpoint.
        [<CustomOperation("patch")>]
        member _.Patch (app : WebApplication, pattern : Pattern, handler : Func<_, _, _, _, _, _, _, _, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) =
            app.MapPatch (pattern, handler=handler)
            |> RouteHandlerBuilder.withTag (Pattern.toGroupName pattern)
            |> defaultArg (configure |> Option.map ((<<) ignore)) ignore
            app

        /// Adds an HTTP PATCH endpoint.
        [<CustomOperation("patch")>]
        member _.Patch (app : WebApplication, pattern : Pattern, handler : Func<_, _, _, _, _, _, _, _, _, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) =
            app.MapPatch (pattern, handler=handler)
            |> RouteHandlerBuilder.withTag (Pattern.toGroupName pattern)
            |> defaultArg (configure |> Option.map ((<<) ignore)) ignore
            app

        /// Adds an HTTP PATCH endpoint.
        [<CustomOperation("patch")>]
        member _.Patch (app : WebApplication, pattern : Pattern, produces : (int * Type) list, handler : Func<'a, 'b>, ?configure : RouteHandlerBuilder -> _) =
            let handler : Delegate =
                if typeof<'a>.Equals typeof<unit> then upcast Func<'b> (fun () -> (Unchecked.unbox<Func<unit, 'b>> (box handler)).Invoke ())
                else upcast handler

            app.MapPatch (pattern, handler=handler)
            |> RouteHandlerBuilder.withTag (Pattern.toGroupName pattern)
            |> RouteHandlerBuilder.produces produces
            |> defaultArg (configure |> Option.map ((<<) ignore)) ignore

            app

        /// Adds an HTTP PATCH endpoint.
        [<CustomOperation("patch")>]
        member _.Patch (app : WebApplication, pattern : Pattern, produces : (int * Type) list, handler : Func<_, _, _>, ?configure : RouteHandlerBuilder -> _) =
            app.MapPatch (pattern, handler=handler)
            |> RouteHandlerBuilder.withTag (Pattern.toGroupName pattern)
            |> RouteHandlerBuilder.produces produces
            |> defaultArg (configure |> Option.map ((<<) ignore)) ignore
            app

        /// Adds an HTTP PATCH endpoint.
        [<CustomOperation("patch")>]
        member _.Patch (app : WebApplication, pattern : Pattern, produces : (int * Type) list, handler : Func<_, _, _, _>, ?configure : RouteHandlerBuilder -> _) =
            app.MapPatch (pattern, handler=handler)
            |> RouteHandlerBuilder.withTag (Pattern.toGroupName pattern)
            |> RouteHandlerBuilder.produces produces
            |> defaultArg (configure |> Option.map ((<<) ignore)) ignore
            app

        /// Adds an HTTP PATCH endpoint.
        [<CustomOperation("patch")>]
        member _.Patch (app : WebApplication, pattern : Pattern, produces : (int * Type) list, handler : Func<_, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) =
            app.MapPatch (pattern, handler=handler)
            |> RouteHandlerBuilder.withTag (Pattern.toGroupName pattern)
            |> RouteHandlerBuilder.produces produces
            |> defaultArg (configure |> Option.map ((<<) ignore)) ignore
            app

        /// Adds an HTTP PATCH endpoint.
        [<CustomOperation("patch")>]
        member _.Patch (app : WebApplication, pattern : Pattern, produces : (int * Type) list, handler : Func<_, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) =
            app.MapPatch (pattern, handler=handler)
            |> RouteHandlerBuilder.withTag (Pattern.toGroupName pattern)
            |> RouteHandlerBuilder.produces produces
            |> defaultArg (configure |> Option.map ((<<) ignore)) ignore
            app

        /// Adds an HTTP PATCH endpoint.
        [<CustomOperation("patch")>]
        member _.Patch (app : WebApplication, pattern : Pattern, produces : (int * Type) list, handler : Func<_, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) =
            app.MapPatch (pattern, handler=handler)
            |> RouteHandlerBuilder.withTag (Pattern.toGroupName pattern)
            |> RouteHandlerBuilder.produces produces
            |> defaultArg (configure |> Option.map ((<<) ignore)) ignore
            app

        /// Adds an HTTP PATCH endpoint.
        [<CustomOperation("patch")>]
        member _.Patch (app : WebApplication, pattern : Pattern, produces : (int * Type) list, handler : Func<_, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) =
            app.MapPatch (pattern, handler=handler)
            |> RouteHandlerBuilder.withTag (Pattern.toGroupName pattern)
            |> RouteHandlerBuilder.produces produces
            |> defaultArg (configure |> Option.map ((<<) ignore)) ignore
            app

        /// Adds an HTTP PATCH endpoint.
        [<CustomOperation("patch")>]
        member _.Patch (app : WebApplication, pattern : Pattern, produces : (int * Type) list, handler : Func<_, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) =
            app.MapPatch (pattern, handler=handler)
            |> RouteHandlerBuilder.withTag (Pattern.toGroupName pattern)
            |> RouteHandlerBuilder.produces produces
            |> defaultArg (configure |> Option.map ((<<) ignore)) ignore
            app

        /// Adds an HTTP PATCH endpoint.
        [<CustomOperation("patch")>]
        member _.Patch (app : WebApplication, pattern : Pattern, produces : (int * Type) list, handler : Func<_, _, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) =
            app.MapPatch (pattern, handler=handler)
            |> RouteHandlerBuilder.withTag (Pattern.toGroupName pattern)
            |> RouteHandlerBuilder.produces produces
            |> defaultArg (configure |> Option.map ((<<) ignore)) ignore
            app

        /// Adds an HTTP PATCH endpoint.
        [<CustomOperation("patch")>]
        member _.Patch (app : WebApplication, pattern : Pattern, produces : (int * Type) list, handler : Func<_, _, _, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) =
            app.MapPatch (pattern, handler=handler)
            |> RouteHandlerBuilder.withTag (Pattern.toGroupName pattern)
            |> RouteHandlerBuilder.produces produces
            |> defaultArg (configure |> Option.map ((<<) ignore)) ignore
            app

        /// Adds an HTTP PATCH endpoint.
        [<CustomOperation("patch")>]
        member _.Patch (app : WebApplication, pattern : Pattern, produces : (int * Type) list, handler : Func<_, _, _, _, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) =
            app.MapPatch (pattern, handler=handler)
            |> RouteHandlerBuilder.withTag (Pattern.toGroupName pattern)
            |> RouteHandlerBuilder.produces produces
            |> defaultArg (configure |> Option.map ((<<) ignore)) ignore
            app

        /// Adds an HTTP PATCH endpoint.
        [<CustomOperation("patch")>]
        member _.Patch (app : WebApplication, pattern : Pattern, produces : (int * Type) list, handler : Func<_, _, _, _, _, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) =
            app.MapPatch (pattern, handler=handler)
            |> RouteHandlerBuilder.withTag (Pattern.toGroupName pattern)
            |> RouteHandlerBuilder.produces produces
            |> defaultArg (configure |> Option.map ((<<) ignore)) ignore
            app

        /// Adds an HTTP PATCH endpoint.
        [<CustomOperation("patch")>]
        member _.Patch (app : WebApplication, pattern : Pattern, produces : (int * Type) list, handler : Func<_, _, _, _, _, _, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) =
            app.MapPatch (pattern, handler=handler)
            |> RouteHandlerBuilder.withTag (Pattern.toGroupName pattern)
            |> RouteHandlerBuilder.produces produces
            |> defaultArg (configure |> Option.map ((<<) ignore)) ignore
            app

        /// Adds an HTTP PATCH endpoint.
        [<CustomOperation("patch")>]
        member _.Patch (app : WebApplication, pattern : Pattern, produces : (int * Type) list, handler : Func<_, _, _, _, _, _, _, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) =
            app.MapPatch (pattern, handler=handler)
            |> RouteHandlerBuilder.withTag (Pattern.toGroupName pattern)
            |> RouteHandlerBuilder.produces produces
            |> defaultArg (configure |> Option.map ((<<) ignore)) ignore
            app

        /// Adds an HTTP PATCH endpoint.
        [<CustomOperation("patch")>]
        member _.Patch (app : WebApplication, pattern : Pattern, produces : (int * Type) list, handler : Func<_, _, _, _, _, _, _, _, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) =
            app.MapPatch (pattern, handler=handler)
            |> RouteHandlerBuilder.withTag (Pattern.toGroupName pattern)
            |> RouteHandlerBuilder.produces produces
            |> defaultArg (configure |> Option.map ((<<) ignore)) ignore
            app

        /// Adds an HTTP PATCH endpoint.
        [<CustomOperation("patch")>]
        member _.Patch (app : WebApplication, pattern : Pattern, produces : (int * Type) list, handler : Func<_, _, _, _, _, _, _, _, _, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) =
            app.MapPatch (pattern, handler=handler)
            |> RouteHandlerBuilder.withTag (Pattern.toGroupName pattern)
            |> RouteHandlerBuilder.produces produces
            |> defaultArg (configure |> Option.map ((<<) ignore)) ignore
            app

        /// Adds an HTTP PATCH endpoint.
        [<CustomOperation("patch")>]
        member this.Patch (builder : WebApplicationBuilder, pattern : Pattern, handler : Func<_, _>, ?configure : RouteHandlerBuilder -> _) = this.Patch (builder.Build (), pattern, handler)

        /// Adds an HTTP PATCH endpoint.
        [<CustomOperation("patch")>]
        member this.Patch (builder : WebApplicationBuilder, pattern : Pattern, handler : Func<_, _, _>, ?configure : RouteHandlerBuilder -> _) = this.Patch (builder.Build (), pattern, handler)

        /// Adds an HTTP PATCH endpoint.
        [<CustomOperation("patch")>]
        member this.Patch (builder : WebApplicationBuilder, pattern : Pattern, handler : Func<_, _, _, _>, ?configure : RouteHandlerBuilder -> _) = this.Patch (builder.Build (), pattern, handler)

        /// Adds an HTTP PATCH endpoint.
        [<CustomOperation("patch")>]
        member this.Patch (builder : WebApplicationBuilder, pattern : Pattern, handler : Func<_, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) = this.Patch (builder.Build (), pattern, handler)

        /// Adds an HTTP PATCH endpoint.
        [<CustomOperation("patch")>]
        member this.Patch (builder : WebApplicationBuilder, pattern : Pattern, handler : Func<_, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) = this.Patch (builder.Build (), pattern, handler)

        /// Adds an HTTP PATCH endpoint.
        [<CustomOperation("patch")>]
        member this.Patch (builder : WebApplicationBuilder, pattern : Pattern, handler : Func<_, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) = this.Patch (builder.Build (), pattern, handler)

        /// Adds an HTTP PATCH endpoint.
        [<CustomOperation("patch")>]
        member this.Patch (builder : WebApplicationBuilder, pattern : Pattern, handler : Func<_, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) = this.Patch (builder.Build (), pattern, handler)

        /// Adds an HTTP PATCH endpoint.
        [<CustomOperation("patch")>]
        member this.Patch (builder : WebApplicationBuilder, pattern : Pattern, handler : Func<_, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) = this.Patch (builder.Build (), pattern, handler)

        /// Adds an HTTP PATCH endpoint.
        [<CustomOperation("patch")>]
        member this.Patch (builder : WebApplicationBuilder, pattern : Pattern, handler : Func<_, _, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) = this.Patch (builder.Build (), pattern, handler)

        /// Adds an HTTP PATCH endpoint.
        [<CustomOperation("patch")>]
        member this.Patch (builder : WebApplicationBuilder, pattern : Pattern, handler : Func<_, _, _, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) = this.Patch (builder.Build (), pattern, handler)

        /// Adds an HTTP PATCH endpoint.
        [<CustomOperation("patch")>]
        member this.Patch (builder : WebApplicationBuilder, pattern : Pattern, handler : Func<_, _, _, _, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) = this.Patch (builder.Build (), pattern, handler)

        /// Adds an HTTP PATCH endpoint.
        [<CustomOperation("patch")>]
        member this.Patch (builder : WebApplicationBuilder, pattern : Pattern, handler : Func<_, _, _, _, _, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) = this.Patch (builder.Build (), pattern, handler)

        /// Adds an HTTP PATCH endpoint.
        [<CustomOperation("patch")>]
        member this.Patch (builder : WebApplicationBuilder, pattern : Pattern, handler : Func<_, _, _, _, _, _, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) = this.Patch (builder.Build (), pattern, handler)

        /// Adds an HTTP PATCH endpoint.
        [<CustomOperation("patch")>]
        member this.Patch (builder : WebApplicationBuilder, pattern : Pattern, handler : Func<_, _, _, _, _, _, _, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) = this.Patch (builder.Build (), pattern, handler)

        /// Adds an HTTP PATCH endpoint.
        [<CustomOperation("patch")>]
        member this.Patch (builder : WebApplicationBuilder, pattern : Pattern, handler : Func<_, _, _, _, _, _, _, _, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) = this.Patch (builder.Build (), pattern, handler)

        /// Adds an HTTP PATCH endpoint.
        [<CustomOperation("patch")>]
        member this.Patch (builder : WebApplicationBuilder, pattern : Pattern, handler : Func<_, _, _, _, _, _, _, _, _, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) = this.Patch (builder.Build (), pattern, handler)

        /// Adds an HTTP PATCH endpoint.
        [<CustomOperation("patch")>]
        member this.Patch (builder : WebApplicationBuilder, pattern : Pattern, produces : (int * Type) list, handler : Func<_, _>, ?configure : RouteHandlerBuilder -> _) = this.Patch (builder.Build (), pattern, produces, handler)

        /// Adds an HTTP PATCH endpoint.
        [<CustomOperation("patch")>]
        member this.Patch (builder : WebApplicationBuilder, pattern : Pattern, produces : (int * Type) list, handler : Func<_, _, _>, ?configure : RouteHandlerBuilder -> _) = this.Patch (builder.Build (), pattern, produces, handler)

        /// Adds an HTTP PATCH endpoint.
        [<CustomOperation("patch")>]
        member this.Patch (builder : WebApplicationBuilder, pattern : Pattern, produces : (int * Type) list, handler : Func<_, _, _, _>, ?configure : RouteHandlerBuilder -> _) = this.Patch (builder.Build (), pattern, produces, handler)

        /// Adds an HTTP PATCH endpoint.
        [<CustomOperation("patch")>]
        member this.Patch (builder : WebApplicationBuilder, pattern : Pattern, produces : (int * Type) list, handler : Func<_, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) = this.Patch (builder.Build (), pattern, produces, handler)

        /// Adds an HTTP PATCH endpoint.
        [<CustomOperation("patch")>]
        member this.Patch (builder : WebApplicationBuilder, pattern : Pattern, produces : (int * Type) list, handler : Func<_, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) = this.Patch (builder.Build (), pattern, produces, handler)

        /// Adds an HTTP PATCH endpoint.
        [<CustomOperation("patch")>]
        member this.Patch (builder : WebApplicationBuilder, pattern : Pattern, produces : (int * Type) list, handler : Func<_, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) = this.Patch (builder.Build (), pattern, produces, handler)

        /// Adds an HTTP PATCH endpoint.
        [<CustomOperation("patch")>]
        member this.Patch (builder : WebApplicationBuilder, pattern : Pattern, produces : (int * Type) list, handler : Func<_, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) = this.Patch (builder.Build (), pattern, produces, handler)

        /// Adds an HTTP PATCH endpoint.
        [<CustomOperation("patch")>]
        member this.Patch (builder : WebApplicationBuilder, pattern : Pattern, produces : (int * Type) list, handler : Func<_, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) = this.Patch (builder.Build (), pattern, produces, handler)

        /// Adds an HTTP PATCH endpoint.
        [<CustomOperation("patch")>]
        member this.Patch (builder : WebApplicationBuilder, pattern : Pattern, produces : (int * Type) list, handler : Func<_, _, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) = this.Patch (builder.Build (), pattern, produces, handler)

        /// Adds an HTTP PATCH endpoint.
        [<CustomOperation("patch")>]
        member this.Patch (builder : WebApplicationBuilder, pattern : Pattern, produces : (int * Type) list, handler : Func<_, _, _, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) = this.Patch (builder.Build (), pattern, produces, handler)

        /// Adds an HTTP PATCH endpoint.
        [<CustomOperation("patch")>]
        member this.Patch (builder : WebApplicationBuilder, pattern : Pattern, produces : (int * Type) list, handler : Func<_, _, _, _, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) = this.Patch (builder.Build (), pattern, produces, handler)

        /// Adds an HTTP PATCH endpoint.
        [<CustomOperation("patch")>]
        member this.Patch (builder : WebApplicationBuilder, pattern : Pattern, produces : (int * Type) list, handler : Func<_, _, _, _, _, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) = this.Patch (builder.Build (), pattern, produces, handler)

        /// Adds an HTTP PATCH endpoint.
        [<CustomOperation("patch")>]
        member this.Patch (builder : WebApplicationBuilder, pattern : Pattern, produces : (int * Type) list, handler : Func<_, _, _, _, _, _, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) = this.Patch (builder.Build (), pattern, produces, handler)

        /// Adds an HTTP PATCH endpoint.
        [<CustomOperation("patch")>]
        member this.Patch (builder : WebApplicationBuilder, pattern : Pattern, produces : (int * Type) list, handler : Func<_, _, _, _, _, _, _, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) = this.Patch (builder.Build (), pattern, produces, handler)

        /// Adds an HTTP PATCH endpoint.
        [<CustomOperation("patch")>]
        member this.Patch (builder : WebApplicationBuilder, pattern : Pattern, produces : (int * Type) list, handler : Func<_, _, _, _, _, _, _, _, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) = this.Patch (builder.Build (), pattern, produces, handler)

        /// Adds an HTTP PATCH endpoint.
        [<CustomOperation("patch")>]
        member this.Patch (builder : WebApplicationBuilder, pattern : Pattern, produces : (int * Type) list, handler : Func<_, _, _, _, _, _, _, _, _, _, _, _, _, _, _, _, _>, ?configure : RouteHandlerBuilder -> _) = this.Patch (builder.Build (), pattern, produces, handler)
#endif

[<AutoOpen>]
module WebAppBuilder =
    /// Creates a web application using computation expression syntax.
    let webApp = WebAppBuilder [||]

    /// Contains an alternative builder that accepts command line arguments.
    module CommandLine =
        /// Creates a web application using computation expression syntax
        /// and the given command line arguments.
        let webApp args = WebAppBuilder args
