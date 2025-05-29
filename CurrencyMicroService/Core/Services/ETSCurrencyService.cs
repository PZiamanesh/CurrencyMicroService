using CurrencyMicroService.Core.DTOs;
using CurrencyMicroService.Core.Entities;
using CurrencyMicroService.Core.Exceptions;
using CurrencyMicroService.Core.Exceptions.MessageTemplates;
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

        public async Task<List<ETSCurrencyResult>> GetETSCurrenciesByDateAsync(DateTime date)
        {
            var currencies = await _repository.GetETSCurrenciesByDateAsync(date);
            return currencies.Select(ToCurrencyInfoResult).ToList();
        }

        public async Task<ETSCurrencyResult?> GetETSCurrencyByCodeAndDateAsync(string code, DateTime date)
        {
            var currency = await _repository.GetETSCurrencyByCodeAndDateAsync(code, date);
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

                var today = DateTime.UtcNow.Date;

                foreach (var etsCurrency in etsCurrencies)
                {
                    var existingCurrencyForToday = await _repository.GetETSCurrencyByCodeAndDateAsync(etsCurrency.Code, today);

                    if (existingCurrencyForToday != null)
                    {
                        existingCurrencyForToday.Name = etsCurrency.Name;
                        existingCurrencyForToday.CashBuy = etsCurrency.CashBuy;
                        existingCurrencyForToday.CashSell = etsCurrency.CashSell;
                        existingCurrencyForToday.TransferBuy = etsCurrency.TransferBuy;
                        existingCurrencyForToday.TransferSell = etsCurrency.TransferSell;
                        existingCurrencyForToday.EssentialGoodsBuy = etsCurrency.EssentialGoodsBuy;
                        existingCurrencyForToday.EssentialGoodsSell = etsCurrency.EssentialGoodsSell;
                        existingCurrencyForToday.WeightedAverage = etsCurrency.WeightedAverage;
                        existingCurrencyForToday.FetchDate = etsCurrency.FetchDate;

                        _repository.UpdateETSCurrency(existingCurrencyForToday);
                    }
                    else
                    {
                        etsCurrency.CreatedDate = today;
                        _repository.AddETSCurrency(etsCurrency);
                    }
                }

                await _repository.SaveChangesAsync();
            }
            catch
            {
                throw new InternalServerException(string.Format(
                    ExceptionMessages.DatabaseUpdateError,
                    nameof(ETSCurrency)));
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
                FetchDate = etsCurrency.FetchDate,
                CreatedDate = etsCurrency.CreatedDate
            };
        }
    }
}
