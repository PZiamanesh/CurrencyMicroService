using CurrencyMicroService.Core.Entities;
using CurrencyMicroService.Core.Exceptions;
using CurrencyMicroService.Core.Exceptions.MessageTemplates;
using CurrencyMicroService.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CurrencyMicroService.Infrastructure.Repositories
{
    public class ETSCurrencyRepository : IETSCurrencyRepository
    {
        private readonly CurrencySyncContext _context;

        public ETSCurrencyRepository(
            CurrencySyncContext context
            )
        {
            _context = context;
        }

        public async Task<List<ETSCurrency>> GetLatestETSCurrenciesAsync()
        {
            try
            {
                var latestCreationDate = await _context.ETSCurrencies
                    .OrderByDescending(c => c.CreatedDate)
                    .Select(c => c.CreatedDate)
                    .FirstOrDefaultAsync();

                if (latestCreationDate == default)
                {
                    return [];
                }

                return await _context.ETSCurrencies
                    .Where(c => c.FetchDate == latestCreationDate)
                    .OrderBy(c => c.Code)
                    .ToListAsync();
            }
            catch
            {
                throw new InternalServerException(string.Format(
                    ExceptionMessages.DatabaseRetrieveError,
                    nameof(ETSCurrency)));
            }
        }

        public async Task<ETSCurrency?> GetLatestETSCurrencyByCodeAsync(string code)
        {
            try
            {
                return await _context.ETSCurrencies
                    .Where(c => c.Code == code)
                    .OrderByDescending(c => c.CreatedDate)
                    .FirstOrDefaultAsync();
            }
            catch
            {
                throw new InternalServerException(string.Format(
                    ExceptionMessages.DatabaseRetrieveByFilterError,
                    nameof(ETSCurrency),
                    code));
            }
        }

        public async Task<ETSCurrency?> GetETSCurrencyByCodeAndDateAsync(string code, DateTime date)
        {
            try
            {
                return await _context.ETSCurrencies
                    .Where(c => c.Code == code && c.CreatedDate.Date == date.Date)
                    .FirstOrDefaultAsync();
            }
            catch
            {
                throw new InternalServerException(string.Format(
                    ExceptionMessages.DatabaseRetrieveByFilterError,
                    nameof(ETSCurrency),
                    $"{code} for date {date:yyyy-MM-dd}"));
            }
        }

        public async Task<List<ETSCurrency>> GetETSCurrenciesByDateAsync(DateTime date)
        {
            try
            {
                return await _context.ETSCurrencies
                    .Where(c => c.CreatedDate.Date == date.Date)
                    .OrderBy(c => c.Code)
                    .ToListAsync();
            }
            catch
            {
                throw new InternalServerException(string.Format(
                    ExceptionMessages.DatabaseRetrieveByFilterError,
                    nameof(ETSCurrency),
                    $"date {date:yyyy-MM-dd}"));
            }
        }

        public void AddETSCurrency(ETSCurrency currencyInfo)
        {
            try
            {
                _context.ETSCurrencies.Add(currencyInfo);
            }
            catch
            {
                throw new InternalServerException(string.Format(
                    ExceptionMessages.DatabaseAddError,
                    nameof(ETSCurrency)));
            }
        }

        public void UpdateETSCurrency(ETSCurrency currencyInfo)
        {
            try
            {
                _context.ETSCurrencies.Update(currencyInfo);
            }
            catch
            {
                throw new InternalServerException(string.Format(
                    ExceptionMessages.DatabaseUpdateError,
                    nameof(ETSCurrency)));
            }
        }

        public async Task SaveChangesAsync()
        {
            try
            {
                await _context.SaveChangesAsync();
            }
            catch
            {
                throw new InternalServerException(
                    ExceptionMessages.DatabaseSaveChangesError);
            }
        }
    }
}
