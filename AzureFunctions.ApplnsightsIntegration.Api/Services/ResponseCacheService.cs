using AzureFunctions.ApplnsightsIntegration.Api.Interfaces;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace AzureFunctions.ApplnsightsIntegration.Api.Services
{
    public class ResponseCacheService : IResponseCacheService
    {
        private readonly IDistributedCache _distributedCache;
        private readonly TelemetryClient _telemetryClient;
        private readonly ILogger<ResponseCacheService> _logger;

        public ResponseCacheService(
            IDistributedCache distributedCache,
            TelemetryClient telemetryClient,
            ILogger<ResponseCacheService> logger
        )
        {
            _distributedCache = distributedCache;
            _telemetryClient = telemetryClient;
            _logger = logger;
        }

        public async Task CacheResponseAsync(string cacheKey, object response, TimeSpan timeToLive)
        {
            if (response == null)
            {
                return;
            }

            var serializedResponse = JsonSerializer.Serialize(response);

            var dependency = new DependencyTelemetry
            {
                Type = "Redis",
                Name = nameof(DistributedCacheExtensions.SetStringAsync)
            };
            dependency.Properties[nameof(cacheKey)] = cacheKey;

            // Log a Redis dependency in the dependencies table.
            using (_telemetryClient.StartOperation(dependency))
            {
                try
                {
                    var start = DateTime.UtcNow;
                    dependency.Timestamp = start;

                    await _distributedCache.SetStringAsync(cacheKey, serializedResponse, new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = timeToLive
                    });

                    dependency.Duration = DateTime.UtcNow - start;
                    dependency.Success = true;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Exception in {ServiceName} -> {MethodName}.", nameof(ResponseCacheService), nameof(CacheResponseAsync));

                    dependency.Success = false;
                }
            }
        }
    }
}
