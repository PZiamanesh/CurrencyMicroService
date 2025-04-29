using CurrencyMicroService.Core.Exceptions;
using CurrencyMicroService.Core.Interfaces;
using CurrencyMicroService.Infrastructure.HttpClientServices;

namespace CurrencyMicroService.Infrastructure.Jobs
{
    public class ETSCurrencyUpdateJob
    {
        private readonly ETSCurrencyScraperHttpClientService _scraperService;
        private readonly IETSCurrencyService _currencyService;
        private readonly ILogger<ETSCurrencyUpdateJob> _logger;

        public ETSCurrencyUpdateJob(
            ETSCurrencyScraperHttpClientService scraperService,
            IETSCurrencyService currencyService,
            ILogger<ETSCurrencyUpdateJob> logger
            )
        {
            _scraperService = scraperService;
            _currencyService = currencyService;
            _logger = logger;
        }

        public async Task ExecuteAsync()
        {
            try
            {
                _logger.LogInformation("Starting ETS currency update job");

                var currencyInfos = await _scraperService.GetLatestETSCurrencyRatesAsync();

                if (currencyInfos == null || !currencyInfos.Any())
                {
                    _logger.LogWarning("No ETS currency data fetched, skipping database update");
                    return;
                }

                await _currencyService.UpdateETSCurrenciesAsync(currencyInfos);

                _logger.LogInformation("ETS Currency update job completed successfully");
            }
            catch
            {
                throw new InternalServerException("Error in ETS currency update job");
            }
        }
    }
}
