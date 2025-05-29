using CurrencyMicroService.Core.Interfaces;

namespace CurrencyMicroService.Core.Entities
{
    public class ETSCurrency : IBaseEntity
    {
        public int Id { get; set; }

        public string Name { get; set; } // نوع ارز

        public string Code { get; set; } // کد ارز

        public decimal CashBuy { get; set; } // خرید(اسکناس)

        public decimal CashSell { get; set; } // فروش(اسکناس)

        public decimal TransferBuy { get; set; } // خرید(حواله)

        public decimal TransferSell { get; set; } // فروش(حواله)

        public decimal EssentialGoodsBuy { get; set; } // خرید حواله کالاهای اساسی و ضروری

        public decimal EssentialGoodsSell { get; set; } // فروش حواله کالاهای اساسی و ضروری

        public decimal WeightedAverage { get; set; } // میانگین موزون

        public DateTime FetchDate { get; set; } // آخرین تاریخ دریافت از بانک مرکزی

        public DateTime CreatedDate { get; set; }
    }
}
