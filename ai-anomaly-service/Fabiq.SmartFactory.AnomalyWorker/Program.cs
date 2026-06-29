using Fabiq.SmartFactory.AnomalyWorker.Services;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddSingleton<AnomalyDetectionService>();
builder.Services.AddHostedService<TelemetryAnomalyWorker>();

var host = builder.Build();
host.Run();