using CurrencyMicroService.Core.Entities;
using CurrencyMicroService.Core.Exceptions;
using CurrencyMicroService.Core.Exceptions.MessageTemplates;
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
                _logger.LogInformation($"Starting {nameof(ETSCurrencyUpdateJob)}");

                var currencyInfos = await _scraperService.GetLatestETSCurrencyRatesAsync();

                if (currencyInfos == null || !currencyInfos.Any())
                {
                    _logger.LogWarning($"No {nameof(ETSCurrency)} data fetched, skipping database update");
                    return;
                }

                await _currencyService.UpdateETSCurrenciesAsync(currencyInfos);

                _logger.LogInformation($"{nameof(ETSCurrencyUpdateJob)} completed successfully");
            }
            catch
            {
                throw new InternalServerException(string.Format(
                    ExceptionMessages.JobExecutionError,
                    nameof(ETSCurrencyUpdateJob)));
            }
        }
    }
}
