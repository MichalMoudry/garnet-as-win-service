using System.Net;
using CacheService.Core.Config;
using Garnet;
using Garnet.server;
using Microsoft.Extensions.Logging;

namespace CacheService.Core;

public partial class CacheServer(
    ILogger<CacheServer> logger,
    IConfigService config,
    ISecretVault vault)
{
    private GarnetServerOptions? _serverOptions;

    public async Task Initialize()
    {
        _serverOptions = await config
            .GetServerOptions(vault)
            .ConfigureAwait(true);

        foreach (var endPoint in _serverOptions.EndPoints)
        {
            switch (endPoint)
            {
                case IPEndPoint ipEndPoint:
                    LogServerInfo(
                        ipEndPoint.Address.ToString(),
                        ipEndPoint.Port
                    );
                    break;
            }
        }
    }

    public GarnetServer Start()
    {
        var server = new GarnetServer(_serverOptions);
        server.Start();
        return server;
    }

    /// <summary>
    /// Method for logging basic server information.
    /// </summary>
    [LoggerMessage(
        EventId = 0,
        Level = LogLevel.Information,
        Message = "Starting a Garnet server on {Host}:{Port}")]
    public partial void LogServerInfo(string host, int port);
}
