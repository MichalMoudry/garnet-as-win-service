using System.Globalization;
using CacheService.Configuration.Env;
using Garnet.server;
using Garnet.server.Auth;

namespace CacheService.Configuration;

/// <summary>
/// Service for handling cache's custom configuration.
/// </summary>
internal sealed class ConfigService : IConfigService
{
    private readonly IEnvironmentService _envService;

    private readonly IConfiguration _cfg;

    public ConfigService(IEnvironmentService envService, IConfiguration cfg)
    {
        _envService = envService;
        _cfg = cfg;
    }

    /// <inheritdoc/>
    public async Task<GarnetServerOptions> GetServerOptions(ISecretVault secretVault)
    {
        var hostAddress = _cfg["HostAddress"];
        var password = secretVault.IsEnabled switch
        {
            true => await secretVault.GetSecretAsync("cache_password"),
            false => _cfg["Password"]
        };

        return new GarnetServerOptions
        {
            Address = hostAddress,
            Port = Convert.ToInt32(
                _cfg["port"] ?? "6378",
                CultureInfo.InvariantCulture
            )/*,
            AuthSettings = new PasswordAuthenticationSettings(password)*/
        };
    }
}