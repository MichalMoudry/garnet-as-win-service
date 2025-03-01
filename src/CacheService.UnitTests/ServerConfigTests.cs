using System.Net;
using CacheService.Configuration;
using CacheService.Configuration.Env;
using CacheService.UnitTests.Model;
using Garnet.server;
using Microsoft.Extensions.Configuration;
using NSubstitute;

namespace CacheService.UnitTests;

/// <summary>
/// A fixture containing tests related to cache's configuration service.
/// </summary>
[TestFixture]
public sealed class ServerConfigTests
{
    /// <summary>
    /// A test scenario covering config initialization
    /// in a dev environment with default settings.
    /// </summary>
    [Test]
    public async Task TestCorrectConfigServiceInitInDev()
    {
        var secretVault = Substitute.For<ISecretVault>();
        var envService = Substitute.For<IEnvironmentService>();
        var cfg = Substitute.For<IConfiguration>();

        secretVault.IsEnabled.Returns(false);
        cfg["Password"].Returns("temp_pass");

        var cfgService = new ConfigService(cfg, envService);
        var serverSettings = await cfgService.GetServerOptions(secretVault);
        AssertServerSettings(
            serverSettings,
            new ExpectedServerSettings("127.0.0.1", 6379)
        );
    }

    /// <summary>
    /// A test case covering config initialization
    /// in a dev environment with overriden settings.
    /// </summary>
    [TestCase("0.0.0.0", 3278)]
    public async Task TestConfigOverrideInDev(string address, int port)
    {
        var secretVault = Substitute.For<ISecretVault>();
        var envService = Substitute.For<IEnvironmentService>();
        var cfg = Substitute.For<IConfiguration>();

        secretVault.IsEnabled.Returns(false);
        cfg["HostAddress"].Returns(address);
        cfg["Port"].Returns(port.ToString());
        cfg["Password"].Returns("temp_pass");

        var cfgService = new ConfigService(cfg, envService);
        var serverSettings = await cfgService.GetServerOptions(secretVault);
        AssertServerSettings(
            serverSettings,
            new ExpectedServerSettings(address, port)
        );
    }

    /// <summary>
    /// A test scenario covering config initialization
    /// in a production environment with default settings.
    /// </summary>
    [Test]
    public async Task TestCorrectConfigServiceInitInProd()
    {
        var secretVault = GetProdSecretVault("temp_pass", true);
        var envService = Substitute.For<IEnvironmentService>();
        envService.IsProduction.Returns(true);
        var cfg = Substitute.For<IConfiguration>();
        cfg["HostAddress"].Returns("0.0.0.0");

        var cfgService = new ConfigService(cfg, envService);
        var serverSettings = await cfgService.GetServerOptions(secretVault);
        if (!IPAddress.TryParse("0.0.0.0", out var address))
        {
            return;
        }
        AssertServerSettings(
            serverSettings,
            new ExpectedServerSettings(address, 6379)
        );
    }

    /// <summary>
    /// A test scenario covering config initialization in a production environment
    /// with default settings but without initialized secret vault.
    /// </summary>
    [Test]
    public void TestConfigServiceInitInProdWithoutSecretVault()
    {
        var secretVault = GetProdSecretVault("temp_pass", false);
        var envService = Substitute.For<IEnvironmentService>();
        envService.IsProduction.Returns(true);
        var cfg = Substitute.For<IConfiguration>();

        var cfgService = new ConfigService(cfg, envService);
        Assert.ThrowsAsync<InvalidOperationException>(
            async () => _ = await cfgService.GetServerOptions(secretVault)
        );
    }

    /// <summary>
    /// Method for asserting cache's actual server settings against expected.
    /// </summary>
    private static void AssertServerSettings(
        ServerOptions actual,
        ExpectedServerSettings expected)
    {
        if (actual.EndPoint is not IPEndPoint endPoint)
        {
            Assert.That(actual.EndPoint, Is.TypeOf<IPEndPoint>());
            return;
        }
        Assert.Multiple(() =>
        {
            Assert.That(endPoint.Address, Is.EqualTo(expected.HostAddress));
            Assert.That(endPoint.Port, Is.EqualTo(expected.Port));
        });
    }

    /// <summary>Method for obtaining a configured prod secret vault.</summary>
    private static ISecretVault GetProdSecretVault(string? cachePass, bool isEnabled)
    {
        var secretVault = Substitute.For<ISecretVault>();
        secretVault.IsEnabled.Returns(isEnabled);
        secretVault
            .GetSecretAsync("cache_password")
            .Returns(Task.FromResult(cachePass));
        return secretVault;
    }
}