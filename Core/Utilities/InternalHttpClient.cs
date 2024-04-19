using System.Text.Json;
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
            var targetUrl = $"{config.BaseUrl}:{config.Port}{config.Path}";

            using var httpClient = new HttpClient();
            try
            {
                var response = await httpClient.GetAsync(targetUrl);
                response.EnsureSuccessStatusCode();
                var responseBody = await response.Content.ReadAsStringAsync();
                var dto = JsonSerializer.Deserialize<Tdto>(responseBody);
                return dto;
            }
            catch (Exception e)
            {
                _logger.LogError($"{e.Message}\n{e.StackTrace}");
                throw;
            }
        }
    }
}