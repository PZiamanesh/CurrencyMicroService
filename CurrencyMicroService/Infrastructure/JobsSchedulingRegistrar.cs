using CurrencyMicroService.Core.Exceptions;
using CurrencyMicroService.Core.Exceptions.MessageTemplates;
using CurrencyMicroService.Infrastructure.Jobs;
using CurrencyMicroService.SharedModule;
using Hangfire;

namespace CurrencyMicroService.Infrastructure
{
    /* Cron Expression
        * * * * *
        │ │ │ │ │
        │ │ │ │ └─── day of week (0 - 6) (Sunday=0)
        │ │ │ └───── month (1 - 12)
        │ │ └─────── day of month (1 - 31)
        │ └───────── hour (0 - 23)
        └─────────── minute (0 - 59)
        
        Ex: "30 1,2,16 * * *"
        In this example (1, 2, 16) represents hours part and 30 is the minutes part (30 [1,2,16] * * *).
        If local time is considred, then it means everyday trigger at 1:30 AM, 2:30 AM and 4:30 PM .

        For better flexability, configure local/utc format when scheduling a job in JobsSchedulingRegistrar.
        Local format is the default for all scheduling jobs. p.zia
     */

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
                throw new InternalServerException(string.Format(
                    ExceptionMessages.JobRegistrationError, 
                    "Hangfire jobs"));
            }
        }

        private async Task RegisterETSCurrencyUpdateJobAsync()
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

                BackgroundJob.Enqueue<ETSCurrencyUpdateJob>(job => job.ExecuteAsync());

                _logger.LogInformation($"{nameof(ETSCurrencyUpdateJob)} scheduled successfully");
            }
            catch
            {
                throw new InternalServerException(string.Format(
                    ExceptionMessages.JobRegistrationError,
                    nameof(ETSCurrencyUpdateJob)));
            }
        }

        private async Task RegisterLogCleanupJobAsync()
        {
            try
            {
                string cronExpression = await _settingsProvider.GetSettingAsync(nameof(ApplicationSettingKey.LogsTableMonthlyCleanUp));

                RecurringJob.AddOrUpdate<LogCleanupJob>(
                    recurringJobId: nameof(LogCleanupJob),
                    methodCall: job => job.ExecuteAsync(),
                    cronExpression: cronExpression,
                    options: new RecurringJobOptions()
                    {
                        TimeZone = TimeZoneInfo.Local
                    });

                _logger.LogInformation($"{nameof(LogCleanupJob)} scheduled successfully");
            }
            catch
            {
                throw new InternalServerException(string.Format(
                    ExceptionMessages.JobRegistrationError,
                    nameof(LogCleanupJob)));
            }
        }
    }
}