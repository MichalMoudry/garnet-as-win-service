using CacheService;
using CacheService.Configuration;

var builder = Host.CreateApplicationBuilder(args);

builder.Configuration.AddCommandLine(args);

builder.Services
    .AddTransient<ISecretVault, AzureKeyVault>()
    .AddHostedService<GarnetService>()
    .AddWindowsService(options =>
    {
        options.ServiceName = "Garnet cache service";
    });

var host = builder.Build();
host.Run();
