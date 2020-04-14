using System;
using Consul;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Discovery
{
    public static class DiscoveryExtension
    {
        /// <summary>
        /// 添加服务发现
        /// </summary>
        /// <param name="services">DI服务</param>
        /// <param name="option">服务发现选项</param>
        /// <returns>DI服务</returns>
        public static IServiceCollection AddDiscovery(this IServiceCollection services, Action<DiscoveryOptions> option)
        {
            services.AddOptions();
            services.Configure(option);
            services.TryAddSingleton<IConsulClient>(serviceProvider =>
            {
                var discoveryOption = serviceProvider.GetRequiredService<IOptionsMonitor<DiscoveryOptions>>().CurrentValue;
                return new ConsulClient(configOverride =>
                {
                    configOverride.Token = discoveryOption.Token;
                    configOverride.WaitTime = discoveryOption.WaitTime;
                    configOverride.Datacenter = discoveryOption.DataCenter;
                    configOverride.Address = new Uri(discoveryOption.Address);
                });
            });
            return services;
        }
    }
}