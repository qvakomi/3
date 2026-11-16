using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace WolframLab.Services
{
    public class ResponseParser
    {
        private readonly string _outputDirectory = "Results";

        public ResponseParser()
        {
            if (!Directory.Exists(_outputDirectory))
            {
                Directory.CreateDirectory(_outputDirectory);
            }
        }

        public async Task SaveResultAsync(string queryName, string jsonResponse)
        {
            try
            {
                string safeFileName = string.Join("_", queryName.Split(Path.GetInvalidFileNameChars()));

                string fullPath = Path.Combine(_outputDirectory, $"{safeFileName}_full.json");
                await File.WriteAllTextAsync(fullPath, jsonResponse);

                string readableText = ExtractReadableAnswer(jsonResponse);

                string textPath = Path.Combine(_outputDirectory, $"{safeFileName}_result.txt");
                await File.WriteAllTextAsync(textPath, readableText);

                Console.WriteLine($"[SUCCESS] Результаты для \"{queryName}\" успешно сохранены в папку {_outputDirectory}/");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Ошибка при сохранении результатов для \"{queryName}\": {ex.Message}");
            }
        }

        private string ExtractsReadableAnswer(string jsonResponse)
        {
            try
            {
                using (JsonDocument doc = JsonDocument.Parse(jsonResponse))
                {
                    var root = doc.RootElement;
                    if (root.TryGetProperty("queryresult", out var queryResult))
                    {
                        if (queryResult.TryGetProperty("success", out var success) && success.GetBoolean())
                        {
                            if (queryResult.TryGetProperty("pods", out var pods))
                            {
                                string extractedText = "";
                                foreach (var pod in pods.EnumerateArray())
                                {
                                    string title = pod.GetProperty("title").GetString() ?? "";
                                    if (pod.TryGetProperty("subpods", out var subpods))
                                    {
                                        foreach (var subpod in subpods.EnumerateArray())
                                        {
                                            if (subpod.TryGetProperty("plaintext", out var plaintext))
                                            {
                                                string text = plaintext.GetString() ?? "";
                                                if (!string.IsNullOrEmpty(text))
                                                {
                                                    extractedText += $"[{title}] : {text}\n";
                                                }
                                            }
                                        }
                                    }
                                }
                                return string.IsNullOrEmpty(extractedText) ? "Ответ не содержит текстовых данных." : extractedText;
                            }
                        }
                    }
                }
            }
            catch
            {
            }

            return jsonResponse;
        }

        private string ExtractReadableAnswer(string jsonResponse) => ExtractsReadableAnswer(jsonResponse);
    }
}