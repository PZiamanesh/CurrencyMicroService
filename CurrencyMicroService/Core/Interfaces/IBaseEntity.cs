namespace CurrencyMicroService.Core.Interfaces
{
    public interface IBaseEntity
    {
        int Id { get; set; }
        DateTime CreatedDate { get; set; }
    }
}
