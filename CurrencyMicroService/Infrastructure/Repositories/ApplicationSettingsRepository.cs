using CurrencyMicroService.Core.Entities;
using CurrencyMicroService.Core.Exceptions;
using CurrencyMicroService.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CurrencyMicroService.Infrastructure.Repositories
{
    public class ApplicationSettingsRepository : IApplicationSettingsRepository
    {
        private readonly CurrencySyncContext _context;

        public ApplicationSettingsRepository(
            CurrencySyncContext context
            )
        {
            _context = context;
        }

        public async Task<List<ApplicationSettings>> GetAllSettingsAsync()
        {
            try
            {
                return await _context.ApplicationSettings.ToListAsync();
            }
            catch
            {
                throw new InternalServerException("Failed to retrieve application settings from database");
            }
        }

        public async Task<ApplicationSettings?> GetSettingByKeyAsync(string settingKey)
        {
            try
            {
                return await _context.ApplicationSettings
                    .FirstOrDefaultAsync(s => s.SettingKey == settingKey);
            }
            catch
            {
                throw new InternalServerException($"Failed to retrieve application setting with key '{settingKey}'");
            }
        }
    }
}
