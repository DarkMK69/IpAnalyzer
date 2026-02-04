namespace IpAnalyzer.Configuration
{
    /// <summary>
    /// Конфигурация путей приложения
    /// </summary>
    public class PathsConfig
    {
        public string IpAddressesFile { get; set; } = "ip_addresses.txt";
        public string OutputDirectory { get; set; } = "output";
    }
}
