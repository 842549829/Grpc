using System;
using System.Collections.Generic;
using System.Text;
using Discovery.Client;
using Microsoft.Extensions.DependencyInjection;

namespace Discovery
{
    public static class ClientDiscoveryExtension
    {
        public static IServiceCollection AddClientDiscovery(this IServiceCollection services)
        {
            services.AddClientDiscovery(option => { });
            return services;
        }

        public static IServiceCollection AddClientDiscovery(this IServiceCollection services, Action<DiscoveryOptions> option)
        {
            services.AddDiscovery(option);
            services.AddSingleton<IClientRouteManager, ConsulClientRouteManager>();
            services.AddSingleton<IAddressSelector, PollingAddressSelector>();
            return services;
        }
    }
}