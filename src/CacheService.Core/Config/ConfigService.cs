using System.Diagnostics;
using System.Net;
using CacheService.Core.Env;
using Garnet.server;
using Garnet.server.Auth.Settings;
using Microsoft.Extensions.Configuration;

namespace CacheService.Core.Config;

/// <inheritdoc/>
public sealed class ConfigService(
    IConfiguration cfg,
    IEnvironmentService envService) : IConfigService
{
    /// <inheritdoc/>
    public async Task<GarnetServerOptions> GetServerOptions(
        ISecretVault secretVault)
    {
        Debug.Assert(secretVault != null);
        var password = secretVault.IsEnabled switch
        {
            true => await secretVault
                .GetSecretAsync("cache_password")
                .ConfigureAwait(true),
            false => !envService.IsProduction
                ? cfg["Password"]
                : throw new InvalidOperationException(
                    "Config password shouldn't be used in production"
                )
        };

        var isCfgPortValid = int.TryParse(cfg["Port"], out var port);
        return new GarnetServerOptions
        {
            EndPoints = [
                new IPEndPoint(
                    ReadIpAddrConfig(cfg["HostAddress"]),
                    isCfgPortValid ? port : 6379
                )
            ],
            AuthSettings = new PasswordAuthenticationSettings(password),
            QuietMode = envService.IsProduction
        };
    }

    /// <inheritdoc/>
    public GarnetServerOptions GetServerOptions()
    {
        var isCfgPortValid = int.TryParse(cfg["Port"], out var port);
        var password = cfg["Password"];

        return new GarnetServerOptions
        {
            EndPoints = [
                new IPEndPoint(
                    ReadIpAddrConfig(cfg["HostAddress"]),
                    isCfgPortValid ? port : 6379
                )
            ],
            QuietMode = envService.IsProduction,
            AuthSettings = !string.IsNullOrEmpty(password)
                ? new PasswordAuthenticationSettings(password)
                : new NoAuthSettings()
        };
    }

    /// <summary>
    /// Tries to read an IP address from a provided string. If the string isn't
    /// a valid IP address, then a loopback address is returned.
    /// </summary>
    /// <param name="address">String to parse as the IP address.</param>
    /// <returns>An instance of <see cref="IPAddress"/>.</returns>
    private static IPAddress ReadIpAddrConfig(string? address)
    {
        var isValid = IPAddress.TryParse(address, out var ipAddress);
        return isValid && ipAddress != null ? ipAddress : IPAddress.Loopback;
    }
}
