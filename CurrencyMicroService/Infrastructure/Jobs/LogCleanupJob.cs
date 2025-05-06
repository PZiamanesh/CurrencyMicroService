using CurrencyMicroService.Core.Exceptions;
using CurrencyMicroService.Core.Exceptions.MessageTemplates;
using Microsoft.Data.SqlClient;
using System.Data;

namespace CurrencyMicroService.Infrastructure.Jobs
{
    public class LogCleanupJob
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<LogCleanupJob> _logger;

        public LogCleanupJob(
            IConfiguration configuration,
            ILogger<LogCleanupJob> logger
            )
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task ExecuteAsync()
        {
            try
            {
                _logger.LogInformation($"Starting {nameof(LogCleanupJob)}");

                string connectionString = _configuration.GetConnectionString("CurrencyDbConnection")!;
                const string tableName = "Logs";

                await CleanupOldLogsAsync(connectionString, tableName);

                _logger.LogInformation($"{nameof(LogCleanupJob)} completed successfully");
            }
            catch
            {
                throw new InternalServerException(string.Format(
                    ExceptionMessages.JobExecutionError,
                    nameof(LogCleanupJob)));
            }
        }

        private async Task CleanupOldLogsAsync(string connectionString, string tableName)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                DateTime firstDayOfCurrentMonth = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);

                string sql = $"DELETE FROM {tableName} WHERE TimeStamp < @FirstDayOfCurrentMonth";

                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.Add("@FirstDayOfCurrentMonth", SqlDbType.DateTime).Value = firstDayOfCurrentMonth;

                    int rowsAffected = await command.ExecuteNonQueryAsync();
                    _logger.LogInformation($"Deleted {rowsAffected} log records older than {firstDayOfCurrentMonth:yyyy-MM-dd}");
                }
            }
        }
    }
}
