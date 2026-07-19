using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Ecommerce.Messaging
{
    public static class ServiceCollectionExtensions
    {
        // Binds the RabbitMq config section. Needed by publishers and consumers alike.
        public static IServiceCollection AddRabbitMqOptions(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<RabbitMqOptions>(configuration.GetSection("RabbitMq"));
            return services;
        }

        // For services that publish events (Customer/Inventory/Order/Payment).
        public static IServiceCollection AddRabbitMqPublisher(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddRabbitMqOptions(configuration);
            services.AddSingleton<IEventPublisher, RabbitMqEventPublisher>();
            return services;
        }
    }
}
