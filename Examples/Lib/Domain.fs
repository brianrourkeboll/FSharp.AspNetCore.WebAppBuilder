module Domain

open System

type SqlConnectionString = SqlConnectionString of value : string
type SqlCommandTimeout = SqlCommandTimeout of value : int

type Clown =
    { Id       : Id
      Name     : string
      Kind     : ClownKind
      ShoeSize : int }

and Id = Guid
and [<RequireQualifiedAccess>] ClownKind = ``🤡`` | ``😶`` | ``🧙‍``

type Guid with
    static member NewId () : Id = Guid.NewGuid ()
