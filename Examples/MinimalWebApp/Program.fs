module Examples.MinimalWebApp.Program

#nowarn "20" // Don't warn about unignored values.

open FSharp.AspNetCore.Builder
open FsToolkit.ErrorHandling
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Http
open Microsoft.AspNetCore.Mvc
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.Logging
open ControllerHelpers
open Db
open Domain
open Dtos
open type StatusCodes

type private Program = class end

let app configureBuilder =
    webApp {
        connectionString "SqlDb" SqlConnectionString
        configurationValue "AppSettings:SqlCommandTimeout" SqlCommandTimeout
        singleton Id.NewId
        singleton typeof<IDataAccess> typeof<DataAccess>
        logging (fun logging -> logging.AddConsole ())

        services (fun services ->
            services.AddEndpointsApiExplorer ()
            services.AddSwaggerGen ())

        buildWith configureBuilder

        webApp (fun app ->
            app.UseSwagger ()
            app.UseSwaggerUI ())

        get "/api/clowns" [
            Status200OK,                  typeof<seq<Dtos.Get.Clown>>
            Status500InternalServerError, typeof<ProblemDetails>
        ] (fun (logger : ILogger<Program>) (db : IDataAccess) ->
            db.GetAll ()
            |> AsyncResult.teeError (function
                | Get.Many.DbExn e -> logger.LogError ("A database exception occurred when trying to get the clowns.", e)
                | Get.Many.DbTimeout e -> logger.LogWarning ("A database timeout occurred when trying to get the clowns.", e))
            |> AsyncResult.foldResult
                (Seq.map Get.Clown.toDto >> Results.Ok)
                (function
                 | Get.Many.DbExn _ -> Results.Problem "A database exception occurred."
                 | Get.Many.DbTimeout _ -> Results.Problem "A database timeout occurred.")
            |> Async.StartAsTask
        ) (fun routeHandler ->
            routeHandler.WithOpenApi (fun op -> op.Description <- "This endpoint gets clowns."; op)
            routeHandler.AllowAnonymous ()
        )

        get "/api/clowns/{id}" [
            Status200OK,                  typeof<Dtos.Get.Clown>
            Status404NotFound,            null
            Status500InternalServerError, typeof<ProblemDetails>
        ] (fun (logger : ILogger<Program>) (db : IDataAccess) id ->
            db.Get id
            |> AsyncResult.teeError (function
                | Get.One.NotFound -> ()
                | Get.One.DbExn e -> logger.LogError ("A database exception occurred when trying to get the clown.", e)
                | Get.One.DbTimeout e -> logger.LogWarning ("A database timeout occurred when trying to get the clown.", e))
            |> AsyncResult.foldResult
                (Get.Clown.toDto >> Results.Ok)
                (function
                 | Get.One.NotFound -> Results.NotFound ()
                 | Get.One.DbExn _ -> Results.Problem "A database exception occurred."
                 | Get.One.DbTimeout _ -> Results.Problem "A database timeout occurred.")
            |> Async.StartAsTask)

        post "/api/clowns" [
            Status201Created,             typeof<Dtos.Get.Clown>
            Status400BadRequest,          typeof<ValidationProblemDetails>
            Status409Conflict,            typeof<string>
            Status500InternalServerError, typeof<ProblemDetails>
        ] (fun (logger : ILogger<Program>) mkId (db : IDataAccess) clown ->
            Create.Clown.toDomain (mkId ()) clown
            |> AsyncResult.ofResult
            |> AsyncResult.mapError ValidationErrors.ofList
            |> AsyncResult.bind (db.Create >> AsyncResult.mapError IOError)
            |> AsyncResult.teeError (function
                | IOError (Create.DbExn e) -> logger.LogError ("A database exception occurred when trying to get the clown.", e)
                | IOError (Create.DbTimeout e) -> logger.LogWarning ("A database timeout occurred when trying to get the clown.", e)
                | IOError (Create.NameConflict _) | ValidationErrors _ -> ())
            |> AsyncResult.foldResult
                (Get.Clown.toDto >> fun clown -> Results.Created ($"/clowns/{clown.Id}", clown))
                (function
                 | ValidationErrors es -> Results.ValidationProblem es
                 | IOError (Create.NameConflict existingId) -> Results.Conflict ($"There is an existing clown with ID {existingId} with the same name.")
                 | IOError (Create.DbExn _) -> Results.Problem "A database exception occurred."
                 | IOError (Create.DbTimeout _) -> Results.Problem "A database timeout occurred.")
            |> Async.StartAsTask)

        put "/api/clowns/{id}" [
            Status200OK,                  typeof<Dtos.Get.Clown>
            Status400BadRequest,          typeof<ValidationProblemDetails>
            Status404NotFound,            null
            Status409Conflict,            typeof<string>
            Status500InternalServerError, typeof<ProblemDetails>
        ] (fun (logger : ILogger<Program>) (db : IDataAccess) id clown ->
            Update.Clown.toDomain id clown
            |> AsyncResult.ofResult
            |> AsyncResult.mapError ValidationErrors.ofList
            |> AsyncResult.bind (db.Update >> AsyncResult.mapError IOError)
            |> AsyncResult.teeError (function
                | IOError (Update.DbExn e) -> logger.LogError ("A database exception occurred when trying to get the clown.", e)
                | IOError (Update.DbTimeout e) -> logger.LogWarning ("A database timeout occurred when trying to get the clown.", e)
                | IOError (Update.NotFound _) | IOError (Update.NameConflict _) | ValidationErrors _ -> ())
            |> AsyncResult.foldResult
                (Get.Clown.toDto >> Results.Ok)
                (function
                 | ValidationErrors es -> Results.ValidationProblem es
                 | IOError Update.NotFound -> Results.NotFound ()
                 | IOError (Update.NameConflict existingId) -> Results.Conflict ($"There is an existing clown with ID {existingId} with the same name.")
                 | IOError (Update.DbExn _) -> Results.Problem "A database exception occurred."
                 | IOError (Update.DbTimeout _) -> Results.Problem "A database timeout occurred.")
            |> Async.StartAsTask)

        delete "/api/clowns/{id}" [
            Status204NoContent,           null
            Status500InternalServerError, typeof<ProblemDetails>
        ] (fun (logger : ILogger<Program>) (db : IDataAccess) id ->
            db.Delete id
            |> AsyncResult.teeError (function
                | Delete.DbExn e -> logger.LogError ("A database exception occurred when trying to get the clowns.", e)
                | Delete.DbTimeout e -> logger.LogWarning ("A database timeout occurred when trying to get the clowns.", e))
            |> AsyncResult.foldResult
                Results.NoContent
                (function
                 | Delete.DbExn _ -> Results.Problem "A database exception occurred."
                 | Delete.DbTimeout _ -> Results.Problem "A database timeout occurred.")
            |> Async.StartAsTask)
    }

(app ignore).Run ()
