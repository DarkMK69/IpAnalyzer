using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using IpAnalyzer.Interfaces;
using IpAnalyzer.Services;
using IpAnalyzer.Models;
using IpAnalyzer.Configuration;

namespace IpAnalyzer
{
    class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                var basePath = AppDomain.CurrentDomain.BaseDirectory;
                
                var configBuilder = new ConfigurationBuilder()
                    .SetBasePath(basePath)
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .Build();

                var appConfig = new AppConfig();
                configBuilder.Bind(appConfig);

                var services = new ServiceCollection();
                ConfigureServices(services, appConfig);

                var serviceProvider = services.BuildServiceProvider();


                var ipFilePath = Path.IsPathRooted(appConfig.Paths.IpAddressesFile) 
                    ? appConfig.Paths.IpAddressesFile 
                    : Path.GetFullPath(Path.Combine(basePath, appConfig.Paths.IpAddressesFile));
                    
                var outputDirectory = Path.IsPathRooted(appConfig.Paths.OutputDirectory)
                    ? appConfig.Paths.OutputDirectory
                    : Path.GetFullPath(Path.Combine(basePath, appConfig.Paths.OutputDirectory));
                    
                var apiUrl = appConfig.ApiSettings.IpInfoUrl;

                Console.WriteLine("Загрузка IP адресов из файла...");
                var addressProvider = new FileIpAddressProvider(ipFilePath);
                var ipAddresses = await addressProvider.GetAsync();
                Console.WriteLine($"✓ Загружено {ipAddresses.Count()} IP адресов\n");


                var httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
                var httpClient = httpClientFactory.CreateClient();
                var ipInfoClient = new HttpInfoClient(httpClient, apiUrl);

                Console.WriteLine("Получение информации об IP адресах...");
                var ipInfoList = new List<IpInfoDto>();

                foreach (var ip in ipAddresses)
                {
                    try
                    {
                        var ipInfo = await ipInfoClient.GetInfoAsync(ip);
                        ipInfoList.Add(ipInfo);
                        Console.WriteLine($"  ✓ {ip} -> {ipInfo.Country}, {ipInfo.City}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"  ✗ {ip} -> {ex.Message}");
                    }
                }

                Console.WriteLine($"✓ Обработано {ipInfoList.Count} из {ipAddresses.Count()} IP адресов\n");

                var statisticsService = new IpStatisticsService(ipInfoList);
                await statisticsService.ProcessAsync();

                var publisher = new ConsoleIpStatisticsPublisher();

                Console.WriteLine();
                await publisher.PublishAsync(statisticsService.SortedCountryIpDetails);
                await publisher.PublishAsync(statisticsService.SortedCityIpDetails);


                var filePublisher = new FileIpStatisticsPublisher(outputDirectory);
                Console.WriteLine("\n📁 Выгрузка данных в файлы...");
                await filePublisher.PublishAsync(statisticsService.SortedCountryIpDetails);
                await filePublisher.PublishAsync(statisticsService.SortedCityIpDetails);

                var markdownPublisher = new MarkdownIpStatisticsPublisher(outputDirectory);
                Console.WriteLine("\n📋 Генерация Markdown отчета...");
                await markdownPublisher.PublishAsync(statisticsService.SortedCountryIpDetails);
                await markdownPublisher.PublishAsync(statisticsService.SortedCityIpDetails);

                Console.WriteLine("\n✅ Анализ завершен!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Ошибка: {ex.Message}");
                Environment.Exit(1);
            }
        }

        /// <summary>
        /// Настройка сервисов для DI контейнера
        /// </summary>
        private static void ConfigureServices(ServiceCollection services, AppConfig appConfig)
        {
            // HttpClientFactory для создания HttpClient
            services.AddHttpClient();
        }
    }
}
