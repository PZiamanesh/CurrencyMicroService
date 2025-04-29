using CurrencyMicroService.Infrastructure;
using Hangfire;
using Hangfire.SqlServer;
using Microsoft.EntityFrameworkCore;
using System.Net;
using Serilog.Sinks.MSSqlServer;
using Serilog;
using Serilog.Events;
using CurrencyMicroService.SharedModule;
using CurrencyMicroService;

var builder = WebApplication.CreateBuilder(args);

// ===== ioc container section

#region Api Layer Configs

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

#endregion

#region Core Layer Configs

builder.Services.AddLayerServices(builder.Configuration);

builder.Services.AddSingleton<ApplicationSettingsProvider>();

builder.Services.AddScoped<JobsSchedulingRegistrar>();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddProblemDetails();

#endregion

#region Infrastructure Layer Configs

// global tls protocol
ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

// dbContext
builder.Services.AddDbContext<CurrencySyncContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("CurrencyDbConnection")));

// hangfire configuration and registration
builder.Services.AddHangfire(configuration => configuration
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseSqlServerStorage(builder.Configuration.GetConnectionString("HangfireConnection"), new SqlServerStorageOptions
    {
        CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
        SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
        QueuePollInterval = TimeSpan.Zero,
        UseRecommendedIsolationLevel = true,
        DisableGlobalLocks = true
    })
    .UseFilter(new AutomaticRetryAttribute { Attempts = 3, DelaysInSeconds = new[] { 5, 8, 12 } })
);

builder.Services.AddHangfireServer();

// serilog sinks registraton
var sinkOpts = new MSSqlServerSinkOptions
{
    TableName = "Logs",
    AutoCreateSqlTable = true
};
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Warning()
    .MinimumLevel.Override("CurrencyMicroService.Core", LogEventLevel.Information)
    .MinimumLevel.Override("CurrencyMicroService.Infrastructure", LogEventLevel.Information)
    .MinimumLevel.Override("CurrencyMicroService.SharedModule", LogEventLevel.Information)

    .WriteTo.Console() // console sink

    .WriteTo.MSSqlServer(
        connectionString: builder.Configuration.GetConnectionString("CurrencyDbConnection"),
        sinkOptions: sinkOpts) // sqlServer sink

    .CreateLogger();

builder.Host.UseSerilog();

#endregion

// ===== request pipeline section

var app = builder.Build();

#region Request Pipeline

app.UseExceptionHandler();

// initialize application settings
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();

    try
    {
        var dbContext = services.GetRequiredService<CurrencySyncContext>();
        dbContext.Database.EnsureCreated();

        var settingsProvider = services.GetRequiredService<ApplicationSettingsProvider>();
        await settingsProvider.InitializeAsync();

        logger.LogInformation("Application settings initialized successfully");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred during initialization of application settings");
    }
}

app.UseSwagger();

app.UseSwaggerUI();

app.UseSerilogRequestLogging();

app.UseHangfireDashboard();

app.MapControllers();

// invoke job scheduler - scheduling jobs are registred in JobsSchedulingRegistrar class
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();

    try
    {
        var jobsRegistrar = services.GetRequiredService<JobsSchedulingRegistrar>();
        await jobsRegistrar.RegisterJobsAsync();

        logger.LogInformation("Application started and initialization complete");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred during job registration");
    }
}

#endregion

app.Run();

