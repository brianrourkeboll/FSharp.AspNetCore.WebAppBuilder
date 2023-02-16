module Db

open System
open Domain

module Create =
    type DbError =
        | NameConflict of existingId : Guid
        | DbTimeout    of exn : exn
        | DbExn        of exn : exn

module Get =
    module One =
        type DbError =
            | NotFound
            | DbTimeout of exn : exn
            | DbExn     of exn : exn

    module Many =
        type DbError =
            | DbTimeout of exn : exn
            | DbExn     of exn : exn

module Update =
    type DbError =
        | NotFound
        | NameConflict of existingId : Guid
        | DbTimeout    of exn : exn
        | DbExn        of exn : exn

module Delete =
    type DbError =
        | DbTimeout of exn : exn
        | DbExn     of exn : exn

type IDataAccess =
    abstract Create : clown:Clown -> Async<Result<Clown, Create.DbError>>
    abstract Get    : id:Guid     -> Async<Result<Clown, Get.One.DbError>>
    abstract GetAll : unit        -> Async<Result<seq<Clown>, Get.Many.DbError>>
    abstract Update : clown:Clown -> Async<Result<Clown, Update.DbError>>
    abstract Delete : id:Guid     -> Async<Result<unit, Delete.DbError>>

[<AutoOpen>]
module internal Impl =
    type DummyDb = System.Collections.Concurrent.ConcurrentDictionary<Guid, Clown>

    open Create

    let create (dummyDb : DummyDb) (SqlConnectionString _connectionString) (SqlCommandTimeout _timeout) (clown : Clown) : Async<Result<Clown, DbError>> =
        async {
            // Pretend this is wrapped in a transaction, etc...
            return
                dummyDb.Values
                |> Seq.tryPick (fun { Id = id; Name = name } -> if name = clown.Name then Some id else None)
                |> Option.map (Error << NameConflict)
                |> Option.defaultWith (fun () ->
                    if dummyDb.TryAdd (clown.Id, clown) then Ok clown
                    else Error (DbExn (exn "Just for fun.")))
        }

    open Get.One

    let get (dummyDb : DummyDb) (SqlConnectionString _connectionString) (SqlCommandTimeout _timeout) (id : Guid) : Async<Result<Clown, DbError>> =
        async {
            match dummyDb.TryGetValue id with
            | true, clown -> return Ok clown
            | false, _ -> return Error NotFound
        }

    open Get.Many

    let getAll (dummyDb : DummyDb) (SqlConnectionString _connectionString) (SqlCommandTimeout _timeout) : Async<Result<seq<Clown>, DbError>> =
        async {
            return Ok dummyDb.Values
        }

    open Update

    let update (dummyDb : DummyDb) (SqlConnectionString _connectionString) (SqlCommandTimeout _timeout) (clown : Clown) : Async<Result<Clown, DbError>> =
        async {
            // Pretend this is wrapped in a transaction, etc...
            match dummyDb.TryGetValue clown.Id with
            | false, _ -> return Error NotFound
            | true, existingClown ->
                return
                    dummyDb.Values
                    |> Seq.tryPick (fun { Id = id; Name = name } -> if id <> clown.Id && name = clown.Name then Some id else None)
                    |> Option.map (Error << NameConflict)
                    |> Option.defaultWith (fun () ->
                        if dummyDb.TryUpdate (clown.Id, clown, existingClown) then Ok clown
                        else Error NotFound)
        }

    open Delete

    let delete (dummyDb : DummyDb) (SqlConnectionString _connectionString) (SqlCommandTimeout _timeout) (id : Guid) : Async<Result<unit, DbError>> =
        async {
            match dummyDb.TryRemove id with _ -> return Ok ()
        }

[<Sealed>]
type DataAccess (connectionString : SqlConnectionString, commandTimeout : SqlCommandTimeout) =
    let dummyDb = DummyDb ()
    interface IDataAccess with
        member _.Create clown = create dummyDb connectionString commandTimeout clown
        member _.Get id = get dummyDb connectionString commandTimeout id
        member _.GetAll () = getAll dummyDb connectionString commandTimeout
        member _.Update clown = update dummyDb connectionString commandTimeout clown
        member _.Delete id = delete dummyDb connectionString commandTimeout id
