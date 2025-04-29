using CurrencyMicroService.Core.Entities;
using CurrencyMicroService.Core.Interfaces;

namespace CurrencyMicroService.Core.Services
{
    public class ApplicationSettingsService : IApplicationSettingsService
    {
        private readonly IApplicationSettingsRepository _repository;

        public ApplicationSettingsService(
            IApplicationSettingsRepository repository
            )
        {
            _repository = repository;
        }

        public async Task<Dictionary<string, string>> GetAllSettingsAsDictionaryAsync()
        {
            var settings = await _repository.GetAllSettingsAsync();
            return settings.ToDictionary(s => s.SettingKey, s => s.SettingValue);
        }

        public async Task<string?> GetSettingValueByKeyAsync(string settingKey)
        {
            var setting = await _repository.GetSettingByKeyAsync(settingKey);
            return setting?.SettingValue;
        }

        public async Task<ApplicationSettings?> GetSettingByKeyAsync(string settingKey)
        {
            return await _repository.GetSettingByKeyAsync(settingKey);
        }

        public async Task<List<ApplicationSettings>> GetAllSettingsAsync()
        {
            return await _repository.GetAllSettingsAsync();
        }
    }
}
