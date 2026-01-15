using CacheService.Configuration;
using CacheService.Core;
using CacheService.Core.Config;
using CacheService.Core.Env;

var builder = Host.CreateApplicationBuilder(args);
builder.Configuration.AddCommandLine(args);

builder.Services
    .AddSingleton<IEnvironmentService, EnvironmentService>()
    .AddTransient<ISecretVault, AzureKeyVault>()
    .AddSingleton<IConfigService, ConfigService>()
    .AddHostedService<GarnetService>()
    .AddWindowsService(options =>
    {
        options.ServiceName = "Garnet cache service";
    });

var host = builder.Build();
host.Run();
