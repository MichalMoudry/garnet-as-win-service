using System.Globalization;
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
    public async Task<GarnetServerOptions> GetServerOptions(ISecretVault secretVault)
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
        var port = cfg["Port"];

        return new GarnetServerOptions
        {
            Address = !string.IsNullOrEmpty(address) ? address : "127.0.0.1",
            Port = Convert.ToInt32(
                !string.IsNullOrEmpty(port) ? port : "6379",
                CultureInfo.InvariantCulture
            ),
            AuthSettings = new PasswordAuthenticationSettings(password)
        };
    }
}