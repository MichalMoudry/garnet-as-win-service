using CacheService.Configuration.Env;
using Garnet.server;

namespace CacheService.Configuration;

/// <summary>
/// Service for handling cache's custom configuration.
/// </summary>
internal interface IConfigService
{
    /// <summary>
    /// Method for obtaining environment specific configuration for
    /// the Garnet server.
    /// </summary>
    Task<GarnetServerOptions> GetServerOptions(
        ISecretVault secretVault,
        AppEnvironment currentEnv
    );
}