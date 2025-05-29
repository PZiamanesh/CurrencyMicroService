using CurrencyMicroService.Core.Entities;
using CurrencyMicroService.Core.Interfaces;
using CurrencyMicroService.Infrastructure.Jobs;
using CurrencyMicroService.SharedModule;
using Hangfire;
using Hangfire.Storage;

namespace CurrencyMicroService.Infrastructure.JobServices
{
    public interface IETSCurrencyJobSchedulerService
    {
        Task ScheduleNextJobExecutionAsync(bool wasSuccessful);
        Task<bool> HasDataForTodayAsync();
        Task InitializeJobSchedulingAsync();
        Task SetupRecurringJobAsync();
    }

    public class ETSCurrencyJobSchedulerService : IETSCurrencyJobSchedulerService
    {
        private readonly ApplicationSettingsProvider _settingsProvider;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ETSCurrencyJobSchedulerService> _logger;

        public ETSCurrencyJobSchedulerService(
            ApplicationSettingsProvider settingsProvider,
            IServiceProvider serviceProvider,
            ILogger<ETSCurrencyJobSchedulerService> logger)
        {
            _settingsProvider = settingsProvider;
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task InitializeJobSchedulingAsync()
        {
            try
            {
                bool hasDataForToday = await HasDataForTodayAsync();

                if (hasDataForToday)
                {
                    _logger.LogInformation($"{nameof(ETSCurrency)} data already exists for today. Scheduling job for tomorrow.");
                    await ScheduleJobForTomorrow();
                }
                else
                {
                    _logger.LogInformation($"No {nameof(ETSCurrency)} data for today. Setting up recurring job.");
                    await SetupRecurringJobAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error initializing job scheduling");
                await SetupRecurringJobAsync();
            }
        }

        public async Task SetupRecurringJobAsync()
        {
            try
            {
                string cronExpression = await _settingsProvider.GetSettingAsync(nameof(ApplicationSettingKey.ETSCurrencyDailyJobStartTime));

                RecurringJob.AddOrUpdate<ETSCurrencyUpdateJob>(
                    recurringJobId: nameof(ETSCurrencyUpdateJob),
                    methodCall: job => job.ExecuteAsync(),
                    cronExpression: cronExpression,
                    options: new RecurringJobOptions()
                    {
                        TimeZone = TimeZoneInfo.Local
                    });

                _logger.LogInformation($"{nameof(ETSCurrencyUpdateJob)} setup completed with cron: {cronExpression}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting up recurring job");
                throw;
            }
        }

        public async Task<bool> HasDataForTodayAsync()
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var currencyService = scope.ServiceProvider.GetRequiredService<IETSCurrencyService>();

                var today = DateTime.UtcNow.Date;
                var todaysCurrencies = await currencyService.GetETSCurrenciesByDateAsync(today);

                bool hasData = todaysCurrencies != null && todaysCurrencies.Any();

                _logger.LogInformation($"{nameof(ETSCurrency)} data exists for today ({today:yyyy-MM-dd}): {hasData}");

                return hasData;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error checking if {nameof(ETSCurrency)} data exists for today");
                return false;
            }
        }

        public async Task ScheduleNextJobExecutionAsync(bool wasSuccessful)
        {
            if (wasSuccessful)
            {
                _logger.LogInformation($"{nameof(ETSCurrency)} update was successful. Scheduling job for tomorrow.");
                await ScheduleJobForTomorrow();
            }
            else
            {
                _logger.LogInformation($"{nameof(ETSCurrency)} update failed. Checking if recurring job exists...");

                bool recurringJobExists = DoesRecurringJobExist();

                if (!recurringJobExists)
                {
                    _logger.LogWarning($"No recurring job '{nameof(ETSCurrencyUpdateJob)}' found after failure. Recreating recurring job for retries.");
                    await SetupRecurringJobAsync();
                }
                else
                {
                    _logger.LogInformation($"Recurring job '{nameof(ETSCurrencyUpdateJob)}' exists. Will retry according to schedule.");
                }
            }
        }

        private async Task ScheduleJobForTomorrow()
        {
            try
            {
                RecurringJob.RemoveIfExists(nameof(ETSCurrencyUpdateJob));

                var tomorrowMidnight = DateTime.UtcNow.Date.AddDays(1);

                BackgroundJob.Schedule<ETSCurrencyUpdateJob>(
                    job => job.ExecuteAsync(),
                    tomorrowMidnight);

                _logger.LogInformation($"Job '{nameof(ETSCurrencyUpdateJob)}' scheduled for tomorrow at {tomorrowMidnight:yyyy-MM-dd HH:mm:ss} UTC");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error scheduling job '{nameof(ETSCurrencyUpdateJob)}' for tomorrow, falling back to recurring job");
                await SetupRecurringJobAsync();
            }
        }

        private bool DoesRecurringJobExist()
        {
            try
            {
                using var connection = JobStorage.Current.GetConnection();
                var recurringJobs = connection.GetRecurringJobs();

                bool exists = recurringJobs.Any(job => job.Id == nameof(ETSCurrencyUpdateJob));

                _logger.LogInformation($"Recurring job '{nameof(ETSCurrencyUpdateJob)}' exists: {exists}");

                return exists;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error checking if recurring job '{nameof(ETSCurrencyUpdateJob)}' exists, assuming it doesn't");
                return false;
            }
        }
    }
}
