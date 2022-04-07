using System;
using System.Threading.Tasks;

namespace AzureFunctions.ApplnsightsIntegration.Api.Interfaces
{
    public interface IResponseCacheService
    {
        Task CacheResponseAsync(string cacheKey, object response, TimeSpan timeToLive);
    }
}
