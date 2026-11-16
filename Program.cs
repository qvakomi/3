using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WolframLab.Config;
using WolframLab.Services;

namespace WolframLab
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("=== Запуск программы для работы с Wolfram Alpha API ===");

            try
            {
                AppConfig config = AppConfig.Load();
                Console.WriteLine("[INFO] Конфигурация успешно загружена.");

                var client = new WolframClient(config.AppId);
                var parser = new ResponseParser();
                var queries = new Dictionary<string, string>
                {
                    { "1_Math_Integral", "integrate x^2 sin^3 x dx" },
                    { "2_Fact_Population", "population of Russia 2024" },
                    { "3_Chem_MolarMass", "molar mass of H2SO4" },
                    { "4_Phys_KineticEnergy", "kinetic energy of 5kg object at 10m/s" },
                    { "5_Geo_Distance", "distance between Moscow and Saint Petersburg" },
                    { "6_Finance_Currency", "100 USD to RUB" },
                    { "7_Time_CurrentTime", "current time in Kazan" },
                    { "8_Astronomy_DistanceToMars", "distance to Mars" },
                    { "9_Medical_BMI", "body mass index 180cm 75kg" },
                    { "10_Linguistic_Translate", "translate hello to Russian" },
                    { "11_Meteo_Weather", "weather in Tokyo" },
                    { "12_Physics_EscapeVelocity", "escape velocity of Earth" },
                    { "13_GeoBio_HighestMountain", "highest mountain in the world" },
                    { "14_Tech_Binary", "binary representation of 255" },
                    { "15_Nutrition_Calories", "calories in 100g banana" }
                };

                foreach (var kvp in queries)
                {
                    string queryName = kvp.Key;
                    string queryString = kvp.Value;

                    Console.WriteLine($"\n----------------------------------------");
                    Console.WriteLine($"Выполнение запроса: {queryName} -> \"{queryString}\"");

                    string jsonResponse = await client.GetQueryResultAsync(queryString);

                    if (!string.IsNullOrEmpty(jsonResponse))
                    {
                        await parser.SaveResultAsync(queryName, jsonResponse);
                    }
                    else
                    {
                        Console.WriteLine($"[WARNING] Не удалось получить ответ для запроса: {queryName}");
                    }

                    await Task.Delay(1000);
                }

                Console.WriteLine("\n=== Все запросы успешно выполнены и сохранены в папку Results/ ===");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n[FATAL ERROR] Произошла критическая ошибка: {ex.Message}");
            }
        }
    }
}
