using AzureFunctions.ApplnsightsIntegration.Api.ApplicationInsights;
using AzureFunctions.ApplnsightsIntegration.Api.Interfaces;
using AzureFunctions.ApplnsightsIntegration.Api.Services;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;

[assembly: FunctionsStartup(typeof(AzureFunctions.ApplnsightsIntegration.Api.Startup))]
namespace AzureFunctions.ApplnsightsIntegration.Api
{
    // DI https://docs.microsoft.com/en-us/azure/azure-functions/functions-dotnet-dependency-injection
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            // Use telemetry initializers when you want to enrich telemetry with additional information
            builder.Services.AddSingleton<ITelemetryInitializer, CloudRoleTelemetryInitializer>();

            builder.Services.AddStackExchangeRedisCache(options => options.Configuration = Environment.GetEnvironmentVariable("REDISCACHE_CONNECTIONSTRING"));
            builder.Services.AddSingleton<IResponseCacheService, ResponseCacheService>();
        }
    }
}