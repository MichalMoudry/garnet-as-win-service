using CacheService;
using CacheService.Configuration;
using CacheService.Configuration.Azure;
using CacheService.Configuration.Env;

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
