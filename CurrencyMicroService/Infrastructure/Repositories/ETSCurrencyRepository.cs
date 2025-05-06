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
                var latestDate = await _context.ETSCurrencies
                    .OrderByDescending(c => c.FetchDate)
                    .Select(c => c.FetchDate)
                    .FirstOrDefaultAsync();

                if (latestDate == default)
                {
                    return [];
                }

                return await _context.ETSCurrencies
                    .Where(c => c.FetchDate == latestDate)
                    .OrderBy(c => c.Code)
                    .ToListAsync();
            }
            catch
            {
                throw new InternalServerException(string.Format(
                    ExceptionMessages.DatabaseRetrieveLatestErrorFor,
                    nameof(ETSCurrency)));
            }
        }

        public async Task<ETSCurrency?> GetLatestETSCurrencyByCodeAsync(string code)
        {
            try
            {
                return await _context.ETSCurrencies
                    .Where(c => c.Code == code)
                    .OrderByDescending(c => c.FetchDate)
                    .FirstOrDefaultAsync();
            }
            catch
            {
                throw new InternalServerException(string.Format(
                    ExceptionMessages.DatabaseRetrieveByFilterErrorFor,
                    nameof(ETSCurrency),
                    code));
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
                    ExceptionMessages.DatabaseAddErrorFor,
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
                    ExceptionMessages.DatabaseUpdateErrorFor,
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
