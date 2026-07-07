using Fabiq.SmartFactory.Api.Data;
using Fabiq.SmartFactory.Api.Options;
using Fabiq.SmartFactory.Api.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddProblemDetails();
builder.Services.AddHealthChecks();

builder.Services.AddDbContext<SmartFactoryDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("SmartFactoryDb")));

builder.Services.Configure<KafkaOptions>(builder.Configuration.GetSection("Kafka"));
builder.Services.AddSingleton<ApplicationMetrics>();
builder.Services.AddScoped<FactoryEventProcessor>();
builder.Services.AddHostedService<KafkaIngestionHostedService>();

builder.Logging.ClearProviders();
builder.Logging.AddJsonConsole();
builder.Logging.SetMinimumLevel(LogLevel.Information);

// Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendDev", policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:3000",
                "http://127.0.0.1:3000",
                "http://localhost:5173",
                "https://localhost:5173"
            )
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseExceptionHandler("/error");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseCors("FrontendDev");

app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");

app.Run();