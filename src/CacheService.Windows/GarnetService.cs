using CacheService.Core;
using CacheService.Core.Config;

namespace CacheService;

/// <summary>
/// A background service for running a Garnet server.
/// </summary>
public sealed class GarnetService(
    ILogger<ServerFacade> logger,
    ISecretVault secretVault,
    IConfigService cfgService) : BackgroundService
{
    /// <inheritdoc/>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!stoppingToken.IsCancellationRequested)
        {
            var server = new ServerFacade(logger, cfgService, secretVault);
            await server.Initialize();
            using var srvInstance = server.Start();

            await Task
                .Delay(Timeout.Infinite, stoppingToken)
                .ConfigureAwait(true);
        }
    }
}
