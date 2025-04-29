using CurrencyMicroService.Core.Entities;

namespace CurrencyMicroService.Core.Interfaces
{
    public interface IApplicationSettingsService
    {
        Task<Dictionary<string, string>> GetAllSettingsAsDictionaryAsync();

        Task<string?> GetSettingValueByKeyAsync(string settingKey);

        Task<ApplicationSettings?> GetSettingByKeyAsync(string settingKey);

        Task<List<ApplicationSettings>> GetAllSettingsAsync();
    }
}
