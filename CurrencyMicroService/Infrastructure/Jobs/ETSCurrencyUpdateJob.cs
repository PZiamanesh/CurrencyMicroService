using CurrencyMicroService.Core.Entities;
using CurrencyMicroService.Core.Exceptions;
using CurrencyMicroService.Core.Exceptions.MessageTemplates;
using CurrencyMicroService.Core.Interfaces;
using CurrencyMicroService.Infrastructure.HttpClientServices;
using CurrencyMicroService.Infrastructure.JobServices;

namespace CurrencyMicroService.Infrastructure.Jobs
{
    public class ETSCurrencyUpdateJob
    {
        private readonly ETSCurrencyScraperHttpClientService _scraperService;
        private readonly IETSCurrencyService _currencyService;
        private readonly ILogger<ETSCurrencyUpdateJob> _logger;
        private readonly IETSCurrencyJobSchedulerService _etsJobSchedulerService;

        public ETSCurrencyUpdateJob(
            ETSCurrencyScraperHttpClientService scraperService,
            IETSCurrencyService currencyService,
            ILogger<ETSCurrencyUpdateJob> logger,
            IETSCurrencyJobSchedulerService jobSchedulerService)
        {
            _scraperService = scraperService;
            _currencyService = currencyService;
            _logger = logger;
            _etsJobSchedulerService = jobSchedulerService;
        }

        public async Task ExecuteAsync()
        {
            bool wasSuccessful = false;

            try
            {
                _logger.LogInformation($"Starting {nameof(ETSCurrencyUpdateJob)}");

                bool hasDataForToday = await _etsJobSchedulerService.HasDataForTodayAsync();
                if (hasDataForToday)
                {
                    _logger.LogInformation($"{nameof(ETSCurrency)} data already exists for today. Skipping fetch and scheduling for tomorrow.");
                    wasSuccessful = true;
                    return;
                }

                var etsCurrencies = await _scraperService.GetLatestETSCurrencyRatesAsync();

                if (etsCurrencies == null || !etsCurrencies.Any())
                {
                    _logger.LogWarning($"No {nameof(ETSCurrency)} data fetched from remote source");
                    wasSuccessful = false;
                    return;
                }

                await _currencyService.UpdateETSCurrenciesAsync(etsCurrencies);

                _logger.LogInformation($"{nameof(ETSCurrencyUpdateJob)} completed successfully. Fetched {etsCurrencies.Count} currencies.");
                wasSuccessful = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in {nameof(ETSCurrencyUpdateJob)}");
                wasSuccessful = false;

                throw new InternalServerException(string.Format(
                    ExceptionMessages.JobExecutionError,
                    nameof(ETSCurrencyUpdateJob)));
            }
            finally
            {
                try
                {
                    await _etsJobSchedulerService.ScheduleNextJobExecutionAsync(wasSuccessful);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error scheduling next job execution");
                }
            }
        }
    }
}
