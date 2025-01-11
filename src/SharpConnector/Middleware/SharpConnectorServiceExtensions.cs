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
        /// Registers the ISharpConnectorClient<T> as a scoped service in the dependency injection container.
        /// This means that a new instance of ISharpConnectorClient<T> will be created for each HTTP request.
        /// Typically used for services that are request-specific and need to be disposed at the end of each request.
        /// </summary>
        /// <typeparam name="T">The type of the payload used by the SharpConnectorClient.</typeparam>
        /// <param name="services">The IServiceCollection where the service will be registered.</param>
        /// <returns>The IServiceCollection with the added service.</returns>
        public static IServiceCollection AddSharpConnectorServicesScoped<T>(this IServiceCollection services)
        {
            // Adds MVC controllers to the service collection, necessary for handling HTTP requests.
            services.AddControllers();

            // Registers the ISharpConnectorClient<T> as Scoped, meaning it will have a different instance for each HTTP request.
            services.AddScoped<ISharpConnectorClient<T>>(sp =>
            {
                // Retrieves the configuration service from the dependency injection container.
                var configuration = sp.GetRequiredService<IConfiguration>();

                // Creates and returns a new instance of SharpConnectorClient<T> using the retrieved configuration.
                return new SharpConnectorClient<T>(configuration);
            });

            return services;
        }

        /// <summary>
        /// Registers the ISharpConnectorClient<T> as a singleton service in the dependency injection container.
        /// This means that only a single instance of ISharpConnectorClient<T> will be used throughout the entire application's lifetime.
        /// Typically used for services that do not require multiple instances and can share state across the entire application.
        /// </summary>
        /// <typeparam name="T">The type of the payload used by the SharpConnectorClient.</typeparam>
        /// <param name="services">The IServiceCollection where the service will be registered.</param>
        /// <returns>The IServiceCollection with the added service.</returns>
        public static IServiceCollection AddSharpConnectorServicesSingleton<T>(this IServiceCollection services)
        {
            // Adds MVC controllers to the service collection, necessary for handling HTTP requests.
            services.AddControllers();

            // Registers the ISharpConnectorClient<T> as Singleton, meaning only one instance will exist for the entire application.
            services.AddSingleton<ISharpConnectorClient<T>>(sp =>
            {
                // Retrieves the configuration service from the dependency injection container.
                var configuration = sp.GetRequiredService<IConfiguration>();

                // Creates and returns a new instance of SharpConnectorClient<T> using the retrieved configuration.
                return new SharpConnectorClient<T>(configuration);
            });

            return services;
        }

        /// <summary>
        /// A default registration method for ISharpConnectorClient<T> that registers it as a Singleton.
        /// This is provided for backward compatibility with existing codebases that may have previously registered services as Singletons.
        /// Developers can still choose to change the lifetime to Scoped by using the AddSharpConnectorServicesScoped method.
        /// </summary>
        /// <typeparam name="T">The type of the payload used by the SharpConnectorClient.</typeparam>
        /// <param name="services">The IServiceCollection where the service will be registered.</param>
        /// <returns>The IServiceCollection with the added service.</returns>
        public static IServiceCollection AddSharpConnectorServices<T>(this IServiceCollection services)
        {
            // The default behavior is to register the service as a Singleton.
            return services.AddSharpConnectorServicesSingleton<T>();
        }
    }
}
