using CurrencyMicroService.Core.Entities;

namespace CurrencyMicroService.Core.Interfaces
{
    public interface IETSCurrencyRepository
    {
        Task<List<ETSCurrency>> GetLatestETSCurrenciesAsync();

        Task<ETSCurrency?> GetLatestETSCurrencyByCodeAsync(string code);

        void AddETSCurrency(ETSCurrency etsCurrency);

        void UpdateETSCurrency(ETSCurrency etsCurrency);

        Task SaveChangesAsync();
    }
}
