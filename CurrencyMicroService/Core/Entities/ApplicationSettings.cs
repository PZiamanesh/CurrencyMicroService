using CurrencyMicroService.Core.Interfaces;

namespace CurrencyMicroService.Core.Entities
{
    public class ApplicationSettings : IBaseEntity
    {
        public int Id { get; set; }

        public string SettingKey { get; set; }

        public string SettingValue { get; set; }

        public string Description { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
