namespace FSharp.AspNetCore.Builder

open System
open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
open System.Reflection
open System.Linq

/// <namespacedoc>
/// <summary>
/// Contains the <see cref="T:FSharp.AspNetCore.Builder.WebAppBuilder"/> computation expression builder.
/// </summary>
/// </namespacedoc>
///
/// <summary>
/// Creates a web application using computation expression syntax.
/// <para>
/// See also: 
/// <seealso cref="T:FSharp.AspNetCore.Builder.Gets"/>, 
/// <seealso cref="T:FSharp.AspNetCore.Builder.Posts"/>, 
/// <seealso cref="T:FSharp.AspNetCore.Builder.Puts"/>, 
/// <seealso cref="T:FSharp.AspNetCore.Builder.Deletes"/>, 
/// <seealso cref="T:FSharp.AspNetCore.Builder.Patches"/>.
/// </para>
/// </summary>
[<Sealed>]
type WebAppBuilder internal (args : string array) =
    /// <exclude/>
    member _.Yield _ = WebApplication.CreateBuilder args

    /// <summary>
    /// Applies the given function to the <see cref="T:Microsoft.AspNetCore.Builder.WebApplicationBuilder"/>
    /// being used to build the app.
    /// Useful when multiple properties of the builder need to be accessed at once.
    /// </summary>
    /// <param name="builder">The web application builder.</param>
    /// <param name="configureBuilder">The function to apply to the web application builder.</param>
    /// <example>
    /// <code>
    /// let app =
    ///     webApp {
    ///         builder (fun builder ->
    ///             builder.Services.AddSingleton
    ///                 (EnvironmentName builder.Environment.EnvironmentName))
    ///     }
    /// </code>
    /// </example>
    [<CustomOperation("builder")>]
    member _.Builder (builder : WebApplicationBuilder, configureBuilder) =
        ignore (configureBuilder builder)
        builder

    /// <summary>
    /// Applies the given function to the <see cref="P:Microsoft.AspNetCore.Builder.WebApplicationBuilder.Configuration"/>
    /// property of the <see cref="T:Microsoft.AspNetCore.Builder.WebApplicationBuilder"/> being used to build the app.
    /// </summary>
    /// <param name="builder">The web application builder.</param>
    /// <param name="configureConfiguration">The function to apply to the app configuration.</param>
    /// <example>
    /// <code>
    /// let app =
    ///     webApp {
    ///         configuration (fun config -> config.AddIniFile "config.ini")
    ///     }
    /// </code>
    /// </example>
    [<CustomOperation("configuration")>]
    member _.Configuration (builder : WebApplicationBuilder, configureConfiguration) =
        ignore (configureConfiguration builder.Configuration)
        builder

    /// <summary>
    /// Applies the given action to the <see cref="P:Microsoft.AspNetCore.Builder.WebApplicationBuilder.Logging"/>
    /// property of the <see cref="T:Microsoft.AspNetCore.Builder.WebApplicationBuilder"/> being used to build the app.
    /// </summary>
    /// <param name="builder">The web application builder.</param>
    /// <param name="configureLogging">The function to apply to the logging builder.</param>
    /// <example>
    /// <code>
    /// let app =
    ///     webApp {
    ///         logging (fun logging ->
    ///             logging.ClearProviders ()
    ///             logging.AddConsole ())
    ///     }
    /// </code>
    /// </example>
    [<CustomOperation("logging")>]
    member _.Logging (builder : WebApplicationBuilder, configureLogging) =
        ignore (configureLogging builder.Logging)
        builder

    /// <summary>
    /// Applies the given action to the <see cref="P:Microsoft.AspNetCore.Builder.WebApplicationBuilder.Services"/>
    /// and <see cref="P:Microsoft.AspNetCore.Builder.WebApplicationBuilder.Configuration"/> properties
    /// of the <see cref="T:Microsoft.AspNetCore.Builder.WebApplicationBuilder"/> being used to build the app.
    /// <para>
    /// See also: <seealso cref="M:FSharp.AspNetCore.Builder.Priority1.Services"/>
    /// </para>
    /// </summary>
    /// <param name="builder">The web application builder.</param>
    /// <param name="configureServices">The function to apply to the web application builder's service collection.</param>
    /// <example>
    /// <code>
    /// let app =
    ///     webApp {
    ///         services (fun services config ->
    ///             services.Configure&lt;MyOptions&gt; ("MyOptionsName", config.GetSection "MyOptionsSection")
    ///             services.AddEndpointsApiExplorer ()
    ///             services.AddSwaggerGen ()
    ///             services.AddControllers ())
    ///     }
    /// </code>
    /// </example>
    [<CustomOperation("services")>]
    member _.Services (builder : WebApplicationBuilder, configureServices) =
        ignore (configureServices builder.Services builder.Configuration)
        builder

    /// <summary>
    /// Applies the given action to the <see cref="P:Microsoft.AspNetCore.Builder.WebApplicationBuilder.Host"/>
    /// property of the <see cref="T:Microsoft.AspNetCore.Builder.WebApplicationBuilder"/> being used to build the app.
    /// </summary>
    /// <param name="builder">The web application builder.</param>
    /// <param name="configureHost">The function to apply to the host builder.</param>
    /// <example>
    /// <code>
    /// let app =
    ///     webApp {
    ///         host (fun host ->
    ///             host.ConfigureHostOptions (fun options ->
    ///                 options.BackgroundServiceExceptionBehavior &lt;-
    ///                     BackgroundServiceExceptionBehavior.StopHost))
    ///     }
    /// </code>
    /// </example>
    [<CustomOperation("host")>]
    member _.Host (builder : WebApplicationBuilder, configureHost) =
        ignore (configureHost builder.Host)
        builder

    /// <summary>
    /// Applies the given action to the <see cref="P:Microsoft.AspNetCore.Builder.WebApplicationBuilder.WebHost"/>
    /// property of the <see cref="T:Microsoft.AspNetCore.Builder.WebApplicationBuilder"/> being used to build the app.
    /// </summary>
    /// <param name="builder">The web application builder.</param>
    /// <param name="configureWebHost">The function to apply to the web host builder.</param>
    /// <example>
    /// <code>
    /// let app =
    ///     webApp {
    ///         webHost (fun webHost ->
    ///             webHost.ConfigureAppConfiguration (fun context _configBuilder ->
    ///                 context.HostingEnvironment.ApplicationName &lt;- "MyApp"))
    ///     }
    /// </code>
    /// </example>
    [<CustomOperation("webHost")>]
    member _.WebHost (builder : WebApplicationBuilder, configureWebHost) =
        ignore (configureWebHost builder.WebHost)
        builder

    /// <summary>
    /// Adds the ability to read configuration from environment variables.
    /// </summary>
    /// <param name="builder">The web application builder.</param>
    /// <example>
    /// <code>
    /// let app =
    ///     webApp {
    ///         environmentVariables
    ///     }
    /// </code>
    /// </example>
    [<CustomOperation("environmentVariables")>]
    member _.EnvironmentVariables (builder : WebApplicationBuilder) =
        ignore <| builder.Configuration.AddEnvironmentVariables ()
        builder

    /// <summary>
    /// Adds a required JSON configuration file to the <see cref="T:Microsoft.AspNetCore.Builder.WebApplicationBuilder"/>'s
    /// <see cref="P:Microsoft.AspNetCore.Builder.WebApplicationBuilder.Configuration"/>.
    /// </summary>
    /// <param name="builder">The web application builder.</param>
    /// <param name="path">The path of the JSON configuration file.</param>
    /// <example>
    /// <code>
    /// let app =
    ///     webApp {
    ///         jsonFile "auth.json"
    ///     }
    /// </code>
    /// </example>
    [<CustomOperation("jsonFile")>]
    member _.JsonFile (builder : WebApplicationBuilder, path : string) =
        ignore <| builder.Configuration.AddJsonFile (path, optional=false)
        builder

    /// <summary>
    /// Adds an optional JSON configuration file to the <see cref="T:Microsoft.AspNetCore.Builder.WebApplicationBuilder"/>'s
    /// <see cref="P:Microsoft.AspNetCore.Builder.WebApplicationBuilder.Configuration"/>.
    /// </summary>
    /// <param name="builder">The web application builder.</param>
    /// <param name="path">The path of the JSON configuration file.</param>
    /// <example>
    /// <code>
    /// let app =
    ///     webApp {
    ///         optionalJsonFile "auth.json"
    ///     }
    /// </code>
    /// </example>
    [<CustomOperation("optionalJsonFile")>]
    member _.OptionalJsonFile (builder : WebApplicationBuilder, path : string) =
        ignore <| builder.Configuration.AddJsonFile (path, optional=true)
        builder

    /// <summary>
    /// Adds a singleton service of the type specified in <paramref name="serviceType"/> to the
    /// app's service collection.
    /// </summary>
    /// <param name="builder">The web application builder.</param>
    /// <param name="serviceType">The type of the service to add.</param>
    /// <example>
    /// <code lang="fsharp">
    /// let app =
    ///     webApp {
    ///         singleton typeof&lt;Service&gt;
    ///     }
    /// </code>
    /// </example>
    [<CustomOperation("singleton")>]
    member _.Singleton (builder : WebApplicationBuilder, serviceType : Type) =
        ignore <| builder.Services.AddSingleton serviceType
        builder

    /// <summary>
    /// Adds a singleton service of the type specified in <paramref name="serviceType"/> with
    /// an implementation of the type specified in <paramref name="implementationType"/> to the
    /// app's service collection.
    /// </summary>
    /// <param name="builder">The web application builder.</param>
    /// <param name="serviceType">The type of the service to add.</param>
    /// <param name="implementationType">The type of the service implementation to add.</param>
    /// <example>
    /// <code lang="fsharp">
    /// let app =
    ///     webApp {
    ///         singleton typeof&lt;IService&gt; typeof&lt;Service&gt;
    ///     }
    /// </code>
    /// </example>
    [<CustomOperation("singleton")>]
    member _.Singleton (builder : WebApplicationBuilder, serviceType : Type, implementationType : Type) =
        ignore <| builder.Services.AddSingleton (serviceType, implementationType)
        builder

    /// <summary>
    /// Adds a singleton service of the type specified in <paramref name="serviceType"/> using
    /// an implementation provided by applying the given <paramref name="implementationFactory"/> to the
    /// app's service provider.
    /// </summary>
    /// <param name="builder">The web application builder.</param>
    /// <param name="serviceType">The type of the service to add.</param>
    /// <param name="implementationFactory">A function that produces the service implementation.</param>
    /// <example>
    /// <code lang="fsharp">
    /// let app =
    ///     webApp {
    ///         singleton
    ///             typeof&lt;IService&gt;
    ///             (fun serviceProvider -> Service (serviceProvider.GetRequiredService&lt;OtherService&gt; ()))
    ///     }
    /// </code>
    /// </example>
    [<CustomOperation("singleton")>]
    member _.Singleton (builder : WebApplicationBuilder, serviceType : Type, implementationFactory : IServiceProvider -> 'TImplementation) =
        ignore <| builder.Services.AddSingleton (serviceType=serviceType, implementationFactory=(implementationFactory >> box))
        builder

    /// <summary>
    /// Adds a singleton service of the type specified in <paramref name="serviceType"/> using
    /// the given <paramref name="implementationInstance"/>.
    /// </summary>
    /// <param name="builder">The web application builder.</param>
    /// <param name="serviceType">The type of the service to add.</param>
    /// <param name="implementationInstance">An object to use as the service implementation.</param>
    /// <example>
    /// <code lang="fsharp">
    /// let app =
    ///     webApp {
    ///         singleton typeof&lt;IService&gt; (Service ())
    ///     }
    /// </code>
    /// </example>
    [<CustomOperation("singleton")>]
    member _.Singleton (builder : WebApplicationBuilder, serviceType : Type, implementationInstance : obj) =
        ignore <| builder.Services.AddSingleton (serviceType=serviceType, implementationInstance=implementationInstance)
        builder

    /// <summary>
    /// Adds a singleton service using
    /// the given <paramref name="implementationInstance"/>.
    /// </summary>
    /// <param name="builder">The web application builder.</param>
    /// <param name="implementationInstance">An object to use as the service implementation.</param>
    /// <example>
    /// <code lang="fsharp">
    /// let app =
    ///     webApp {
    ///         singleton (Service ())
    ///     }
    /// </code>
    /// </example>
    [<CustomOperation("singleton")>]
    member _.Singleton (builder : WebApplicationBuilder, implementationInstance : 'TService) =
        ignore <| builder.Services.AddSingleton<'TService> (implementationInstance=implementationInstance)
        builder

    /// <summary>
    /// Adds a scoped service of the type specified in <paramref name="serviceType"/> to the
    /// app's service collection.
    /// </summary>
    /// <param name="builder">The web application builder.</param>
    /// <param name="serviceType">The type of the service to add.</param>
    /// <example>
    /// <code lang="fsharp">
    /// let app =
    ///     webApp {
    ///         scoped typeof&lt;Service&gt;
    ///     }
    /// </code>
    /// </example>
    [<CustomOperation("scoped")>]
    member _.Scoped (builder : WebApplicationBuilder, serviceType : Type) =
        ignore <| builder.Services.AddScoped serviceType
        builder

    /// <summary>
    /// Adds a scoped service of the type specified in <paramref name="serviceType"/> with
    /// an implementation of the type specified in <paramref name="implementationType"/> to the
    /// app's service collection.
    /// </summary>
    /// <param name="builder">The web application builder.</param>
    /// <param name="serviceType">The type of the service to add.</param>
    /// <param name="implementationType">The type of the service implementation to add.</param>
    /// <example>
    /// <code lang="fsharp">
    /// let app =
    ///     webApp {
    ///         scoped typeof&lt;IService&gt; typeof&lt;Service&gt;
    ///     }
    /// </code>
    /// </example>
    [<CustomOperation("scoped")>]
    member _.Scoped (builder : WebApplicationBuilder, serviceType : Type, implementationType : Type) =
        ignore <| builder.Services.AddScoped (serviceType, implementationType)
        builder

    /// <summary>
    /// Adds a scoped service of the type specified in <paramref name="serviceType"/> using
    /// an implementation provided by applying the given <paramref name="implementationFactory"/> to the
    /// app's service provider.
    /// </summary>
    /// <param name="builder">The web application builder.</param>
    /// <param name="serviceType">The type of the service to add.</param>
    /// <param name="implementationFactory">A function that produces the service implementation.</param>
    /// <example>
    /// <code lang="fsharp">
    /// let app =
    ///     webApp {
    ///         scoped
    ///             typeof&lt;IService&gt;
    ///             (fun serviceProvider -> Service (serviceProvider.GetRequiredService&lt;OtherService&gt; ()))
    ///     }
    /// </code>
    /// </example>
    [<CustomOperation("scoped")>]
    member _.Scoped (builder : WebApplicationBuilder, serviceType : Type, implementationFactory : IServiceProvider -> 'TImplementation) =
        ignore <| builder.Services.AddScoped (serviceType=serviceType, implementationFactory=(implementationFactory >> box))
        builder

    /// <summary>
    /// Adds a transient service of the type specified in <paramref name="serviceType"/> to the
    /// app's service collection.
    /// </summary>
    /// <param name="builder">The web application builder.</param>
    /// <param name="serviceType">The type of the service to add.</param>
    /// <example>
    /// <code lang="fsharp">
    /// let app =
    ///     webApp {
    ///         transient typeof&lt;Service&gt;
    ///     }
    /// </code>
    /// </example>
    [<CustomOperation("transient")>]
    member _.Transient (builder : WebApplicationBuilder, serviceType : Type) =
        ignore <| builder.Services.AddTransient serviceType
        builder

    /// <summary>
    /// Adds a transient service of the type specified in <paramref name="serviceType"/> with
    /// an implementation of the type specified in <paramref name="implementationType"/> to the
    /// app's service collection.
    /// </summary>
    /// <param name="builder">The web application builder.</param>
    /// <param name="serviceType">The type of the service to add.</param>
    /// <param name="implementationType">The type of the service implementation to add.</param>
    /// <example>
    /// <code lang="fsharp">
    /// let app =
    ///     webApp {
    ///         transient typeof&lt;IService&gt; typeof&lt;Service&gt;
    ///     }
    /// </code>
    /// </example>
    [<CustomOperation("transient")>]
    member _.Transient (builder : WebApplicationBuilder, serviceType : Type, implementationType : Type) =
        ignore <| builder.Services.AddTransient (serviceType, implementationType)
        builder

    /// <summary>
    /// Adds a transient service of the type specified in <paramref name="serviceType"/> using
    /// an implementation provided by applying the given <paramref name="implementationFactory"/> to the
    /// app's service provider.
    /// </summary>
    /// <param name="builder">The web application builder.</param>
    /// <param name="serviceType">The type of the service to add.</param>
    /// <param name="implementationFactory">A function that produces the service implementation.</param>
    /// <example>
    /// <code lang="fsharp">
    /// let app =
    ///     webApp {
    ///         transient
    ///             typeof&lt;IService&gt;
    ///             (fun serviceProvider -> Service (serviceProvider.GetRequiredService&lt;OtherService&gt; ()))
    ///     }
    /// </code>
    /// </example>
    [<CustomOperation("transient")>]
    member _.Transient (builder : WebApplicationBuilder, serviceType : Type, implementationFactory : IServiceProvider -> 'TImplementation) =
        ignore <| builder.Services.AddTransient (serviceType=serviceType, implementationFactory=(implementationFactory >> box))
        builder

    /// <summary>
    /// Adds a singleton service produced by applying the given <paramref name="configure"/> function
    /// to the <see cref="T:Microsoft.AspNetCore.Builder.WebApplicationBuilder"/>'s
    /// <see cref="P:Microsoft.AspNetCore.Builder.WebApplicationBuilder.Configuration"/> property.
    /// </summary>
    /// <example>
    /// <code lang="fsharp">
    /// let app =
    ///     webApp {
    ///         fromConfig (fun config -> Service config["MyKey"])
    ///     }
    /// </code>
    /// </example>
    [<CustomOperation("fromConfig")>]
    member _.FromConfiguration (builder : WebApplicationBuilder, configure : IConfiguration -> 'ConfiguredValue when 'ConfiguredValue : not struct) =
        ignore <| builder.Services.AddSingleton (implementationInstance=configure builder.Configuration)
        builder

    /// <summary>
    /// Applies the given <paramref name="configureOptions"/> action to the registered options
    /// of the specified type.
    /// </summary>
    /// <param name="builder">The web application builder.</param>
    /// <param name="configureOptions">The action to apply to the options.</param>
    /// <example>
    /// <code lang="fsharp">
    /// let app =
    ///     webApp {
    ///         configure (fun (jsonOptions : JsonOptions) ->
    ///             jsonOptions.SerializerOptions.Converters.Add (JsonStringEnumConverter ()))
    ///     }
    /// </code>
    /// </example>
    [<CustomOperation("configure")>]
    member _.Configure (builder : WebApplicationBuilder, configureOptions : 'TOptions -> _) =
        ignore <| builder.Services.Configure (fun service -> ignore (configureOptions service))
        builder

    /// <summary>
    /// Adds a strongly-typed connection string to the app's service collection.
    /// Performs a lookup in the app's configuration at the path <c>"ConnectionStrings:&lt;name&gt;"</c>.
    /// </summary>
    /// <param name="builder">The web application builder.</param>
    /// <param name="name">The name of the connection string to look up in the <c>ConnectionStrings</c> section of the app configuration.</param>
    /// <param name="ctor">The constructor to pass the configuration value into.</param>
    /// <exception cref="System.InvalidOperationException">Raised when no connection string with the given <paramref name="name"/> is found in the app's configuration.</exception>
    /// <example>
    /// <code lang="json">
    /// {
    ///   "ConnectionStrings": {
    ///     "SqlDb": "Data Source=localhost\\LocalDb;Initial Catalog=MyDb;IntegratedSecurity=true"
    ///   }
    /// }
    /// </code>
    /// <code lang="fsharp">
    /// type SqlDbConnectionString = SqlDbConnectionString of string
    ///
    /// let app =
    ///     webApp {
    ///         connectionString "SqlDb" SqlDbConnectionString
    ///     }
    /// </code>
    /// </example>
    [<CustomOperation("connectionString")>]
    member this.ConnectionString (builder : WebApplicationBuilder, name : string, ctor : string -> 'ConnectionString when 'ConnectionString : not struct) =
        this.FromConfiguration (builder, fun config ->
            let connectionString = config.GetConnectionString name

            if String.IsNullOrWhiteSpace connectionString then
                invalidOp $"No connection string '{name}' found."

            ctor connectionString)

    /// <summary>
    /// Adds a strongly-typed configuration value to the app's service collection.
    /// </summary>
    /// <param name="builder">The web application builder.</param>
    /// <param name="key">The key to look up in the app configuration.</param>
    /// <param name="ctor">The constructor to pass the configuration value into.</param>
    /// <exception cref="System.InvalidOperationException">Raised when no value with the given <paramref name="key"/> is found in the app's configuration.</exception>
    /// <example>
    /// <code lang="json">
    /// {
    ///   "AppSettings": {
    ///     "SqlCommandTimeout": "00:05:00"
    ///   }
    /// }
    /// </code>
    /// <code lang="fsharp">
    /// type SqlCommandTimeout = SqlCommandTimeout of TimeSpan
    ///
    /// let app =
    ///     webApp {
    ///         configurationValue "AppSettings:SqlCommandTimeout" SqlCommandTimeout
    ///     }
    /// </code>
    /// </example>
    [<CustomOperation("configurationValue")>]
    member this.ConfigurationValue (builder : WebApplicationBuilder, key : string, ctor : 'Value -> 'ConfiguredValue when 'ConfiguredValue : not struct) =
        this.FromConfiguration (builder, fun config ->
            let value = config.GetRequiredSection key

            if String.IsNullOrWhiteSpace value.Value then
                invalidOp $"No value found for key '{key}'."

            ctor (value.Get<'Value> ()))

    /// <summary>
    /// Adds a hosted service to the app's service collection.
    /// </summary>
    /// <exception cref="System.InvalidOperationException">Raised if <paramref name="serviceType"/> does not implement <see cref="T:Microsoft.Extensions.Hosting.IHostedService"/>.</exception>
    /// <example>
    /// <code lang="fsharp">
    /// let app =
    ///     webApp {
    ///         hostedService typeof&lt;MyHostedService&gt;
    ///     }
    /// </code>
    /// </example>
    [<CustomOperation("hostedService")>]
    member _.HostedService (builder : WebApplicationBuilder, serviceType : Type) =
        if not (serviceType.IsAssignableTo typeof<IHostedService>) then
            invalidOp $"The type '{serviceType.Name}' does not implement {nameof IHostedService}."

        ignore <| builder.Services.AddSingleton (typeof<IHostedService>, serviceType)
        builder

    /// <summary>
    /// Adds a hosted service to the app's service collection.
    /// </summary>
    /// <example>
    /// <code lang="fsharp">
    /// let app =
    ///     webApp {
    ///         hostedService (fun serviceProvider ->
    ///             MyHostedService (serviceProvider.GetRequireService&lt;OtherService&gt; ()))
    ///     }
    /// </code>
    /// </example>
    [<CustomOperation("hostedService")>]
    member _.HostedService (builder : WebApplicationBuilder, implementationFactory) =
        ignore <| builder.Services.AddHostedService (fun sp -> implementationFactory sp)
        builder

    /// <summary>
    /// Calls <see cref="M:Microsoft.AspNetCore.Builder.WebApplicationBuilder.Build()"/> on the web application builder,
    /// returning a <see cref="T:Microsoft.AspNetCore.Builder.WebApplication"/>.
    /// </summary>
    [<CustomOperation("build")>]
    member _.Build (builder : WebApplicationBuilder, ()) =
        builder.Build ()

    /// <summary>
    /// Applies the given function to the web application builder
    /// immediately before calling <see cref="M:Microsoft.AspNetCore.Builder.WebApplicationBuilder.Build()"/> on it,
    /// returning a <see cref="T:Microsoft.AspNetCore.Builder.WebApplication"/>.
    /// Any following operations must operate on the built <see cref="T:Microsoft.AspNetCore.Builder.WebApplication"/>,
    /// not the <see cref="T:Microsoft.AspNetCore.Builder.WebApplicationBuilder"/>.
    /// </summary>
    /// <param name="builder">The web application builder.</param>
    /// <param name="configureBuilder">The function to apply to the web application builder.</param>
    /// <example>
    /// Use this operation to enable easy integration testing.
    /// 
    /// <para>
    /// In your Program.fs:
    /// </para>
    /// 
    /// <code lang="fsharp">
    /// let app configureBuilder =
    ///     webApp {
    ///         buildWith configureBuilder
    ///         get "/hello" (fun () -> "🌎")
    ///     }
    ///
    /// (app ignore).Run ()
    /// </code>
    /// 
    /// In your test file:
    /// 
    /// <code lang="fsharp">
    /// [&lt;Fact&gt;]
    /// let ``Hello, world`` () =
    ///     task {
    ///         use app = Program.app (fun builder -> builder.WebHost.UseTestServer ())
    ///         do! app.StartAsync ()
    ///         use client = app.GetTestClient ()
    ///         let! response = client.GetStringAsync "hello"
    ///         Assert.Equal ("🌎", response)
    ///     }
    ///     |> Async.AwaitTask
    ///     |> Async.RunSynchronously
    /// </code>
    /// </example>
    [<CustomOperation("buildWith")>]
    member this.BuildWith (builder : WebApplicationBuilder, configureBuilder) =
        this.Build (this.Builder (builder, configureBuilder), ())

    /// <summary>
    /// Applies the given action to the built <see cref="T:Microsoft.AspNetCore.Builder.WebApplication"/>.
    /// </summary>
    /// <param name="app">The built web application.</param>
    /// <param name="configureApp">The function to apply to the built web application.</param>
    /// <example>
    /// <code lang="fsharp">
    /// let app =
    ///     webApp {
    ///         get "/hello" (fun () -> "🌎")
    ///         webApp (fun app ->
    ///             app.UseSwagger ()
    ///             app.UseSwaggerUI ()
    ///             app.MapControllers ())
    ///     }
    /// </code>
    /// </example>
    [<CustomOperation("webApp")>]
    member _.WebApp (app : WebApplication, configureApp) =
        ignore (configureApp app)
        app

    /// <summary>
    /// Calls <see cref="M:Microsoft.AspNetCore.Builder.WebApplicationBuilder.Build"/> and
    /// applies the given action to the resulting <see cref="T:Microsoft.AspNetCore.Builder.WebApplication"/>.
    /// </summary>
    /// <param name="builder">The web application builder.</param>
    /// <param name="configureApp">The function to apply to the built web application.</param>
    /// <example>
    /// <code lang="fsharp">
    /// let app =
    ///     webApp {
    ///         webApp (fun app ->
    ///             app.UseSwagger ()
    ///             app.UseSwaggerUI ()
    ///             app.MapControllers ())
    ///     }
    /// </code>
    /// </example>
    [<CustomOperation("webApp")>]
    member this.WebApp (builder : WebApplicationBuilder, configureApp) = this.WebApp (builder.Build (), configureApp)

    /// <exclude/>
    member this.Run (builder : WebApplicationBuilder) = this.Run (builder.Build ())

    /// <exclude/>
    member _.Run (app : WebApplication) = app

/// <summary>
/// A route pattern.
/// </summary>
/// <example>
/// <code lang="fsharp">
/// "/"
/// </code>
/// <code lang="fsharp">
/// "/api/clowns"
/// </code>
/// <code lang="fsharp">
/// "/api/clowns/{id}"
/// </code>
/// </example>
type Pattern = string

module internal Pattern =
    let toGroupName (pattern : Pattern) =
        if isNull pattern then nullArg (nameof pattern) else
        pattern.Split ('/', StringSplitOptions.RemoveEmptyEntries)
        |> Array.tryFindBack (not << String.exists (function '{' | '}' -> true | _ -> false))
        |> Option.bind (fun last -> last.Split ('?', StringSplitOptions.RemoveEmptyEntries) |> Array.tryHead)
        |> Option.toObj

open Microsoft.AspNetCore.Http
open Microsoft.AspNetCore.Routing

module internal RouteHandlerBuilder =
    let withTag (tag : string) (routeHandlerBuilder : RouteHandlerBuilder) = routeHandlerBuilder.WithTags tag

    let produces produces (routeHandlerBuilder : RouteHandlerBuilder) =
        (routeHandlerBuilder, produces) ||> List.fold (fun route (statusCode, responseTy) ->
            route.Produces (statusCode=statusCode, responseType=responseTy, contentType=null, additionalContentTypes=[||]))

/// <summary>
/// Contains operations for adding HTTP <c>GET</c> endpoints.
/// </summary>
/// <example>
/// <code lang="fsharp">
/// let app =
///     webApp {
///         get "/hello" (fun () -> "🌎")
///     }
/// </code>
/// <code lang="fsharp">
/// let app =
///     webApp {
///         get "/api/clowns/{id}" (fun (db : IDataAccess) id -> db.Get id)
///     }
/// </code>
/// <code lang="fsharp">
/// open type Microsoft.AspNetCore.Http.StatusCodes
///
/// let app =
///     webApp {
///         get "/api/clowns/{id}"
///             [Status200OK, typeof&lt;Clown&gt;]
///             (fun (db : IDataAccess) id -> db.Get id)
///     }
/// </code>
/// <code lang="fsharp">
/// open type Microsoft.AspNetCore.Http.StatusCodes
///
/// let app =
///     webApp {
///         get "/hello" [
///             Status200OK, typeof&lt;string&gt;
///             Status404NotFound, null
///         ] (fun () ->
///             if DateTime.Today.DayOfWeek = DayOfWeek.Monday then Results.NotFound ()
///             else Results.Ok "🌎"
///         ) (fun routeHandler ->
///             routeHandler.WithOpenApi (fun op -> op.Description &lt;- "Hello, 🌎 — unless it's Monday."; op)
///             routeHandler.AllowAnonymous ()
///         )
///     }
/// </code>
/// </example>

[<AutoOpen>]     
module internal DelegateCreatorsTypes =
    let mapAndConfigureEndpoint map (route: string) (handler: Delegate) configure (endpointRouteBuilder: WebApplication) =
        let get = 
            endpointRouteBuilder
            |> map route handler
        get
        |> defaultArg (configure |> Option.map ((<<) ignore)) ignore

        get

    [<AbstractClass>]
    type DelegateCreatorBase() =
        abstract member Delegate: Delegate
    
    type DelegateCreator<'Signature>(``delegate``: Delegate, createdFrom: 'Signature) =
        inherit DelegateCreatorBase()

        member this.Params =
            (``delegate``, createdFrom)

        member this.GetParamsInfo() =
            let fDeclaredMethods = createdFrom.GetType().GetTypeInfo().DeclaredMethods.ToArray()
            let fParams = fDeclaredMethods[0].GetParameters()
            fParams

        member this.FixIndirectParams(``delegate``: Delegate) = 
            let fParams = this.GetParamsInfo ()
            let mParametersField = ``delegate``.Method.GetType().GetRuntimeFields().Single(fun p -> p.Name = "m_parameters")
            mParametersField.SetValue(``delegate``.Method, fParams)
            ``delegate``
            
        override this.Delegate: Delegate =
            this.FixIndirectParams ``delegate``

    type DelegateCreator2<'a, 'b>(f: 'a -> 'b) = 
        inherit DelegateCreator<'a -> 'b>(Func<_, _>(f), f)
        override this.Delegate: Delegate =
            let (fDelegate, _) = base.Params
            let unitFunctionPassed = typeof<'a>.Equals typeof<unit>
            
            let ``delegate`` : Delegate =
                if unitFunctionPassed then upcast Func<'b> (fun () -> (Unchecked.unbox<Func<unit, 'b>> (box fDelegate)).Invoke ()) else fDelegate
                    
            if unitFunctionPassed then
                ``delegate``
            else
                this.FixIndirectParams ``delegate``
            
    type DelegateCreator3<'a, 'b, 'c>(f: 'a -> 'b -> 'c) = 
        inherit DelegateCreator<'a -> 'b -> 'c>(Func<_, _, _>(f), f)
        
    type DelegateCreator4<'a, 'b, 'c, 'd>(f: 'a -> 'b -> 'c -> 'd) = 
        inherit DelegateCreator<'a -> 'b -> 'c -> 'd>(Func<_, _, _, _>(f), f)
        
    type DelegateCreator5<'a, 'b, 'c, 'd, 'e>(f: 'a -> 'b -> 'c -> 'd -> 'e) = 
        inherit DelegateCreator<'a -> 'b -> 'c -> 'd -> 'e>(Func<_, _, _, _, _>(f), f)
        
    type DelegateCreator6<'a, 'b, 'c, 'd, 'e, 'f>(f: 'a -> 'b -> 'c -> 'd -> 'e -> 'f) = 
        inherit DelegateCreator<'a -> 'b -> 'c -> 'd -> 'e -> 'f>(Func<_, _, _, _, _, _>(f), f)
        
    type DelegateCreator7<'a, 'b, 'c, 'd, 'e, 'f, 'g>(f: 'a -> 'b -> 'c -> 'd -> 'e -> 'f -> 'g) = 
        inherit DelegateCreator<'a -> 'b -> 'c -> 'd -> 'e -> 'f -> 'g>(Func<_, _, _, _, _, _, _>(f), f)
        
    type DelegateCreator8<'a, 'b, 'c, 'd, 'e, 'f, 'g, 'h>(f: 'a -> 'b -> 'c -> 'd -> 'e -> 'f -> 'g -> 'h) = 
        inherit DelegateCreator<'a -> 'b -> 'c -> 'd -> 'e -> 'f -> 'g -> 'h>(Func<_, _, _, _, _, _, _, _>(f), f)
            
    type DelegateCreator9<'a, 'b, 'c, 'd, 'e, 'f, 'g, 'h, 'i>(f: 'a -> 'b -> 'c -> 'd -> 'e -> 'f -> 'g -> 'h -> 'i) = 
        inherit DelegateCreator<'a -> 'b -> 'c -> 'd -> 'e -> 'f -> 'g -> 'h -> 'i>(Func<_, _, _, _, _, _, _, _, _>(f), f)
            
    type DelegateCreator10<'a, 'b, 'c, 'd, 'e, 'f, 'g, 'h, 'i, 'j>(f: 'a -> 'b -> 'c -> 'd -> 'e -> 'f -> 'g -> 'h -> 'i -> 'j) = 
        inherit DelegateCreator<'a -> 'b -> 'c -> 'd -> 'e -> 'f -> 'g -> 'h -> 'i -> 'j>(Func<_, _, _, _, _, _, _, _, _, _>(f), f)
            
    type DelegateCreator11<'a, 'b, 'c, 'd, 'e, 'f, 'g, 'h, 'i, 'j, 'k>(f: 'a -> 'b -> 'c -> 'd -> 'e -> 'f -> 'g -> 'h -> 'i -> 'j -> 'k) = 
        inherit DelegateCreator<'a -> 'b -> 'c -> 'd -> 'e -> 'f -> 'g -> 'h -> 'i -> 'j -> 'k>(Func<_, _, _, _, _, _, _, _, _, _, _>(f), f)
            
    type DelegateCreator12<'a, 'b, 'c, 'd, 'e, 'f, 'g, 'h, 'i, 'j, 'k, 'l>(f: 'a -> 'b -> 'c -> 'd -> 'e -> 'f -> 'g -> 'h -> 'i -> 'j -> 'k -> 'l) = 
        inherit DelegateCreator<'a -> 'b -> 'c -> 'd -> 'e -> 'f -> 'g -> 'h -> 'i -> 'j -> 'k -> 'l>(Func<_, _, _, _, _, _, _, _, _, _, _, _>(f), f)
            
    type DelegateCreator13<'a, 'b, 'c, 'd, 'e, 'f, 'g, 'h, 'i, 'j, 'k, 'l, 'm>(f: 'a -> 'b -> 'c -> 'd -> 'e -> 'f -> 'g -> 'h -> 'i -> 'j -> 'k -> 'l -> 'm) = 
        inherit DelegateCreator<'a -> 'b -> 'c -> 'd -> 'e -> 'f -> 'g -> 'h -> 'i -> 'j -> 'k -> 'l -> 'm>(Func<_, _, _, _, _, _, _, _, _, _, _, _, _>(f), f)
            
    type DelegateCreator14<'a, 'b, 'c, 'd, 'e, 'f, 'g, 'h, 'i, 'j, 'k, 'l, 'm, 'n>(f: 'a -> 'b -> 'c -> 'd -> 'e -> 'f -> 'g -> 'h -> 'i -> 'j -> 'k -> 'l -> 'm -> 'n) = 
        inherit DelegateCreator<'a -> 'b -> 'c -> 'd -> 'e -> 'f -> 'g -> 'h -> 'i -> 'j -> 'k -> 'l -> 'm -> 'n>(Func<_, _, _, _, _, _, _, _, _, _, _, _, _, _>(f), f)
            
    type DelegateCreator15<'a, 'b, 'c, 'd, 'e, 'f, 'g, 'h, 'i, 'j, 'k, 'l, 'm, 'n, 'o>(f: 'a -> 'b -> 'c -> 'd -> 'e -> 'f -> 'g -> 'h -> 'i -> 'j -> 'k -> 'l -> 'm -> 'n -> 'o) = 
        inherit DelegateCreator<'a -> 'b -> 'c -> 'd -> 'e -> 'f -> 'g -> 'h -> 'i -> 'j -> 'k -> 'l -> 'm -> 'n -> 'o>(Func<_, _, _, _, _, _, _, _, _, _, _, _, _, _, _>(f), f)
            
    type DelegateCreator16<'a, 'b, 'c, 'd, 'e, 'f, 'g, 'h, 'i, 'j, 'k, 'l, 'm, 'n, 'o, 'p>(f: 'a -> 'b -> 'c -> 'd -> 'e -> 'f -> 'g -> 'h -> 'i -> 'j -> 'k -> 'l -> 'm -> 'n -> 'o -> 'p) = 
        inherit DelegateCreator<'a -> 'b -> 'c -> 'd -> 'e -> 'f -> 'g -> 'h -> 'i -> 'j -> 'k -> 'l -> 'm -> 'n -> 'o -> 'p>(Func<_, _, _, _, _, _, _, _, _, _, _, _, _, _, _, _>(f), f)

    let DCMap =
        [
            typeof<DelegateCreator2<_, _>>
            typeof<DelegateCreator3<_, _, _>>
            typeof<DelegateCreator4<_, _, _, _>>
            typeof<DelegateCreator5<_, _, _, _, _>>
            typeof<DelegateCreator6<_, _, _, _, _, _>>
            typeof<DelegateCreator7<_, _, _, _, _, _, _>>
            typeof<DelegateCreator8<_, _, _, _, _, _, _, _>>
            typeof<DelegateCreator9<_, _, _, _, _, _, _, _, _>>
            typeof<DelegateCreator10<_, _, _, _, _, _, _, _, _, _>>
            typeof<DelegateCreator11<_, _, _, _, _, _, _, _, _, _, _>>
            typeof<DelegateCreator12<_, _, _, _, _, _, _, _, _, _, _, _>>
            typeof<DelegateCreator13<_, _, _, _, _, _, _, _, _, _, _, _, _>>
            typeof<DelegateCreator14<_, _, _, _, _, _, _, _, _, _, _, _, _, _>>
            typeof<DelegateCreator15<_, _, _, _, _, _, _, _, _, _, _, _, _, _, _>>
            typeof<DelegateCreator16<_, _, _, _, _, _, _, _, _, _, _, _, _, _, _, _>>
        ]
        |> List.map (fun t -> t.GetGenericArguments().Length, t.GetGenericTypeDefinition())
        |> Map.ofList

    type WebAppBuilder with
        member this.GenerateDelegate(handler) =
            let declaredMethods = handler.GetType().GetTypeInfo().DeclaredMethods.ToArray()
            let declaredMethod = declaredMethods[0]
            let parametersInfo = declaredMethod.GetParameters()
            let parametersTypes =
                parametersInfo
                |> Array.map (fun pi -> pi.ParameterType)
                |> List.ofArray
                |> (fun l -> l @ [ declaredMethod.ReturnType ])
                |> List.toArray
            let ``delegate``: Delegate =
                match Map.tryFind parametersTypes.Length DCMap with
                | Some delegateCreator ->
                    let genericDelegateCreator = delegateCreator.MakeGenericType(parametersTypes)
                    let delegateCreatorInstance = Activator.CreateInstance(genericDelegateCreator, handler :> obj) :?> DelegateCreatorBase
                    delegateCreatorInstance.Delegate
                | _ -> failwith $"No {nameof(DelegateCreator)} type found with {parametersTypes.Length} generic arguments"
            ``delegate``

        member this.CreateRoute map produces pattern handler configure (app : WebApplication) =
            app
            |> mapAndConfigureEndpoint map pattern (this.GenerateDelegate(handler)) configure
            |> RouteHandlerBuilder.withTag (Pattern.toGroupName pattern)
            |> RouteHandlerBuilder.produces produces
            |> defaultArg (configure |> Option.map ((<<) ignore)) ignore

            app

