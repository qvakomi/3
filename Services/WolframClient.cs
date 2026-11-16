using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace WolframLab.Services
{
    public class WolframClient
    {
        private readonly string _appId;
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://api.wolframalpha.com/v2/query";

        public WolframClient(string appId)
        {
            _appId = appId;
            _httpClient = new HttpClient();
            
            if (!_httpClient.DefaultRequestHeaders.Contains("User-Agent"))
            {
                _httpClient.DefaultRequestHeaders.Add("User-Agent", "WolframLabConsoleApp/1.0");
            }
        }

        public async Task<string> GetQueryResultAsync(string inputQuery)
        {
            try
            {
                string url = $"{BaseUrl}?input={Uri.EscapeDataString(inputQuery)}&appid={_appId}&output=json&translated=true";

                Console.WriteLine($"[LOG] Отправка запроса: \"{inputQuery}\"...");

                HttpResponseMessage response = await _httpClient.GetAsync(url);
                
                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException($"Ошибка сети или сервера! Статус: {response.StatusCode}");
                }

                string jsonResponse = await response.Content.ReadAsStringAsync();
                return jsonResponse;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Ошибка при запросе \"{inputQuery}\": {ex.Message}");
                return string.Empty;
            }
        }
    }
}