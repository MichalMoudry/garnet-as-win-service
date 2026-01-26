using CacheService.Core;

namespace CacheService;

public sealed class Worker(ILogger<ServerFacade> log, IConfigService configSrvc)
    : BackgroundService
{
    /// <inheritdoc/>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var wrapper = new ServerFacade(log, configSrvc);
            await wrapper.Initialize();
            using var server = wrapper.Start();

            await Task
                .Delay(Timeout.Infinite, stoppingToken)
                .ConfigureAwait(true);
        }
    }
}
