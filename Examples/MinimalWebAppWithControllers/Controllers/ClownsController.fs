namespace Examples.MinimalWebAppWithControllers.Controllers

open System.Net.Mime
open FsToolkit.ErrorHandling
open Microsoft.AspNetCore.Http
open Microsoft.AspNetCore.Mvc
open Microsoft.Extensions.Logging
open ControllerHelpers
open Db
open Domain
open Dtos
open type StatusCodes

[<Sealed>]
[<ApiController>]
[<Route("api/clowns")>]
[<Produces(MediaTypeNames.Application.Json)>]
type ClownsController
    (
        dataAccess : IDataAccess,
        logger     : ILogger<ClownsController>,
        mkId       : unit -> Id
    ) =
    inherit ControllerBase ()

    [<HttpGet>]
    [<ProducesResponseType(typeof<seq<Dtos.Get.Clown>>, Status200OK)>]
    [<ProducesResponseType(typeof<ProblemDetails>, Status500InternalServerError)>]
    member _.GetClowns () =
        dataAccess.GetAll ()
        |> AsyncResult.teeError (function
            | Get.Many.DbExn e -> logger.LogError ("A database exception occurred when trying to get the clowns.", e)
            | Get.Many.DbTimeout e -> logger.LogWarning ("A database timeout occurred when trying to get the clowns.", e))
        |> AsyncResult.foldResult
            (Seq.map Get.Clown.toDto >> Results.Ok)
            (function
             | Get.Many.DbExn _ -> Results.Problem "A database exception occurred."
             | Get.Many.DbTimeout _ -> Results.Problem "A database timeout occurred.")

    [<HttpGet("{id}")>]
    [<ProducesResponseType(typeof<Dtos.Get.Clown>, Status200OK)>]
    [<ProducesResponseType(Status404NotFound)>]
    [<ProducesResponseType(typeof<ProblemDetails>, Status500InternalServerError)>]
    member _.GetClown id =
        dataAccess.Get id
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

    [<HttpPost>]
    [<ProducesResponseType(typeof<Dtos.Get.Clown>, Status200OK)>]
    [<ProducesResponseType(typeof<ValidationProblemDetails>, Status400BadRequest)>]
    [<ProducesResponseType(typeof<string>, Status409Conflict)>]
    [<ProducesResponseType(typeof<ProblemDetails>, Status500InternalServerError)>]
    member _.CreateClown ([<FromBody>] clown) =
        Create.Clown.toDomain (mkId ()) clown
        |> AsyncResult.ofResult
        |> AsyncResult.mapError ValidationErrors.ofList
        |> AsyncResult.bind (dataAccess.Create >> AsyncResult.mapError IOError)
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

    [<HttpPut("{id}")>]
    [<ProducesResponseType(typeof<Dtos.Get.Clown>, Status200OK)>]
    [<ProducesResponseType(typeof<ValidationProblemDetails>, Status400BadRequest)>]
    [<ProducesResponseType(Status404NotFound)>]
    [<ProducesResponseType(typeof<string>, Status409Conflict)>]
    [<ProducesResponseType(typeof<ProblemDetails>, Status500InternalServerError)>]
    member _.UpdateClown ([<FromRoute>] id, [<FromBody>] clown) =
        Update.Clown.toDomain id clown
        |> AsyncResult.ofResult
        |> AsyncResult.mapError ValidationErrors.ofList
        |> AsyncResult.bind (dataAccess.Update >> AsyncResult.mapError IOError)
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

    [<HttpDelete("{id}")>]
    [<ProducesResponseType(Status204NoContent)>]
    [<ProducesResponseType(typeof<ProblemDetails>, Status500InternalServerError)>]
    member _.DeleteClown id =
        dataAccess.Delete id
        |> AsyncResult.teeError (function
            | Delete.DbExn e -> logger.LogError ("A database exception occurred when trying to get the clowns.", e)
            | Delete.DbTimeout e -> logger.LogWarning ("A database timeout occurred when trying to get the clowns.", e))
        |> AsyncResult.foldResult
            Results.NoContent
            (function
             | Delete.DbExn _ -> Results.Problem "A database exception occurred."
             | Delete.DbTimeout _ -> Results.Problem "A database timeout occurred.")
