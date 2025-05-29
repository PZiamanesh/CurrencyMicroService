using CurrencyMicroService.Core.Entities;

namespace CurrencyMicroService.Core.Interfaces
{
    public interface IETSCurrencyRepository
    {
        Task<List<ETSCurrency>> GetLatestETSCurrenciesAsync();

        Task<ETSCurrency?> GetLatestETSCurrencyByCodeAsync(string code);

        Task<ETSCurrency?> GetETSCurrencyByCodeAndDateAsync(string code, DateTime date);

        Task<List<ETSCurrency>> GetETSCurrenciesByDateAsync(DateTime date);

        void AddETSCurrency(ETSCurrency etsCurrency);

        void UpdateETSCurrency(ETSCurrency etsCurrency);

        Task SaveChangesAsync();
    }
}
