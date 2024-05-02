/// A module with tests validating cache's environment service.
[<Sealed>]
module CacheService.UnitTests.EnvServiceTests

open System
open CacheService.Configuration.Env
open Microsoft.Extensions.Configuration
open NSubstitute
open NUnit.Framework

/// Method for calling a correct environment check method.
let private IsCorrectEnv (service: EnvironmentService) =
    match service.CurrentEnvironment with
    | AppEnvironment.Dev -> service.IsEnvDevelopment()
    | AppEnvironment.Stg -> service.IsEnvStaging()
    | AppEnvironment.Prod -> service.IsEnvProduction()
    | _ -> ArgumentOutOfRangeException() |> raise

/// A simple test validating if the environment is correctly initialized
/// through IConfiguration object.
[<TestCase("dev", AppEnvironment.Dev)>]
[<TestCase("development", AppEnvironment.Dev)>]
[<TestCase("stg", AppEnvironment.Stg)>]
[<TestCase("staging", AppEnvironment.Stg)>]
[<TestCase("prod", AppEnvironment.Prod)>]
[<TestCase("production", AppEnvironment.Prod)>]
let TestServiceCfgInitialization (envSymbol: string, targetEnv: AppEnvironment) =
    let cfg = Substitute.For<IConfiguration>()
    cfg["Env"].Returns(envSymbol) |> ignore
    let envService = EnvironmentService(cfg)

    Assert.That(IsCorrectEnv(envService), Is.True)
    Assert.That(envService.CurrentEnvironment, Is.EqualTo(targetEnv))

/// A test covering initialization with an unexpected environment symbol.
[<TestCase("develop")>]
[<TestCase("product")>]
[<TestCase("")>]
let TestIncorrectCfgInitialization (envSymbol: string) =
    let cfg = Substitute.For<IConfiguration>()
    cfg["Env"].Returns(envSymbol) |> ignore

    let createService = fun () -> EnvironmentService(cfg) |> ignore
    Assert.That(createService, Throws.TypeOf<InvalidOperationException>())

/// A simple test validating if the environment is correctly initialized
/// through an environment variable.
[<TestCase("DOTNET_ENVIRONMENT", "dev", AppEnvironment.Dev)>]
[<TestCase("DOTNET_ENVIRONMENT", "stg", AppEnvironment.Stg)>]
[<TestCase("DOTNET_ENVIRONMENT", "prod", AppEnvironment.Prod)>]
let TestServiceEnvInitialization (
    envVarName: string,
    envSymbol: string,
    targetEnv: AppEnvironment) =
    let cfg = Substitute.For<IConfiguration>()
    Environment.SetEnvironmentVariable(
        envVarName,
        envSymbol,
        EnvironmentVariableTarget.Process
    )
    let envService = EnvironmentService(cfg)

    Assert.That(IsCorrectEnv(envService), Is.True)
    Assert.That(envService.CurrentEnvironment, Is.EqualTo(targetEnv))