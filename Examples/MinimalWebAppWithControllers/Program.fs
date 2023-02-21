module Examples.MinimalWebAppWithControllers.Program

open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.Logging
open FSharp.AspNetCore.Builder
open Db
open Domain

#nowarn "20" // Don't warn about unignored values.

let app configureBuilder =
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

        buildWith configureBuilder

        webApp (fun app ->
            app.UseSwagger ()
            app.UseSwaggerUI ()
            app.MapControllers ())
    }

(app ignore).Run ()
