using CacheService.Configuration;
using CacheService.Configuration.Env;
using Garnet;

namespace CacheService;

/// <summary>
/// A service for running a Garnet server.
/// </summary>
internal sealed partial class GarnetService : BackgroundService
{
    private readonly ILogger<GarnetService> _logger;

    private readonly ISecretVault _secretVault;

    private readonly IConfigService _cfgService;

    public GarnetService(
        ILogger<GarnetService> logger,
        ISecretVault secretVault,
        IConfigService cfgService)
    {
        _logger = logger;
        _secretVault = secretVault;
        _cfgService = cfgService;
    }

    /// <inheritdoc/>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var options = await _cfgService.GetServerOptions(_secretVault);
            LogServerInfo(options.Address, options.Port);

            var server = new GarnetServer(options);
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
