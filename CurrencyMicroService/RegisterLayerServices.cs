using CurrencyMicroService.Core.Interfaces;
using CurrencyMicroService.Core.Services;
using CurrencyMicroService.Infrastructure.HttpClientServices;
using CurrencyMicroService.Infrastructure.Jobs;
using CurrencyMicroService.Infrastructure.Repositories;

namespace CurrencyMicroService
{
    public static class RegisterLayerServices
    {
        public static IServiceCollection AddLayerServices(
            this IServiceCollection services,
            IConfiguration configuration
            )
        {
            RegisterRepositories(services);
            RegisterCoreServices(services);
            RegisterJobs(services);
            RegisterHttpClientServices(services);
            ConfigureHttpClients(services);

            return services;
        }

        private static void RegisterRepositories(IServiceCollection services)
        {
            services.AddScoped<IApplicationSettingsRepository, ApplicationSettingsRepository>();
            services.AddScoped<IETSCurrencyRepository, ETSCurrencyRepository>();
        }

        private static void RegisterCoreServices(IServiceCollection services)
        {
            services.AddScoped<IApplicationSettingsService, ApplicationSettingsService>();
            services.AddScoped<IETSCurrencyService, ETSCurrencyService>();
        }

        private static void RegisterJobs(IServiceCollection services)
        {
            services.AddScoped<ETSCurrencyUpdateJob>();
            services.AddScoped<LogCleanupJob>();
        }

        private static void RegisterHttpClientServices(IServiceCollection services)
        {
            services.AddScoped<ETSCurrencyScraperHttpClientService>();
        }

        private static void ConfigureHttpClients(IServiceCollection services)
        {
            services.AddHttpClient("ETSCurrencyScraperHttpClientService");
        }
    }
}