using CurrencyMicroService.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace CurrencyMicroService.Infrastructure
{
    public class CurrencySyncContext : DbContext
    {
        public DbSet<ETSCurrency> ETSCurrencies => Set<ETSCurrency>();
        public DbSet<ApplicationSettings> ApplicationSettings => Set<ApplicationSettings>();

        public CurrencySyncContext(DbContextOptions<CurrencySyncContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(CurrencySyncContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }
    }
}
