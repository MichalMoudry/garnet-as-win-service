#r "nuget: Azure.Identity"
#r "nuget: Azure.Security.KeyVault.Secrets"

open System

let client = Azure.Security.KeyVault.Secrets.SecretClient(
    Uri("https://localhost:8443"),
    Azure.Identity.DefaultAzureCredential()
)

let response = client.SetSecret("cache_password", "localhost")
printfn $"{response}"