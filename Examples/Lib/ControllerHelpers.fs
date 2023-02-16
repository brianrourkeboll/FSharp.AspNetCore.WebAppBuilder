module ControllerHelpers

type ActionError<'IOError> =
    | ValidationErrors of errors : Map<string, string[]>
    | IOError of error : 'IOError

module ValidationErrors =
    let ofList errors = ValidationErrors ((Map.empty, errors) ||> List.fold (fun m (k, v) -> m.Add (k, [|v|])))
