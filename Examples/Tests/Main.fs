module Examples.Tests.Program

open Expecto

[<EntryPoint>]
let main argv = Tests.runTestsInAssembly defaultConfig argv
