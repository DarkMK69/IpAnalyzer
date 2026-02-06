using System.Net;
using IpAnalyzer.Interfaces;
using IpAnalyzer.Models;
using Newtonsoft.Json;

namespace IpAnalyzer.Services
{
    /// <summary>
    /// Реализация клиента для получения информации об IP через HTTP API
    /// </summary>
    public class HttpInfoClient : IIpInfoClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;
        private const int RequestDelayMs = 100;

        public HttpInfoClient(HttpClient httpClient, string baseUrl)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            if (string.IsNullOrWhiteSpace(baseUrl))
                throw new ArgumentException("URL не может быть пустым", nameof(baseUrl));

            _baseUrl = baseUrl.TrimEnd('/');
        }

        /// <summary>
        /// Получить информацию об IP адресе через API
        /// </summary>
        public async Task<IpInfoDto> GetInfoAsync(IPAddress ip)
        {
            if (ip == null)
                throw new ArgumentNullException(nameof(ip));

            var url = $"{_baseUrl}/{ip}/json";
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Ошибка при запросе для IP {ip}: {response.StatusCode}");
            }

            var jsonContent = await response.Content.ReadAsStringAsync();
            var ipInfo = JsonConvert.DeserializeObject<IpInfoDto>(jsonContent);

            if (ipInfo == null)
            {
                throw new InvalidOperationException($"Не удалось десериализовать ответ для IP {ip}");
            }

            // Добавим задержку для соблюдения rate limit API
            await Task.Delay(RequestDelayMs);

            return ipInfo;
        }
    }
}
