using CurrencyMicroService.Core.Entities;
using CurrencyMicroService.Core.Exceptions;
using CurrencyMicroService.SharedModule;
using HtmlAgilityPack;
using System.Globalization;

namespace CurrencyMicroService.Infrastructure.HttpClientServices
{
    public class ETSCurrencyScraperHttpClientService
    {
        private readonly ILogger<ETSCurrencyScraperHttpClientService> _logger;
        private readonly HttpClient _httpClient;
        private readonly ApplicationSettingsProvider _settingsProvider;

        public ETSCurrencyScraperHttpClientService(
            ILogger<ETSCurrencyScraperHttpClientService> logger,
            IHttpClientFactory httpClientFactory,
            ApplicationSettingsProvider settingsProvider
            )
        {
            _logger = logger;
            _httpClient = httpClientFactory.CreateClient("ETSCurrencyScraperHttpClientService");
            _settingsProvider = settingsProvider;
        }

        public async Task<List<ETSCurrency>> GetLatestETSCurrencyRatesAsync()
        {
            try
            {
                string etsCurrencyUrl = await _settingsProvider.GetSettingAsync("ETSCurrencyUrl");

                string html = await _httpClient.GetStringAsync(etsCurrencyUrl);
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(html);

                string xpathQuery = await _settingsProvider.GetSettingAsync("ETSCurrency_XPath_Query");
                var currencyRows = htmlDoc.DocumentNode.SelectNodes(xpathQuery);

                if (currencyRows == null || !currencyRows.Any())
                {
                    _logger.LogError("No ETS currency data found in the HTML or it could be an HTML parsing problem");
                    return new List<ETSCurrency>();
                }

                var currencies = new List<ETSCurrency>();
                var fetchDate = DateTime.UtcNow;

                foreach (var row in currencyRows)
                {
                    try
                    {
                        var cells = row.SelectNodes("td");
                        if (cells != null && cells.Count >= 9)
                        {
                            var currency = new ETSCurrency
                            {
                                Name = cells[0].InnerText.Trim(),
                                Code = cells[1].InnerText.Trim(),
                                CashBuy = ParseDecimal(cells[2].InnerText.Trim()),
                                CashSell = ParseDecimal(cells[3].InnerText.Trim()),
                                TransferBuy = ParseDecimal(cells[4].InnerText.Trim()),
                                TransferSell = ParseDecimal(cells[5].InnerText.Trim()),
                                EssentialGoodsBuy = ParseDecimal(cells[6].InnerText.Trim()),
                                EssentialGoodsSell = ParseDecimal(cells[7].InnerText.Trim()),
                                WeightedAverage = ParseDecimal(cells[8].InnerText.Trim()),
                                FetchDate = fetchDate
                            };

                            currencies.Add(currency);
                        }
                    }
                    catch
                    {
                        throw new InternalServerException("Error processing ETS currency row");
                    }
                }

                return currencies;
            }
            catch
            {
                throw new InternalServerException("Error fetching currency rates");
            }
        }

        private decimal ParseDecimal(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return 0;

            string cleanValue = value.Replace(",", "");

            if (decimal.TryParse(cleanValue, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal result))
                return result;

            return 0;
        }
    }
}
