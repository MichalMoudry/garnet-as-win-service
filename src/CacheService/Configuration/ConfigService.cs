using System.Globalization;
using CacheService.Configuration.Env;
using Garnet.server;
using Garnet.server.Auth;

namespace CacheService.Configuration;

/// <summary>
/// Service for handling cache's custom configuration.
/// </summary>
internal sealed class ConfigService(IConfiguration cfg, IEnvironmentService envService) : IConfigService
{
    /// <inheritdoc/>
    public async Task<GarnetServerOptions> GetServerOptions(ISecretVault secretVault)
    {
        var password = secretVault.IsEnabled switch
        {
            true => await secretVault.GetSecretAsync("cache_password"),
            false => !envService.IsEnvProduction()
                ? cfg["Password"]
                : throw new InvalidOperationException(
                    "Config password shouldn't be used in production"
                )
        };

        return new GarnetServerOptions
        {
            Address = cfg["HostAddress"] ?? "127.0.0.1",
            Port = Convert.ToInt32(
                cfg["Port"] ?? "6378",
                CultureInfo.InvariantCulture
            ),
            AuthSettings = new PasswordAuthenticationSettings(password)
        };
    }
}