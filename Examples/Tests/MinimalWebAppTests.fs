module Examples.MinimalWebApp.Tests

open System.Net.Http.Json
open Expecto
open Microsoft.AspNetCore.TestHost
open Swensen.Unquote
open Examples.Tests
open type System.Net.HttpStatusCode
open type Expecto.FsCheckConfig

[<Tests>]
let tests =
    testList "Minimal web app integration tests" [
        testPropertyWithConfig { defaultConfig with arbitrary = [typeof<Arbs>] } "E2E" <| fun (Clown clown) (ShoeSize value) ->
            (task {
                use app = Program.app (fun b -> b.WebHost.UseTestServer ())
                do! app.StartAsync ()
                use client = app.GetTestClient ()

                let expectAtRoute (route : string) statusCode expected =
                    task {
                        use! resp = client.GetAsync route
                        resp.StatusCode =! statusCode
                        if OK <= statusCode && statusCode < enum 300 then
                            let! content = resp.Content.ReadFromJsonAsync ()
                            content =! expected
                    }

                do! expectAtRoute "clowns" OK []
                use! created = client.PostAsJsonAsync ("clowns", clown)
                created.StatusCode =! Created
                let! createdClown = created.Content.ReadFromJsonAsync<ClownShape> ()
                do! expectAtRoute $"{created.Headers.Location}" OK createdClown
                do! expectAtRoute "clowns" OK [createdClown]
                use! updated = client.PutAsJsonAsync (created.Headers.Location, {| createdClown with ShoeSize = value |})
                updated.StatusCode =! OK
                let! updatedClown = updated.Content.ReadFromJsonAsync<ClownShape> ()
                do! expectAtRoute $"{created.Headers.Location}" OK updatedClown
                do! expectAtRoute "clowns" OK [updatedClown]
                use! deleted = client.DeleteAsync $"{created.Headers.Location}"
                deleted.StatusCode =! NoContent
                do! expectAtRoute $"{created.Headers.Location}" NotFound null
                do! expectAtRoute "clowns" OK []
            }).GetAwaiter().GetResult()
    ]
