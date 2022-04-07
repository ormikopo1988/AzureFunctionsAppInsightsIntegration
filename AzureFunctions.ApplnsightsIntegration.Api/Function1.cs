using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using AzureFunctions.ApplnsightsIntegration.Api.Interfaces;

namespace AzureFunctions.ApplnsightsIntegration.Api
{
    public class Function1
    {
        private readonly IResponseCacheService _responseCacheService;

        public Function1(IResponseCacheService responseCacheService)
        {
            _responseCacheService = responseCacheService;
        }

        [FunctionName("Function1")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string name = req.Query["name"];

            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name ??= data?.name;

            var responseMessage = string.IsNullOrEmpty(name)
                ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
                : $"Hello, {name}. This HTTP triggered function executed successfully.";

            await _responseCacheService.CacheResponseAsync(string.IsNullOrEmpty(name) ? "testKey" : name, responseMessage, TimeSpan.FromSeconds(600));

            return new OkObjectResult(responseMessage);
        }
    }
}
