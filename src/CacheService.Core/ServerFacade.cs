using System.Net;
using CacheService.Core.Config;
using Garnet;
using Garnet.server;
using Microsoft.Extensions.Logging;

namespace CacheService.Core;

/// <summary>
/// A wrapper around <see cref="GarnetServer"/> to simplify the cache server
/// initialization.
/// </summary>
public sealed partial class ServerFacade
{
    private GarnetServerOptions? _serverOptions;
    private readonly ILogger<ServerFacade> _log;
    private readonly IConfigService _configService;
    private readonly ISecretVault? _secretVault;

    public ServerFacade(
        ILogger<ServerFacade> logger,
        IConfigService config,
        ISecretVault vault)
    {
        _log = logger;
        _configService = config;
        _secretVault = vault;
    }

    public ServerFacade(ILogger<ServerFacade> logger, IConfigService config)
    {
        _log = logger;
        _configService = config;
    }

    public async Task Initialize()
    {
        _serverOptions = _secretVault switch
        {
            not null => await _configService.GetServerOptions(_secretVault),
            null => _configService.GetServerOptions()
        };

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
