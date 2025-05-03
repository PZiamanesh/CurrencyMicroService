namespace CurrencyMicroService.Core.Exceptions
{
    public class ApplicationSettingKeyNotFoundException : Exception
    {
        public ApplicationSettingKeyNotFoundException(string settingKeyName) 
            : base($"No setting key found for {settingKeyName}")
        {
        }
    }
}
