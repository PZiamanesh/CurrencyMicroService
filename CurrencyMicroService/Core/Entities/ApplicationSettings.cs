namespace CurrencyMicroService.Core.Entities
{
    public class ApplicationSettings
    {
        public int Id { get; set; }

        public string SettingKey { get; set; }

        public string SettingValue { get; set; }

        public string Description { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
