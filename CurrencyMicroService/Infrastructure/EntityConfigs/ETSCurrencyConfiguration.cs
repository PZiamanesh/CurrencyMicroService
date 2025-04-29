using CurrencyMicroService.Core.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace CurrencyMicroService.Infrastructure.EntityConfigs
{
    public class ETSCurrencyConfiguration : IEntityTypeConfiguration<ETSCurrency>
    {
        public void Configure(EntityTypeBuilder<ETSCurrency> builder)
        {
            builder.ToTable("ETSCurrencies");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(c => c.Code)
                .IsRequired()
                .HasMaxLength(10);

            builder.Property(c => c.CashBuy)
                .HasColumnType("decimal(18,2)");

            builder.Property(c => c.CashSell)
                .HasColumnType("decimal(18,2)");

            builder.Property(c => c.TransferBuy)
                .HasColumnType("decimal(18,2)");

            builder.Property(c => c.TransferSell)
                .HasColumnType("decimal(18,2)");

            builder.Property(c => c.EssentialGoodsBuy)
                .HasColumnType("decimal(18,2)");

            builder.Property(c => c.EssentialGoodsSell)
                .HasColumnType("decimal(18,2)");

            builder.Property(c => c.WeightedAverage)
                .HasColumnType("decimal(18,2)");

            builder.Property(c => c.FetchDate)
                .IsRequired();
        }
    }
}
