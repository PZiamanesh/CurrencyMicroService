using CurrencyMicroService.Core.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace CurrencyMicroService.Infrastructure.EntityConfigs
{
    public class ApplicationSettingsConfiguration : IEntityTypeConfiguration<ApplicationSettings>
    {
        public void Configure(EntityTypeBuilder<ApplicationSettings> builder)
        {
            builder.ToTable("ApplicationSettings");

            builder.HasKey(s => s.Id);

            builder.Property(s => s.Id).ValueGeneratedNever();

            builder.HasIndex(s => s.SettingKey).IsUnique();

            builder.Property(s => s.SettingKey)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(s => s.SettingValue)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(s => s.Description)
                .IsRequired(false)
                .HasMaxLength(500);

            builder.Property(s => s.CreatedDate)
                .IsRequired();
        }
    }
}
