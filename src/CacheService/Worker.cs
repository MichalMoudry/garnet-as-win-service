using CacheService.Core;

namespace CacheService;

public sealed partial class Worker(
    ILogger<Worker> workerLog,
    ILogger<ServerFacade> log,
    IConfigService configSrvc)
    : BackgroundService
{
    /// <inheritdoc/>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            LogServiceVersion(
                workerLog,
                ThisAssembly.AssemblyInformationalVersion
            );
            var wrapper = new ServerFacade(log, configSrvc);
            await wrapper.Initialize();
            using var server = wrapper.Start();

            await Task
                .Delay(Timeout.Infinite, stoppingToken)
                .ConfigureAwait(true);
        }
    }

    [LoggerMessage(LogLevel.Information, "Service version: {version}")]
    private static partial void LogServiceVersion(
        ILogger<Worker> log,
        string version
    );
}
