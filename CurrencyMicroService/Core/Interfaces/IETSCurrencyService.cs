using CurrencyMicroService.Core.DTOs;
using CurrencyMicroService.Core.Entities;

namespace CurrencyMicroService.Core.Interfaces
{
    public interface IETSCurrencyService
    {
        Task<List<ETSCurrencyResult>> GetLatestETSCurrenciesAsync();

        Task<ETSCurrencyResult?> GetLatestETSCurrencyByCodeAsync(string code);

        Task<List<ETSCurrencyResult>> GetETSCurrenciesByDateAsync(DateTime date);

        Task<ETSCurrencyResult?> GetETSCurrencyByCodeAndDateAsync(string code, DateTime date);

        Task UpdateETSCurrenciesAsync(List<ETSCurrency> etsCurrencies);
    }
}
