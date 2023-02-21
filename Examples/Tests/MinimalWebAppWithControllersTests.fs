module Examples.MinimalWebAppWithControllers.Tests

#nowarn "20"

open System.Net.Http.Json
open Expecto
open FsCheck
open Microsoft.AspNetCore.Mvc
open Microsoft.AspNetCore.TestHost
open Microsoft.Extensions.DependencyInjection
open Swensen.Unquote
open Examples.Tests
open type System.Net.HttpStatusCode
open type Expecto.FsCheckConfig

[<Tests>]
let tests =
    testList "Minimal web app with controllers integration tests" [
        testPropertyWithConfig { defaultConfig with arbitrary = [typeof<Arbs>] } "E2E" <| fun (Clown clown) (ShoeSize value) ->
            (task {
                use app =
                    Program.app (fun b ->
                        // `AddControllers` scans the executing assembly for controllers,
                        // but the executing assembly in this case is the test one, so we need
                        // to tell it to look at the original, too.
                        b.Services.AddControllers().AddApplicationPart typeof<Controllers.ClownsController>.Assembly

                        // Override the real DB implementation.
                        b.Services.AddSingleton
                            (
                                let db = System.Collections.Generic.Dictionary ()
                                { new Db.IDataAccess with
                                      member _.Create clown = async { db.Add (clown.Id, clown); return Ok clown }
                                      member _.Delete id = async { db.Remove id; return Ok () }
                                      member _.Get id = async { return match db.TryGetValue id with true, value -> Ok value | _ -> Error Db.Get.One.NotFound }
                                      member _.GetAll () = async { return Ok db.Values }
                                      member _.Update clown = async { db[clown.Id] <- clown; return Ok clown } }
                            )

                        // Enable the test server.
                        b.WebHost.UseTestServer ()
                    )

                do! app.StartAsync ()

                use client = app.GetTestClient ()

                let expectAtRoute (route : string) statusCode expected =
                    task {
                        use! resp = client.GetAsync route
                        resp.StatusCode =! statusCode
                        if enum 200 <= statusCode && statusCode < enum 300 then
                            let! content = resp.Content.ReadFromJsonAsync ()
                            content =! expected
                    }

                do! expectAtRoute "api/clowns" OK []
                use! created = client.PostAsJsonAsync ("api/clowns", clown)
                created.StatusCode =! Created
                let! createdClown = created.Content.ReadFromJsonAsync<ClownShape> ()
                do! expectAtRoute $"{created.Headers.Location}" OK createdClown
                do! expectAtRoute "api/clowns" OK [createdClown]
                use! updated = client.PutAsJsonAsync (created.Headers.Location, {| createdClown with ShoeSize = value |})
                updated.StatusCode =! OK
                let! updatedClown = updated.Content.ReadFromJsonAsync<ClownShape> ()
                do! expectAtRoute $"{created.Headers.Location}" OK updatedClown
                do! expectAtRoute "api/clowns" OK [updatedClown]
                use! deleted = client.DeleteAsync $"{created.Headers.Location}"
                deleted.StatusCode =! NoContent
                do! expectAtRoute $"{created.Headers.Location}" NotFound null
                do! expectAtRoute "api/clowns" OK []
            }).GetAwaiter().GetResult()
    ]
