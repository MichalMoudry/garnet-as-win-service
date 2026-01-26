using CacheService;
using CacheService.Core;
using CacheService.Core.Config;
using CacheService.Core.Env;

var builder = Host.CreateApplicationBuilder(args);
builder.Services
    .AddHostedService<Worker>()
    .AddSingleton<IEnvironmentService, EnvironmentService>()
    .AddSingleton<IConfigService, ConfigService>();

var host = builder.Build();
host.Run();
