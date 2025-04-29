using CurrencyMicroService.Core.DTOs;
using CurrencyMicroService.Core.Entities;
using CurrencyMicroService.Core.Exceptions;
using CurrencyMicroService.Core.Interfaces;

namespace CurrencyMicroService.Core.Services
{
    public class ETSCurrencyService : IETSCurrencyService
    {
        private readonly IETSCurrencyRepository _repository;

        public ETSCurrencyService(
            IETSCurrencyRepository repository
            )
        {
            _repository = repository;
        }

        public async Task<List<ETSCurrencyResult>> GetLatestETSCurrenciesAsync()
        {
            var currencies = await _repository.GetLatestETSCurrenciesAsync();
            return currencies.Select(ToCurrencyInfoResult).ToList();
        }

        public async Task<ETSCurrencyResult?> GetLatestETSCurrencyByCodeAsync(string code)
        {
            var currency = await _repository.GetLatestETSCurrencyByCodeAsync(code);
            return currency != null ? ToCurrencyInfoResult(currency) : null;
        }

        public async Task UpdateETSCurrenciesAsync(List<ETSCurrency> etsCurrencies)
        {
            try
            {
                if (etsCurrencies == null || !etsCurrencies.Any())
                {
                    return;
                }

                var fetchDate = etsCurrencies.First().FetchDate.Date;

                foreach (var etsCurrency in etsCurrencies)
                {
                    var existingCurrency = await _repository.GetLatestETSCurrencyByCodeAsync(etsCurrency.Code);

                    if (existingCurrency != null)
                    {
                        existingCurrency.Name = etsCurrency.Name;
                        existingCurrency.CashBuy = etsCurrency.CashBuy;
                        existingCurrency.CashSell = etsCurrency.CashSell;
                        existingCurrency.TransferBuy = etsCurrency.TransferBuy;
                        existingCurrency.TransferSell = etsCurrency.TransferSell;
                        existingCurrency.EssentialGoodsBuy = etsCurrency.EssentialGoodsBuy;
                        existingCurrency.EssentialGoodsSell = etsCurrency.EssentialGoodsSell;
                        existingCurrency.WeightedAverage = etsCurrency.WeightedAverage;
                        existingCurrency.FetchDate = etsCurrency.FetchDate;

                        _repository.UpdateETSCurrency(existingCurrency);
                    }
                    else
                    {
                        _repository.AddETSCurrency(etsCurrency);
                    }
                }

                await _repository.SaveChangesAsync();
            }
            catch
            {
                throw new InternalServerException("Error updating ETS currency information");
            }
        }

        private ETSCurrencyResult ToCurrencyInfoResult(ETSCurrency etsCurrency)
        {
            return new ETSCurrencyResult
            {
                Name = etsCurrency.Name,
                Code = etsCurrency.Code,
                CashBuy = etsCurrency.CashBuy,
                CashSell = etsCurrency.CashSell,
                TransferBuy = etsCurrency.TransferBuy,
                TransferSell = etsCurrency.TransferSell,
                EssentialGoodsBuy = etsCurrency.EssentialGoodsBuy,
                EssentialGoodsSell = etsCurrency.EssentialGoodsSell,
                WeightedAverage = etsCurrency.WeightedAverage,
                FetchDate = etsCurrency.FetchDate
            };
        }
    }
}