[<AutoOpen>]
module Gets =
    let private mapGet route handler (router: IEndpointRouteBuilder) = router.MapGet(route, handler = handler)

    type WebAppBuilder with

        /// <summary>
        /// Adds an HTTP <c>GET</c> endpoint.
        /// </summary>
        [<CustomOperation("get")>]
        member this.Get (app : WebApplication, pattern : Pattern, handler : 'handler, ?configure : RouteHandlerBuilder -> _) =
            this.CreateRoute mapGet [] pattern handler configure app

        /// <summary>
        /// Adds an HTTP <c>GET</c> endpoint.
        /// </summary>
        [<CustomOperation("get")>]
        member this.Get (app : WebApplication, pattern : Pattern, produces : (int * Type) list, handler : 'handler, ?configure : RouteHandlerBuilder -> _) =
            this.CreateRoute mapGet produces pattern handler configure app

        /// <summary>
        /// Adds an HTTP <c>GET</c> endpoint.
        /// </summary>
        [<CustomOperation("get")>]
        member this.Get (builder : WebApplicationBuilder, pattern : Pattern, handler : 'handler, ?configure : RouteHandlerBuilder -> _) = this.Get (builder.Build (), pattern, handler, ?configure=configure)

        /// <summary>
        /// Adds an HTTP <c>GET</c> endpoint.
        /// </summary>
        [<CustomOperation("get")>]
        member this.Get (builder : WebApplicationBuilder, pattern : Pattern, produces : (int * Type) list, handler : 'handler, ?configure : RouteHandlerBuilder -> _) = this.Get (builder.Build (), pattern, produces, handler, ?configure=configure)

/// <summary>
/// Contains operations for adding HTTP <c>POST</c> endpoints.
/// </summary>
/// <example>
/// <code lang="fsharp">
/// let app =
///     webApp {
///         post "/api/clowns" (fun (db : IDataAccess) newId clown ->
///             let clown = dataAccess.Create (newId (), clown)
///             Results.Created ($"/clowns/{clown.Id}", clown))
///     }
/// </code>
/// <code lang="fsharp">
/// let app =
///     webApp {
///         post "/clowns" [
///             Status201Created,             typeof&lt;Dtos.Get.Clown&gt;
///             Status400BadRequest,          typeof&lt;ValidationProblemDetails&gt;
///             Status409Conflict,            typeof&lt;string&gt;
///             Status500InternalServerError, typeof&lt;ProblemDetails&gt;
///         ] (fun (logger : ILogger&lt;Program&gt;) mkId (db : IDataAccess) clown ->
///             // ...
///             Results.Created ($"/clowns/{id}", clown)
///         ) (fun routeHandler ->
///             routeHandler.Accepts (typeof&lt;Dtos.Create.Clown&gt;, MediaTypeNames.Application.Json)
///             routeHandler.AddFilter&lt;MyEndpointFilter&gt; ()
///         )
///     }
/// </code>
/// </example>
[<AutoOpen>]
module Posts =
    let private mapPost route handler (router: IEndpointRouteBuilder) = router.MapPost(route, handler = handler)

    type WebAppBuilder with
        /// <summary>
        /// Adds an HTTP <c>POST</c> endpoint.
        /// </summary>
        [<CustomOperation("post")>]
        member this.Post (app : WebApplication, pattern : Pattern, handler : 'handler, ?configure : RouteHandlerBuilder -> _) =
            this.CreateRoute mapPost [] pattern handler configure app

        /// <summary>
        /// Adds an HTTP <c>POST</c> endpoint.
        /// </summary>
        [<CustomOperation("post")>]
        member this.Post (app : WebApplication, pattern : Pattern, produces : (int * Type) list, handler : 'handler, ?configure : RouteHandlerBuilder -> _) =
            this.CreateRoute mapPost produces pattern handler configure app

        /// <summary>
        /// Adds an HTTP <c>POST</c> endpoint.
        /// </summary>
        [<CustomOperation("post")>]
        member this.Post (builder : WebApplicationBuilder, pattern : Pattern, handler : 'handler, ?configure : RouteHandlerBuilder -> _) = this.Post (builder.Build (), pattern, handler, ?configure=configure)

        /// <summary>
        /// Adds an HTTP <c>POST</c> endpoint.
        /// </summary>
        [<CustomOperation("post")>]
        member this.Post (builder : WebApplicationBuilder, pattern : Pattern, produces : (int * Type) list, handler : 'handler, ?configure : RouteHandlerBuilder -> _) = this.Post (builder.Build (), pattern, produces, handler, ?configure=configure)

/// <summary>
/// Contains operations for adding HTTP <c>PUT</c> endpoints.
/// </summary>
/// <example>
/// <code lang="fsharp">
/// let app =
///     webApp {
///         put "/api/clowns/{id}" (fun (db : IDataAccess) id clown -> Results.Ok (db.Update (id, clown)))
///     }
/// </code>
/// <code lang="fsharp">
/// let app =
///     webApp {
///         put "/clowns/id" [
///             Status201Created,             typeof&lt;Dtos.Get.Clown&gt;
///             Status400BadRequest,          typeof&lt;ValidationProblemDetails&gt;
///             Status404NotFound,            typeof&lt;ValidationProblemDetails&gt;
///             Status409Conflict,            typeof&lt;string&gt;
///             Status500InternalServerError, typeof&lt;ProblemDetails&gt;
///         ] (fun (logger : ILogger&lt;Program&gt;) (db : IDataAccess) id clown ->
///             // ...
///             Results.Ok clown
///         ) (fun routeHandler ->
///             routeHandler.Accepts (typeof&lt;Dtos.Update.Clown&gt;, MediaTypeNames.Application.Json)
///             routeHandler.AddFilter&lt;MyEndpointFilter&gt; ()
///         )
///     }
/// </code>
/// </example>
[<AutoOpen>]
module Puts =
    let private mapPut route handler (router: IEndpointRouteBuilder) = router.MapPut(route, handler = handler)

    type WebAppBuilder with
        /// <summary>
        /// Adds an HTTP <c>PUT</c> endpoint.
        /// </summary>
        [<CustomOperation("put")>]
        member this.Put (app : WebApplication, pattern : Pattern, handler : 'handler, ?configure : RouteHandlerBuilder -> _) =
            this.CreateRoute mapPut [] pattern handler configure app

        /// <summary>
        /// Adds an HTTP <c>PUT</c> endpoint.
        /// </summary>
        [<CustomOperation("put")>]
        member this.Put (app : WebApplication, pattern : Pattern, produces : (int * Type) list, handler : 'handler, ?configure : RouteHandlerBuilder -> _) =
            this.CreateRoute mapPut produces pattern handler configure app

        /// <summary>
        /// Adds an HTTP <c>PUT</c> endpoint.
        /// </summary>
        [<CustomOperation("put")>]
        member this.Put (builder : WebApplicationBuilder, pattern : Pattern, handler : 'handler, ?configure : RouteHandlerBuilder -> _) = this.Put (builder.Build (), pattern, handler, ?configure=configure)

        /// <summary>
        /// Adds an HTTP <c>PUT</c> endpoint.
        /// </summary>
        [<CustomOperation("put")>]
        member this.Put (builder : WebApplicationBuilder, pattern : Pattern, produces : (int * Type) list, handler : 'handler, ?configure : RouteHandlerBuilder -> _) = this.Put (builder.Build (), pattern, produces, handler, ?configure=configure)

/// <summary>
/// Contains operations for adding HTTP <c>DELETE</c> endpoints.
/// </summary>
/// <example>
/// <code lang="fsharp">
/// let app =
///     webApp {
///         delete "/api/clowns/{id}" (fun (db : IDataAccess) id -> Results.NoContent (db.Delete id))
///     }
/// </code>
/// <code lang="fsharp">
/// let app =
///     webApp {
///         delete "/clowns/{id}" [
///             Status204NoContent,           null
///             Status500InternalServerError, typeof&lt;ProblemDetails&gt;
///         ] (fun (logger : ILogger&lt;Program&gt;) (db : IDataAccess) id ->
///             // ...
///             Results.NoContent ()
///         ) (fun routeHandler ->
///             routeHandler.AddFilter&lt;MyEndpointFilter&gt; ()
///         )
///     }
/// </code>
/// </example>
[<AutoOpen>]
module Deletes =
    let private mapDelete route handler (router: IEndpointRouteBuilder) = router.MapDelete(route, handler = handler)

    type WebAppBuilder with
        /// <summary>
        /// Adds an HTTP <c>DELETE</c> endpoint.
        /// </summary>
        [<CustomOperation("delete")>]
        member this.Delete (app : WebApplication, pattern : Pattern, handler : 'handler, ?configure : RouteHandlerBuilder -> _) =
            this.CreateRoute mapDelete [] pattern handler configure app

        /// <summary>
        /// Adds an HTTP <c>DELETE</c> endpoint.
        /// </summary>
        [<CustomOperation("delete")>]
        member this.Delete (app : WebApplication, pattern : Pattern, produces : (int * Type) list, handler : 'handler, ?configure : RouteHandlerBuilder -> _) =
            this.CreateRoute mapDelete produces pattern handler configure app

        /// <summary>
        /// Adds an HTTP <c>DELETE</c> endpoint.
        /// </summary>
        [<CustomOperation("delete")>]
        member this.Delete (builder : WebApplicationBuilder, pattern : Pattern, handler : 'handler, ?configure : RouteHandlerBuilder -> _) = this.Delete (builder.Build (), pattern, handler, ?configure=configure)

        /// <summary>
        /// Adds an HTTP <c>DELETE</c> endpoint.
        /// </summary>
        [<CustomOperation("delete")>]
        member this.Delete (builder : WebApplicationBuilder, pattern : Pattern, produces : (int * Type) list, handler : 'handler, ?configure : RouteHandlerBuilder -> _) = this.Delete (builder.Build (), pattern, produces, handler, ?configure=configure)

#if NET7_0_OR_GREATER
/// <summary>
/// Contains operations for adding HTTP <c>PATCH</c> endpoints.
/// </summary>
/// <example>
/// <code lang="fsharp">
/// let app =
///     webApp {
///         patch "/api/clowns/{id}" (fun (db : IDataAccess) id clown -> Results.Ok (db.PatchUpdate (id, clown)))
///     }
/// </code>
/// </example>
[<AutoOpen>]
module Patches =
    let private mapPatch route handler (router: IEndpointRouteBuilder) = router.MapPatch(route, handler = handler)

    type WebAppBuilder with
        /// <summary>
        /// Adds an HTTP <c>PATCH</c> endpoint.
        /// </summary>
        [<CustomOperation("patch")>]
        member this.Patch (app : WebApplication, pattern : Pattern, handler : 'handler, ?configure : RouteHandlerBuilder -> _) =
            this.CreateRoute mapPatch [] pattern handler configure app

        /// <summary>
        /// Adds an HTTP <c>PATCH</c> endpoint.
        /// </summary>
        [<CustomOperation("patch")>]
        member this.Patch (app : WebApplication, pattern : Pattern, produces : (int * Type) list, handler : 'handler, ?configure : RouteHandlerBuilder -> _) =
            this.CreateRoute mapPatch produces pattern handler configure app

        /// <summary>
        /// Adds an HTTP <c>PATCH</c> endpoint.
        /// </summary>
        [<CustomOperation("patch")>]
        member this.Patch (builder : WebApplicationBuilder, pattern : Pattern, handler : 'handler, ?configure : RouteHandlerBuilder -> _) = this.Patch (builder.Build (), pattern, handler, ?configure=configure)

        /// <summary>
        /// Adds an HTTP <c>PATCH</c> endpoint.
        /// </summary>
        [<CustomOperation("patch")>]
        member this.Patch (builder : WebApplicationBuilder, pattern : Pattern, produces : (int * Type) list, handler : 'handler, ?configure : RouteHandlerBuilder -> _) = this.Patch (builder.Build (), pattern, produces, handler, ?configure=configure)
#endif

/// <summary>
/// Contains priority 1 overloads for the <see cref="T:FSharp.AspNetCore.Builder.WebAppBuilder"/>
/// computation expression.
/// </summary>
[<AutoOpen>]
module Priority1 =
    type WebAppBuilder with
        /// <summary>
        /// Applies the given action to the <see cref="P:Microsoft.AspNetCore.Builder.WebApplicationBuilder.Services"/>
        /// property of the <see cref="T:Microsoft.AspNetCore.Builder.WebApplicationBuilder"/> being used to build the app.
        /// </summary>
        /// <param name="builder">The web application builder.</param>
        /// <param name="configureServices">The function to apply to the web application builder's service collection.</param>
        /// <example>
        /// <code>
        /// let app =
        ///     webApp {
        ///         services (fun services ->
        ///             services.AddEndpointsApiExplorer ()
        ///             services.AddSwaggerGen ()
        ///             services.AddControllers ())
        ///     }
        /// </code>
        /// </example>
        [<CustomOperation("services")>]
        member _.Services (builder : WebApplicationBuilder, configureServices) =
            ignore (configureServices builder.Services)
            builder

/// <summary>
/// Contains the <see cref="T:FSharp.AspNetCore.Builder.WebAppBuilder"/> computation expression builder.
/// </summary>
[<AutoOpen>]
module WebAppBuilder =
    /// <summary>
    /// Creates a web application using computation expression syntax.
    /// </summary>
    /// <example>
    /// <code lang="fsharp">
    /// open FSharp.AspNetCore.Builder.CommandLine
    ///
    /// let app =
    ///     webApp {
    ///         get "/hello" (fun () -> "🌎")
    ///     }
    /// </code>
    /// </example>
    let webApp = WebAppBuilder [||]

    /// <summary>
    /// Contains an alternative <c>webApp</c> builder that accepts command line arguments.
    /// </summary>
    module CommandLine =
        /// <summary>
        /// Creates a web application using computation expression syntax
        /// and the given command line arguments.
        /// </summary>
        /// <param name="args">The command-line arguments to give to the underlying <see cref="M:Microsoft.AspNetCore.Builder.WebApplication.CreateBuilder(System.String[])"/> call.</param>
        /// <example>
        /// <code lang="fsharp">
        /// open System
        /// open FSharp.AspNetCore.Builder.CommandLine
        ///
        /// let app =
        ///     webApp (Environment.GetCommandLineArgs ()) {
        ///         get "/hello" (fun () -> "🌎")
        ///     }
        /// </code>
        /// </example>
        let webApp args = WebAppBuilder args
