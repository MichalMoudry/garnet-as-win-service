using CacheService.Core.Config;
using Garnet.server;

namespace CacheService.Core;

/// <summary>
/// Service for handling cache's custom configuration.
/// </summary>
public interface IConfigService
{
    /// <summary>
    /// Method for obtaining environment specific configuration for
    /// the Garnet server.
    /// </summary>
    Task<GarnetServerOptions> GetServerOptions(ISecretVault secretVault);

    /// <summary>
    /// Method for obtaining environment specific configuration for
    /// the Garnet server. This retrieval is done without a storage for
    /// secrets.
    /// </summary>
    GarnetServerOptions GetServerOptions();
}
