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

    private readonly IEnvironmentService _envService;

    public GarnetService(
        ILogger<GarnetService> logger,
        ISecretVault secretVault,
        IConfigService cfgService,
        IEnvironmentService envService)
    {
        _logger = logger;
        _secretVault = secretVault;
        _cfgService = cfgService;
        _envService = envService;
    }

    /// <inheritdoc/>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!stoppingToken.IsCancellationRequested)
        {
            var options = await _cfgService.GetServerOptions(
                _secretVault,
                _envService.CurrentEnvironment
            );
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
