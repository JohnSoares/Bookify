using Asp.Versioning;
using Asp.Versioning.Builder;
using Bookify.Api.Extensions;
using Bookify.Application;
using Bookify.Infrastructure;
using Bookify.Infrastructure.OpenTelemetry;
using Hangfire;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Serilog;
using Serilog.Sinks.OpenTelemetry;
using System.Reflection;
using Web.Api.Extensions;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Host.UseSerilog((context, loggerConfig) =>
    loggerConfig
        .ReadFrom.Configuration(context.Configuration)
        .WriteTo.OpenTelemetry(o =>
        {
            o.Endpoint = context.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"]!;
            o.Protocol = OtlpProtocol.HttpProtobuf;
            o.Headers = ParseOtlpHeaders(context.Configuration["OTEL_EXPORTER_OTLP_HEADERS"]);
            o.ResourceAttributes = new Dictionary<string, object>
            {
                { "service.name", DiagnosticsConfig.ServiceName }
            };
        }));

builder.Services
    .AddApplication()
    .AddPresentation()
    .AddInfrastructure(builder.Configuration);

builder.Services.AddEndpoints(Assembly.GetExecutingAssembly());

WebApplication app = builder.Build();

app.MapDefaultEndpoints();

ApiVersionSet apiVersionSet = app.NewApiVersionSet()
    .HasApiVersion(new ApiVersion(1))
    .ReportApiVersions()
    .Build();

RouteGroupBuilder versionedGroup = app
    .MapGroup("api/v{version:apiVersion}")
    .WithApiVersionSet(apiVersionSet);

app.MapEndpoints(versionedGroup);

if (!app.Environment.IsEnvironment("Testing"))
{
    app.UseBackgroundJobs();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwaggerWithUi();

    app.UseHangfireDashboard(options: new DashboardOptions
    {
        Authorization = [],
        DarkModeEnabled = false
    });

    app.ApplyMigrations();

    // REMARK: Uncomment if you want to seed initial data.
    //app.SeedData();
}

if (app.Environment.IsEnvironment("Testing"))
{
    app.ApplyMigrations();
    app.SeedData();
}

app.UseHttpsRedirection();

app.MapHealthChecks("health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.UseRequestContextLogging();

app.UseSerilogRequestLogging();

app.UseExceptionHandler();

app.UseAuthentication();

app.UseAuthorization();

await app.RunAsync();

static Dictionary<string, string> ParseOtlpHeaders(string? headers)
{
    if (string.IsNullOrWhiteSpace(headers))
    {
        return [];
    }

    return headers
        .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
        .Select(header => header.Split('=', 2))
        .Where(parts => parts.Length == 2)
        .ToDictionary(
            parts => Uri.UnescapeDataString(parts[0]),
            parts => Uri.UnescapeDataString(parts[1]));
}
