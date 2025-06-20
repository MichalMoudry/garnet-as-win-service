using System.Net;
using CacheService.Configuration.Env;
using Garnet.server;
using Garnet.server.Auth.Settings;

namespace CacheService.Configuration;

/// <summary>
/// Service for handling cache's custom configuration.
/// </summary>
internal sealed class ConfigService(
    IConfiguration cfg,
    IEnvironmentService envService) : IConfigService
{
    /// <inheritdoc/>
    public async Task<GarnetServerOptions> GetServerOptions(
        ISecretVault secretVault)
    {
        var password = secretVault.IsEnabled switch
        {
            true => await secretVault.GetSecretAsync("cache_password"),
            false => !envService.IsProduction
                ? cfg["Password"]
                : throw new InvalidOperationException(
                    "Config password shouldn't be used in production"
                )
        };

        var address = cfg["HostAddress"];
        var cfgPort = cfg["Port"];
        var isCfgPortValid = int.TryParse(cfgPort, out var port);
        var isIpAddressValid = IPAddress.TryParse(address, out var ipAddress);
        if (!isIpAddressValid || ipAddress == null)
        {
            throw new InvalidOperationException(
                $"'{address}' is not a valid IP address"
            );
        }

        return new GarnetServerOptions
        {
            EndPoints = [
                new IPEndPoint(ipAddress, isCfgPortValid ? port : 6379)
            ],
            AuthSettings = new PasswordAuthenticationSettings(password),
            QuietMode = envService.IsProduction
        };
    }
}