namespace IpAnalyzer.Configuration
{
    /// <summary>
    /// Главная конфигурация приложения
    /// </summary>
    public class AppConfig
    {
        public PathsConfig Paths { get; set; } = new();
        public ApiConfig ApiSettings { get; set; } = new();
    }
}
