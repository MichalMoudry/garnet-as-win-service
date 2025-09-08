using System.Net;
using CacheService.Core.Config.Env;
using Garnet.server;
using Garnet.server.Auth.Settings;
using Microsoft.Extensions.Configuration;

namespace CacheService.Core.Config;

/// <summary>
/// Service for handling cache's custom configuration.
/// </summary>
public sealed class ConfigService(
    IConfiguration cfg,
    IEnvironmentService envService) : IConfigService
{
    /// <inheritdoc/>
    public async Task<GarnetServerOptions> GetServerOptions(
        ISecretVault secretVault)
    {
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

        var isIpAddressValid = IPAddress.TryParse(
            cfg["HostAddress"],
            out var ipAddress
        );
        var isCfgPortValid = int.TryParse(cfg["Port"], out var port);
        return new GarnetServerOptions
        {
            EndPoints = [
                new IPEndPoint(
                    isIpAddressValid ? ipAddress! : IPAddress.Loopback,
                    isCfgPortValid ? port : 6379
                )
            ],
            AuthSettings = new PasswordAuthenticationSettings(password),
            QuietMode = envService.IsProduction
        };
    }
}