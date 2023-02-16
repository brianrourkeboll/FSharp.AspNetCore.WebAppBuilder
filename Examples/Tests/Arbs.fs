namespace Examples.Tests

open System
open FsCheck

type Kind = Kind of string
type ShoeSize = ShoeSize of int
type Clown = Clown of ClownShape
and ClownShape = {| Name : string; Kind : string; ShoeSize : int |}

module Gens =
    let kind = Gen.elements ["🤡"; "😶"; "🧙‍"]

    let shoeSize = Gen.choose (15, Int32.MaxValue)

    let clown =
        gen {
            let! NonWhiteSpaceString name = Arb.generate
            let! kind = kind
            let! shoeSize = shoeSize
            return {| Name = name; Kind = kind; ShoeSize = shoeSize |}
        }

type Arbs =
    static member Kind () = Gens.kind |> Gen.map Kind |> Arb.fromGen
    static member ShoeSize () = Gens.shoeSize |> Gen.map ShoeSize |> Arb.fromGen
    static member Clown () = Gens.clown |> Gen.map Clown |> Arb.fromGen
