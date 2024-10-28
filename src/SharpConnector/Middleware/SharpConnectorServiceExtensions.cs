// (c) 2020 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SharpConnector.Interfaces;

namespace SharpConnector.Middleware
{
    public static class SharpConnectorServiceExtensions
    {
        /// <summary>
        /// Registers the SharpConnector services in the dependency injection container.
        /// This allows for a generic SharpConnectorClient to be used with any specified type T.
        /// </summary>
        /// <typeparam name="T">The type parameter that specifies the type the client will handle.</typeparam>
        /// <param name="services">The service collection to which the SharpConnector services will be added.</param>
        /// <returns>The updated service collection with the SharpConnector services registered.</returns>
        public static IServiceCollection AddSharpConnectorServices<T>(this IServiceCollection services)
        {
            services.AddControllers();

            services.AddSingleton<ISharpConnectorClient<T>>(sp =>
            {
                var configuration = sp.GetRequiredService<IConfiguration>();
                return new SharpConnectorClient<T>(configuration);
            });

            return services;
        }
    }
}
