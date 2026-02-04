# 🌐 IP Analyzer

Консольное приложение для анализа IP адресов с получением информации через API, статистикой и выгрузкой результатов в различные форматы.

## 📸 Скриншот работы

```
📁 Загрузка IP адресов из файла...
✓ Загружено 11 IP адресов

🌐 Получение информации об IP адресах...
  ✓ 5.18.233.41 -> RU, Saint Petersburg
  ✓ 46.147.120.9 -> RU, Rostov-na-Donu
  ✓ 91.203.56.188 -> GB, London
  ...
✓ Обработано 11 из 11 IP адресов

📊 Статистика по странам:
──────────────────────────────────────────────────────────────
Код                  | Количество      |    Процент |
DE                   |               3 |   27,27% |
RU                   |               2 |   18,18% |
...
```

## ✨ Основные возможности

- 📥 **Загрузка IP адресов** из текстовых файлов
- 🌐 **API интеграция** с ipinfo.io для получения информации об IP
- 📊 **Статистический анализ** по странам и городам
- 📤 **Множественный экспорт**: CSV, JSON, Markdown
- 🖨️ **Красивый вывод** в консоль 

## 🏗️ Архитектура проекта

Приложение построено с соблюдением **SOLID принципов**. Каждый компонент отвечает за одну задачу:

- **FileIpAddressProvider** - загрузка IP из файлов
- **HttpInfoClient** - получение информации через API
- **IpStatisticsService** - анализ статистики
- **Publishers** - выгрузка данных в различные форматы

## 📦 Структура проекта

```
IpAnalyzer/
├── README.md                      ← Этот файл
├── appsettings.json               ← Конфигурация приложения
│
├── IpAnalyzer/
│   ├── Program.cs                 ← Точка входа
│   ├── IpAnalyzer.csproj
│   ├── ip_addresses.txt               ← Файл с IP адресами
│   │
│   ├── Configuration/
│   │   ├── AppConfig.cs
│   │   ├── PathsConfig.cs
│   │   └── ApiConfig.cs
│   │
│   ├── Interfaces/
│   │   ├── IIpAddressProvider.cs
│   │   ├── IIpInfoClient.cs
│   │   ├── IIpStatisticsService.cs
│   │   └── IIpStatisticsPublisher.cs
│   │
│   ├── Services/
│   │   ├── FileIpAddressProvider.cs
│   │   ├── HttpInfoClient.cs
│   │   ├── IpStatisticsService.cs
│   │   ├── ConsoleIpStatisticsPublisher.cs
│   │   ├── FileIpStatisticsPublisher.cs
│   │   └── MarkdownIpStatisticsPublisher.cs
│   │
│   ├── Models/
│   │   ├── IpInfoDto.cs
│   │   ├── CountedData.cs
│   │   ├── CountryIpDetails.cs
│   │   └── CityIpDetails.cs
│   │
│   └── output/
│       ├── countries_statistics.csv
│       ├── countries_statistics.json
│       ├── cities_statistics.csv
│       ├── cities_statistics.json
│       └── report.md
│
└── .gitignore
```

## 🚀 Быстрый старт

### Требования

- **.NET 8.0** или выше
- **Windows, Linux или macOS**
- Интернет подключение (для API запросов)

### Установка

```bash
# Клонируем репозиторий
git clone https://github.com/yourusername/IpAnalyzer.git
cd IpAnalyzer

# Восстанавливаем зависимости
cd IpAnalyzer
dotnet restore

# Собираем проект
dotnet build
```

### Использование

1. **Подготовьте файл с IP адресами** (ip_addresses.txt в корне проекта):
```
5.18.233.41
46.147.120.9
91.203.56.188
89.163.214.77
138.201.95.12
```

2. **Запустите приложение**
```bash
dotnet run
```

3. **Результаты будут в папке `output/`:**
- `countries_statistics.csv` - статистика по странам (CSV)
- `countries_statistics.json` - статистика по странам (JSON)
- `cities_statistics.csv` - статистика по городам (CSV)
- `cities_statistics.json` - статистика по городам (JSON)
- `report.md` - объединенный отчет (Markdown)

## ⚙️ Конфигурация

Все настройки в файле **appsettings.json**:

```json
{
  "Paths": {
    "IpAddressesFile": "../../../ip_addresses.txt",
    "OutputDirectory": "../../../output"
  },
  "ApiSettings": {
    "IpInfoUrl": "https://ipinfo.io"
  }
}
```

**Доступные параметры:**
- `Paths.IpAddressesFile` - путь к файлу с IP адресами (абсолютный или относительный)
- `Paths.OutputDirectory` - папка для сохранения результатов
- `ApiSettings.IpInfoUrl` - URL API для получения информации об IP


## 📤 Форматы выгрузки

### CSV - для таблиц
Откройте в Excel/Google Sheets:
```csv
Код страны,Название страны,Количество IP,Процент
DE,DE,3,27.27%
RU,RU,2,18.18%
GB,GB,1,9.09%
```

### JSON - для программ
Структурированный формат:
```json
[
  {
    "countryCode": "DE",
    "countryName": "DE",
    "count": 3,
    "percentage": 27.27
  }
]
```

### Markdown - для документации
Красивый отчет с таблицами:
```markdown
# IP Адреса - Отчет анализа

**Дата генерации:** 04.02.2026 18:05:52

## 📊 Статистика по странам

| Код | Название | Количество IP | Процент |
|-----|----------|---------------|---------|
| DE | DE | 3 | 27,27% |
```

## 🔌 API интеграция

Приложение использует свободный API [ipinfo.io](https://ipinfo.io/):

```bash
GET https://ipinfo.io/{ip}/json
```

**Ограничения API:**
- 50,000 запросов в месяц (бесплатно)
- Rate limit: 1 запрос в 100ms


## 🛠️ Разработка

### Добавление нового издателя

1. Создайте класс, наследующий `IIpStatisticsPublisher`:
```csharp
public class ExcelPublisher : IIpStatisticsPublisher
{
    public async Task PublishAsync<T>(IEnumerable<T> data)
    {
        // Реализация экспорта в Excel
    }
}
```

2. Используйте в Program.cs:
```csharp
var excelPublisher = new ExcelPublisher();
await excelPublisher.PublishAsync(statisticsService.SortedCountryIpDetails);
```

### Изменение источника данных

1. Создайте новый провайдер:
```csharp
public class DatabaseIpProvider : IIpAddressProvider
{
    public async Task<IEnumerable<IPAddress>> GetAsync(CancellationToken ct = default)
    {
        // Загрузка из БД
    }
}
```

### Использование альтернативного API

Обновите в `appsettings.json`:
```json
{
  "ApiSettings": {
    "IpInfoUrl": "https://ip-api.com/json"
  }
}
```

## 🐛 Известные проблемы

- API ipinfo.io имеет ограничение 50K запросов/месяц
- Первый запрос может быть медленным (DNS резолюция)
- Для больших файлов (10K+ IP) используйте batch обработку

