using CacheService.Configuration;
using Garnet;

namespace CacheService;

/// <summary>
/// A service for running a Garnet server.
/// </summary>
internal sealed partial class GarnetService : BackgroundService
{
    private readonly ILogger<GarnetService> _logger;

    private readonly IConfiguration _cfg;

    public GarnetService(ILogger<GarnetService> logger, IConfiguration cfg)
    {
        _logger = logger;
        _cfg = cfg;
    }

    /// <inheritdoc/>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var options = Config.GetServerOptions(_cfg);
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
