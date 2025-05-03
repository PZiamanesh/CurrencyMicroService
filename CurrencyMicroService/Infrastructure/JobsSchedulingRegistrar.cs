using CurrencyMicroService.Core.Exceptions;
using CurrencyMicroService.Infrastructure.Jobs;
using CurrencyMicroService.SharedModule;
using Hangfire;

namespace CurrencyMicroService.Infrastructure
{
    public class JobsSchedulingRegistrar
    {
        private readonly ApplicationSettingsProvider _settingsProvider;
        private readonly ILogger<JobsSchedulingRegistrar> _logger;

        public JobsSchedulingRegistrar(
            ApplicationSettingsProvider settingsProvider,
            ILogger<JobsSchedulingRegistrar> logger
            )
        {
            _settingsProvider = settingsProvider;
            _logger = logger;
        }

        public async Task RegisterJobsAsync()
        {
            try
            {
                await RegisterETSCurrencyUpdateJobAsync();
                await RegisterLogCleanupJobAsync();
            }
            catch
            {
                throw new InternalServerException("Error registering Hangfire jobs");
            }
        }

        private async Task RegisterETSCurrencyUpdateJobAsync()
        {
            try
            {
                string timeExpression = await _settingsProvider.GetSettingAsync(nameof(ApplicationSettingKey.ETSCurrencyDailyJobStartTime));
                string[] timeParts = timeExpression.Trim().Split('-');

                int hour = int.Parse(timeParts[0]);
                int minute = int.Parse(timeParts[1]);

                RecurringJob.AddOrUpdate<ETSCurrencyUpdateJob>(
                    "daily-ets-currency-update",
                    job => job.ExecuteAsync(),
                    Cron.Daily(hour, minute)
                    );

                _logger.LogInformation($"{nameof(ETSCurrencyUpdateJob)} scheduled successfully");
            }
            catch
            {
                throw new InternalServerException($"Error registering {nameof(ETSCurrencyUpdateJob)}");
            }
        }

        private async Task RegisterLogCleanupJobAsync()
        {
            try
            {
                string dateTimeExpression = await _settingsProvider.GetSettingAsync(nameof(ApplicationSettingKey.LogsTableMonthlyCleanUp));
                string[] timeParts = dateTimeExpression.Trim().Split('-');

                int day = int.Parse(timeParts[0]);
                int hour = int.Parse(timeParts[1]);
                int minute = int.Parse(timeParts[2]);

                RecurringJob.AddOrUpdate<LogCleanupJob>(
                    "monthly-log-cleanup",
                    job => job.ExecuteAsync(),
                    Cron.Monthly(day, hour, minute)
                    );

                _logger.LogInformation($"{nameof(LogCleanupJob)} scheduled successfully");
            }
            catch
            {
                throw new InternalServerException($"Error registering {nameof(LogCleanupJob)}");
            }
        }
    }
}