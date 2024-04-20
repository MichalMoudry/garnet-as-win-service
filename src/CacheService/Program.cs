using CacheService;

var builder = Host.CreateApplicationBuilder(args);

builder.Configuration.AddCommandLine(args);

builder.Services
    .AddHostedService<GarnetService>()
    .AddWindowsService(options =>
    {
        options.ServiceName = "Garnet cache service";
    });

var host = builder.Build();
host.Run();
