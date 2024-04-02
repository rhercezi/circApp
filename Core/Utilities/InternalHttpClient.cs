using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Core.Configs;
using Microsoft.Extensions.Logging;

namespace Core.Utilities
{
    public class InternalHttpClient<Tdto>
    {
        private readonly ILogger<InternalHttpClient<Tdto>> _logger;
        public InternalHttpClient(ILogger<InternalHttpClient<Tdto>> logger)
        {
            _logger = logger;
        }

        public async Task<Tdto> GetResource(HttpClientConfig config)
        {
            var targetUrl = $"{config.BaseUrl}:{config.Port}/{config.Path}";

            using var httpClient = new HttpClient();
            try
            {
                var response = await httpClient.GetAsync(targetUrl);
                response.EnsureSuccessStatusCode();
                var responseBody = await response.Content.ReadAsStringAsync();

                return JsonSerializer.Deserialize<Tdto>(responseBody);
            }
            catch (Exception e)
            {
                _logger.LogError($"{e.Message}\n{e.StackTrace}");
                throw;
            }
        }
    }
}