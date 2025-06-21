using System.Net;
using CacheService.Configuration;
using Garnet;

namespace CacheService;

/// <summary>
/// A service for running a Garnet server.
/// </summary>
internal sealed partial class GarnetService(
    ILogger<GarnetService> logger,
    ISecretVault secretVault,
    IConfigService cfgService) : BackgroundService
{
    /// <inheritdoc/>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!stoppingToken.IsCancellationRequested)
        {
            var options = await cfgService
                .GetServerOptions(secretVault)
                .ConfigureAwait(true);
            foreach (var endPoint in options.EndPoints)
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

            using var server = new GarnetServer(options);
            server.Start();

            await Task
                .Delay(Timeout.Infinite, stoppingToken)
                .ConfigureAwait(true);
        }
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
