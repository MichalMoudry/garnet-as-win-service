/// A module containing tests related to cache's configuration service.
[<Sealed>]
module CacheService.UnitTests.ServerConfigTests

open System
open System.Threading.Tasks
open CacheService.Configuration
open CacheService.Configuration.Env
open Garnet.server
open Microsoft.Extensions.Configuration
open NSubstitute
open NUnit.Framework

/// Method for asserting cache's server settings.
let private AssertServerSettings
    (actual: GarnetServerOptions)
    (expected: TestTypes.ExpectedServerSettings) =
    Assert.That(actual.Address, Is.EqualTo(expected.HostAddress))
    Assert.That(actual.Port, Is.EqualTo(expected.Port))

/// Method for obtaining a configured prod secret vault.
let private GetProdSecretVault (cachePassword: string, isVaultEnabled: bool) =
    let secretVault = Substitute.For<ISecretVault>()
    secretVault.IsEnabled.Returns(isVaultEnabled) |> ignore
    secretVault
        .GetSecretAsync("cache_password")
        .Returns(Task.FromResult(cachePassword))
        |> ignore
    secretVault

/// A test scenario covering config initialization
/// in a dev environment with default settings.
[<Test>]
let TestCorrectConfigServiceInitInDev () =
    task {
        let secretVault = Substitute.For<ISecretVault>()
        let envService = Substitute.For<IEnvironmentService>()
        let cfg = Substitute.For<IConfiguration>()

        secretVault.IsEnabled.Returns(false) |> ignore
        cfg["Password"].Returns("temp_pass") |> ignore

        let cfgService = ConfigService(cfg, envService)
        let! serverSettings = cfgService.GetServerOptions(secretVault)
        AssertServerSettings serverSettings {
            HostAddress = "127.0.0.1"; Port = 6379
        }
    }

/// A test case covering config initialization
/// in a dev environment with overriden settings.
[<TestCase("0.0.0.0", 3278)>]
let TestConfigOverrideInDev (address: string, port: int) =
    task {
        let secretVault = Substitute.For<ISecretVault>()
        let envService = Substitute.For<IEnvironmentService>()
        let cfg = Substitute.For<IConfiguration>()

        secretVault.IsEnabled.Returns(false) |> ignore
        cfg["HostAddress"].Returns(address) |> ignore
        cfg["Port"].Returns(port.ToString()) |> ignore
        cfg["Password"].Returns("temp_pass") |> ignore

        let cfgService = ConfigService(cfg, envService)
        let! serverSettings = cfgService.GetServerOptions(secretVault)
        AssertServerSettings serverSettings {
            HostAddress = address; Port = port 
        }
    }

/// A test scenario covering config initialization
/// in a production environment with default settings.
[<Test>]
let TestCorrectConfigServiceInitInProd () =
    task {
        let secretVault = GetProdSecretVault("temp_pass", true)
        let envService = Substitute.For<IEnvironmentService>()
        envService.IsEnvProduction().Returns(true) |> ignore
        let cfg = Substitute.For<IConfiguration>()
        cfg["HostAddress"].Returns("0.0.0.0") |> ignore

        let cfgService = ConfigService(cfg, envService)
        let! serverSettings = cfgService.GetServerOptions(secretVault)
        AssertServerSettings serverSettings {
            HostAddress = "0.0.0.0"; Port = 6379
        }
    }

/// A test scenario covering config initialization
/// in a production environment with default settings
/// but without initialized secret vault.
[<Test>]
let TestConfigServiceInitInProdWithoutSecretVault () =
    let secretVault = GetProdSecretVault("temp_pass", false)
    let envService = Substitute.For<IEnvironmentService>()
    envService.IsEnvProduction().Returns(true) |> ignore
    let cfg = Substitute.For<IConfiguration>()

    let cfgService = ConfigService(cfg, envService)
    let getServerSettings = fun () ->
        cfgService.GetServerOptions(secretVault)
        |> Async.AwaitTask
        |> Async.RunSynchronously
        |> ignore
    Assert.That(getServerSettings, Throws.TypeOf<AggregateException>())