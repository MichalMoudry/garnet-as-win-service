#r "nuget: Azure.Identity"
#r "nuget: Azure.Security.KeyVault.Secrets"

open System

let client = Azure.Security.KeyVault.Secrets.SecretClient(
    Uri("https://localhost:8443"),
    Azure.Identity.ClientSecretCredential(
    )
)

let input = "localhost"
let response =
    client.SetSecretAsync("cache_password", input)
    |> Async.AwaitTask
    |> Async.RunSynchronously
printfn $"{response}"

let secret =
    client.GetSecretAsync("cache_password")
    |> Async.AwaitTask
    |> Async.RunSynchronously
printfn $"{secret}"