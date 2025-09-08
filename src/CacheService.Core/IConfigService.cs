using CacheService.Core.Config;
using Garnet.server;

namespace CacheService.Core;

public interface IConfigService
{
    /// <summary>
    /// Method for obtaining environment specific configuration for
    /// the Garnet server.
    /// </summary>
    Task<GarnetServerOptions> GetServerOptions(ISecretVault secretVault);
}