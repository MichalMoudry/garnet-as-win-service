using System.ComponentModel;
using CacheService.Configuration.Env;
using Microsoft.Extensions.Configuration;
using NSubstitute;

namespace CacheService.UnitTests;

/// <summary>
/// A fixture with tests validating cache's environment service.
/// </summary>
[TestFixture]
public sealed class EnvServiceTests
{
    /// <summary>
    /// A simple test validating if the environment is correctly initialized
    /// through <see cref="IConfiguration"/> object.
    /// </summary>
    [TestCase("dev", AppEnvironment.Dev)]
    [TestCase("development", AppEnvironment.Dev)]
    [TestCase("stg", AppEnvironment.Stg)]
    [TestCase("staging", AppEnvironment.Stg)]
    [TestCase("prod", AppEnvironment.Prod)]
    [TestCase("production", AppEnvironment.Prod)]
    public void TestServiceCfgInitialization(
        string envSymbol,
        AppEnvironment targetEnv)
    {
        var cfg = Substitute.For<IConfiguration>();
        cfg["Env"].Returns(envSymbol);
        var envService = new EnvironmentService(cfg);

        Assert.Multiple(() =>
        {
            Assert.That(IsCorrectEnv(envService), Is.True);
            Assert.That(envService.CurrentEnvironment, Is.EqualTo(targetEnv));
        });
    }

    /// <summary>
    /// A test covering initialization with an unexpected environment symbol.
    /// </summary>
    [TestCase("develop")]
    [TestCase("product")]
    [TestCase("")]
    public void TestIncorrectCfgInitialization(string? envSymbol)
    {
        var cfg = Substitute.For<IConfiguration>();
        cfg["Env"].Returns(envSymbol);
        Assert.Throws<InvalidOperationException>(
            () => _ = new EnvironmentService(cfg)
        );
    }

    /// <summary>
    /// Method for calling a correct environment check method.
    /// </summary>
    private static bool IsCorrectEnv(EnvironmentService service) =>
        service.CurrentEnvironment switch
        {
            AppEnvironment.Dev => service.IsEnvDevelopment(),
            AppEnvironment.Stg => service.IsEnvStaging(),
            AppEnvironment.Prod => service.IsEnvProduction(),
            _ => throw new InvalidEnumArgumentException()
        };
}