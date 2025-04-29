using CurrencyMicroService.Core.Entities;
using CurrencyMicroService.Core.Exceptions;
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
                    return new List<ETSCurrency>();
                }

                return await _context.ETSCurrencies
                    .Where(c => c.FetchDate == latestDate)
                    .OrderBy(c => c.Code)
                    .ToListAsync();
            }
            catch
            {
                throw new InternalServerException("Failed to retrieve latest ETS currency data from database");
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
                throw new InternalServerException($"Failed to retrieve ETS currency data for code {code}");
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
                throw new InternalServerException($"Failed to add ETS currency data for code {currencyInfo.Code}");
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
                throw new InternalServerException($"Failed to update ETS currency data for code {currencyInfo.Code}");
            }
        }

        public async Task SaveChangesAsync()
        {
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InternalServerException("A concurrency conflict occurred while saving changes");
            }
            catch (DbUpdateException)
            {
                throw new InternalServerException("Failed to save changes to the database");
            }
            catch
            {
                throw new InternalServerException("An unexpected error occurred while saving changes to the database");
            }
        }
    }
}
