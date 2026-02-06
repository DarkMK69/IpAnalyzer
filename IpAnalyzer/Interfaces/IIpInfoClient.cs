using System.Net;
using IpAnalyzer.Models;

namespace IpAnalyzer.Interfaces
{
    /// <summary>
    /// Интерфейс для получения информации об IP адресе
    /// </summary>
    public interface IIpInfoClient
    {
        /// <summary>
        /// Получить информацию об IP адресе
        /// </summary>
        Task<IpInfoDto> GetInfoAsync(IPAddress ip);
    }
}
