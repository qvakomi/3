using System;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace WolframLab.Config
{
    public class AppConfig
    {
        public string AppId { get; set; } = string.Empty;

        public static AppConfig Load()
        {
            // Строим путь к файлу конфигурации в корне проекта
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            var appConfig = new AppConfig();
            configuration.GetSection("WolframSettings").Bind(appConfig);

            if (string.IsNullOrEmpty(appConfig.AppId) || appConfig.AppId == "ВАШ_API_КЛЮЧ_ЗДЕСЬ")
            {
                throw new InvalidOperationException("API-ключ Wolfram Alpha не найден или не задан в файле appsettings.json!");
            }

            return appConfig;
        }
    }
}