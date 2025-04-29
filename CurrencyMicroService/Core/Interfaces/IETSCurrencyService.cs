using CurrencyMicroService.Core.DTOs;
using CurrencyMicroService.Core.Entities;

namespace CurrencyMicroService.Core.Interfaces
{
    public interface IETSCurrencyService
    {
        Task<List<ETSCurrencyResult>> GetLatestETSCurrenciesAsync();

        Task<ETSCurrencyResult?> GetLatestETSCurrencyByCodeAsync(string code);

        Task UpdateETSCurrenciesAsync(List<ETSCurrency> etsCurrencies);
    }
}
