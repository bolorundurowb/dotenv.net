using Microsoft.Extensions.DependencyInjection;

namespace dotenv.net.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddEnv(this IServiceCollection services)
        {
            return services;
        }
    }
}