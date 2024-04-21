using Garnet.server;

namespace CacheService.Configuration;

/// <summary>
/// A helper class for working with cache's configuration.
/// </summary>
internal static class Config
{
    /// <summary>
    /// Method for obtaining environment specific configuration for
    /// the Garnet server.
    /// </summary>
    public static GarnetServerOptions GetServerOptions(IConfiguration cfg)
    {
        var hostAddress = cfg["HostAddress"];
        return new GarnetServerOptions
        {
            Address = hostAddress
        };
    }
}