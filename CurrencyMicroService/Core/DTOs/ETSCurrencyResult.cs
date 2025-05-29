namespace CurrencyMicroService.Core.DTOs
{
    public class ETSCurrencyResult
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public decimal CashBuy { get; set; }
        public decimal CashSell { get; set; }
        public decimal TransferBuy { get; set; }
        public decimal TransferSell { get; set; }
        public decimal EssentialGoodsBuy { get; set; }
        public decimal EssentialGoodsSell { get; set; }
        public decimal WeightedAverage { get; set; }
        public DateTime FetchDate { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
