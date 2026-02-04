using System.Collections.Immutable;
using IpAnalyzer.Interfaces;
using IpAnalyzer.Models;

namespace IpAnalyzer.Services
{
    /// <summary>
    /// Сервис для анализа и обработки статистики IP адресов
    /// Получает уже готовый набор данных и рассчитывает статистику
    /// </summary>
    public class IpStatisticsService : IIpStatisticsService
    {
        private readonly IEnumerable<IpInfoDto> _ipInfoList;
        private ImmutableList<CountryIpDetails> _sortedCountryDetails = ImmutableList<CountryIpDetails>.Empty;
        private ImmutableList<CityIpDetails> _sortedCityDetails = ImmutableList<CityIpDetails>.Empty;

        /// <summary>
        /// Инициализирует сервис с готовым набором данных об IP адресах
        /// </summary>
        public IpStatisticsService(IEnumerable<IpInfoDto> ipInfoList)
        {
            _ipInfoList = ipInfoList ?? throw new ArgumentNullException(nameof(ipInfoList));
        }

        public ImmutableList<CountryIpDetails> SortedCountryIpDetails => _sortedCountryDetails;
        public ImmutableList<CityIpDetails> SortedCityIpDetails => _sortedCityDetails;

        /// <summary>
        /// Рассчитать статистику по полученным данным
        /// </summary>
        public Task ProcessAsync()
        {
            CalculateStatistics();
            return Task.CompletedTask;
        }

        /// <summary>
        /// Рассчитать статистику по странам и городам
        /// </summary>
        private void CalculateStatistics()
        {
            var countryStats = CalculateCountryStatistics();
            var cityStats = CalculateCityStatistics(countryStats);

            _sortedCountryDetails = ImmutableList.CreateRange(countryStats);
            _sortedCityDetails = ImmutableList.CreateRange(cityStats);
        }

        /// <summary>
        /// Рассчитать статистику по странам
        /// </summary>
        private List<CountryIpDetails> CalculateCountryStatistics()
        {
            var countryGroups = _ipInfoList
                .Where(ip => !string.IsNullOrWhiteSpace(ip.Country))
                .GroupBy(ip => ip.Country)
                .ToList();

            var totalCount = _ipInfoList.Count();

            return countryGroups
                .Select(group => new CountryIpDetails
                {
                    CountryCode = group.Key,
                    CountryName = group.Key,
                    Count = group.Count(),
                    Percentage = (group.Count() / (double)totalCount) * 100
                })
                .OrderByDescending(stat => stat.Count)
                .ToList();
        }

        /// <summary>
        /// Рассчитать статистику по городам страны с наибольшим количеством IP
        /// </summary>
        private List<CityIpDetails> CalculateCityStatistics(List<CountryIpDetails> countryStats)
        {
            var topCountry = countryStats.FirstOrDefault();
            if (topCountry == null)
                return new List<CityIpDetails>();

            var ipListForCountry = _ipInfoList
                .Where(ip => ip.Country == topCountry.CountryCode)
                .ToList();

            var totalCountryIps = ipListForCountry.Count;

            return ipListForCountry
                .Where(ip => !string.IsNullOrWhiteSpace(ip.City))
                .GroupBy(ip => new { City = ip.City, Region = ip.Region })
                .Select(group => new CityIpDetails
                {
                    City = group.Key.City,
                    Region = group.Key.Region,
                    CountryCode = topCountry.CountryCode,
                    Count = group.Count(),
                    Percentage = (group.Count() / (double)totalCountryIps) * 100
                })
                .OrderByDescending(city => city.Count)
                .ToList();
        }
    }
}
