module Dtos

open System
open FsToolkit.ErrorHandling

type ValidationError = Path * Message
and Path = string
and Message = string

module Name =
    let toDomain (name : string) =
        if String.IsNullOrWhiteSpace name then
            Error (nameof name, "Name cannot be blank.")
        else
            Ok name

module ClownKind =
    open Domain

    let toDomain (kind : string) =
        match System.Net.WebUtility.UrlDecode kind with
        | nameof ClownKind.``🤡`` -> Ok ClownKind.``🤡``
        | nameof ClownKind.``😶`` -> Ok ClownKind.``😶``
        | nameof ClownKind.``🧙‍`` -> Ok ClownKind.``🧙‍``
        | wrongKind -> Error (nameof kind, $"Kind must be one of {{'{nameof ClownKind.``🤡``}', '{nameof ClownKind.``😶``}', '{nameof ClownKind.``🧙‍``}'}}, but was '{wrongKind}'.")

    let toDto (kind : ClownKind) =
        match kind with
        | ClownKind.``🤡`` -> nameof ClownKind.``🤡``
        | ClownKind.``😶`` -> nameof ClownKind.``😶``
        | ClownKind.``🧙‍`` -> nameof ClownKind.``🧙‍``

module ShoeSize =
    let toDomain (shoeSize : int) =
        if shoeSize < 15 then
            Error (nameof shoeSize, "Shoe size must be at least 15.")
        else
            Ok shoeSize

module Create =
    [<CompiledName("NewClown")>]
    type Clown =
        { Name     : string
          Kind     : string
          ShoeSize : int }

    module Clown =
        let toDto (clown : Domain.Clown) =
            { Name = clown.Name
              Kind = ClownKind.toDto clown.Kind
              ShoeSize = clown.ShoeSize }

        let toDomain id (clown : Clown) : Result<Domain.Clown, ValidationError list> =
            validation {
                let! name = Name.toDomain clown.Name
                and! kind = ClownKind.toDomain clown.Kind
                and! shoeSize = ShoeSize.toDomain clown.ShoeSize
                return { Id = id; Name = name; Kind = kind; ShoeSize = shoeSize }
            }

module Get =
    type Clown =
        { Id       : Guid
          Name     : string
          Kind     : string
          ShoeSize : int }

    module Clown =
        let toDto (clown : Domain.Clown) =
            { Id = clown.Id
              Name = clown.Name
              Kind = ClownKind.toDto clown.Kind
              ShoeSize = clown.ShoeSize }

module Update =
    [<CompiledName("ModifiedClown")>]
    type Clown =
        { Name     : string
          Kind     : string
          ShoeSize : int }

    module Clown =
        let toDomain id (clown : Clown) : Result<Domain.Clown, ValidationError list> =
            validation {
                let! id = if id = Guid.Empty then Error (nameof id, "A clown ID must be provided.") else Ok id
                and! name = Name.toDomain clown.Name
                and! kind = ClownKind.toDomain clown.Kind
                and! shoeSize = ShoeSize.toDomain clown.ShoeSize
                return { Id = id; Name = name; Kind = kind; ShoeSize = shoeSize }
            }
