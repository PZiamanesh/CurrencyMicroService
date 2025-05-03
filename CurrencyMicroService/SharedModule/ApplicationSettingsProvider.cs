using CurrencyMicroService.Core.Exceptions;
using CurrencyMicroService.Core.Interfaces;

namespace CurrencyMicroService.SharedModule
{
    public class ApplicationSettingsProvider
    {
        private Dictionary<string, string> _settings;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ApplicationSettingsProvider> _logger;
        private bool _isInitialized = false;
        private readonly SemaphoreSlim _initLock = new SemaphoreSlim(1, 1);

        public ApplicationSettingsProvider(
            IServiceProvider serviceProvider,
            ILogger<ApplicationSettingsProvider> logger
            )
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _settings = new Dictionary<string, string>();
        }

        public async Task InitializeAsync()
        {
            if (_isInitialized)
                return;

            await _initLock.WaitAsync();
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var settingsService = scope.ServiceProvider.GetRequiredService<IApplicationSettingsService>();

                _settings = await settingsService.GetAllSettingsAsDictionaryAsync();
                _isInitialized = true;

                _logger.LogInformation("Application settings initialized successfully");
            }
            catch
            {
                throw new InternalServerException("Error initializing application settings");
            }
            finally
            {
                _initLock.Release();
            }
        }

        public async Task<string> GetSettingAsync(string settingKey)
        {
            if (!_isInitialized)
            {
                await InitializeAsync();
            }

            if (_settings.TryGetValue(settingKey, out string? value))
            {
                return value;
            }
            else
            {
                _logger.LogError($"No setting key found for {settingKey}");
                throw new ApplicationSettingKeyNotFoundException(settingKey);
            }
        }

    }
}
