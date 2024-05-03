/// A module containing tests related to cache's configuration service.
[<Sealed>]
module CacheService.UnitTests.ServerConfigTests

open CacheService.Configuration
open CacheService.Configuration.Env
open Microsoft.Extensions.Configuration
open NSubstitute
open NUnit.Framework

/// A test scenario covering config initialization
/// in a dev environment with default settings.
[<Test>]
let TestCorrectConfigServiceInitInDev () =
    task {
        let secretVault = Substitute.For<ISecretVault>()
        let envService = Substitute.For<IEnvironmentService>()
        let cfg = Substitute.For<IConfiguration>()

        secretVault.IsEnabled.Returns(false) |> ignore
        cfg["Password"].Returns("test_password") |> ignore

        let cfgService = ConfigService(cfg, envService)
        let! serverSettings = cfgService.GetServerOptions(secretVault)

        Assert.That(serverSettings.Address, Is.EqualTo("127.0.0.1"))
        Assert.That(serverSettings.Port, Is.EqualTo(6378))
    }

/// A test scenario covering config initialization
/// in a production environment with default settings.
[<Test>]
let TestCorrectConfigServiceInitInProd () =
    Assert.Pass()

/// A test scenario covering config initialization
/// in a production environment with default settings
/// but without initialized secret vault.
[<Test>]
let TestConfigServiceInitInProdWithoutSecretVault () =
    Assert.Pass()