using CurrencyMicroService.Core.Exceptions.MessageTemplates;

namespace CurrencyMicroService.Core.Exceptions
{
    public class ApplicationSettingKeyNotFoundException : Exception
    {
        public ApplicationSettingKeyNotFoundException(string settingKey) 
            : base(string.Format(ExceptionMessages.ApplicationSettingKeyNotFoundError, settingKey))
        {
        }
    }
}
