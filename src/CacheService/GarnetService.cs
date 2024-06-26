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
    private readonly ILogger<GarnetService> _logger = logger;

    /// <inheritdoc/>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!stoppingToken.IsCancellationRequested)
        {
            var options = await cfgService.GetServerOptions(secretVault);
            LogServerInfo(options.Address, options.Port);

            using var server = new GarnetServer(options);
            server.Start();

            await Task.Delay(Timeout.Infinite, stoppingToken);
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
