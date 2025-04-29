using CurrencyMicroService.Core.Entities;

namespace CurrencyMicroService.Core.Interfaces
{
    public interface IApplicationSettingsRepository
    {
        Task<List<ApplicationSettings>> GetAllSettingsAsync();

        Task<ApplicationSettings?> GetSettingByKeyAsync(string settingKey);
    }
}
