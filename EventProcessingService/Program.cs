using EventProcessingService;
using StackExchange.Redis;

var builder = Host.CreateDefaultBuilder(args);

builder.ConfigureAppConfiguration((hostContext, config) =>
{
    config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
});

builder.ConfigureServices((hostContext, services) =>
{
    services.AddHostedService<EventProcessingWorker>();

    services.AddSingleton<IConnectionMultiplexer>(
        ConnectionMultiplexer.Connect(hostContext.Configuration["Redis:ConnectionString"]));

    services.AddHttpClient();
    services.AddLogging();
});

var host = builder.Build();
host.Run();
