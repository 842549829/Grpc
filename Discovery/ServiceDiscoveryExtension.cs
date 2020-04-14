using Consul;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server.Features;

namespace Discovery
{
    public static class ServiceDiscoveryExtension
    {
        public static IServiceCollection AddServiceDiscovery(this IServiceCollection services)
        {
            services.AddServiceDiscovery(option => { });
            return services;
        }

        /// <summary>
        /// 添加服务发现
        /// </summary>
        /// <param name="services">DI服务</param>
        /// <param name="option">服务发现选项</param>
        /// <returns>DI服务</returns>
        public static IServiceCollection AddServiceDiscovery(this IServiceCollection services, Action<DiscoveryOptions> option)
        {
            services.AddHttpContextAccessor();
            services.AddDiscovery(option);
            return services;
        }

        /// <summary>
        /// 注册consul服务发现
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseDiscovery(this IApplicationBuilder app)
        {
            var consulClient = app.ApplicationServices.GetRequiredService<IConsulClient>();
            var discoveryOption = app.ApplicationServices.GetRequiredService<IOptionsMonitor<DiscoveryOptions>>().CurrentValue;
            var env = Environment.GetEnvironmentVariable("DISCOVERY_ADDRESS");
            var lifetime = app.ApplicationServices.GetRequiredService<IApplicationLifetime>();
            var addresses = app.ServerFeatures.Get<IServerAddressesFeature>();
            var address = addresses.Addresses.FirstOrDefault();
            var uri = env != null ? new Uri(env) : GetListenAddress(discoveryOption, address);
            var serviceCheck = new AgentServiceCheck
            {
                TCP = $"{uri.Host}:{uri.Port}",
                Interval = TimeSpan.FromSeconds(discoveryOption.HealthCheckSeconds),
                DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(discoveryOption.CriticalDeregisterSeconds)
            };
            var registration = new AgentServiceRegistration
            {
                ID = $"{uri.Authority}-{discoveryOption.ServiceName}",
                Name = discoveryOption.ServiceName,
                Tags = discoveryOption.Tags,
                Address = uri.Host,
                Port = uri.Port,
                Meta = discoveryOption.Meta,
                Check = serviceCheck
            };
            lifetime.ApplicationStarted.Register(() =>
            {
                consulClient.Agent.ServiceDeregister(registration.ID).Wait();
                consulClient.Agent.ServiceRegister(registration).Wait();
            });
            lifetime.ApplicationStopping.Register(() =>
            {
                consulClient.Agent.ServiceDeregister(registration.ID).Wait();
            });

            return app;
        }

        private static Uri GetListenAddress(DiscoveryOptions serviceDiscoveryOptions, string appFeatureAddress)
        {
            var listenAddress = serviceDiscoveryOptions.GetHostAddress();
            if (!string.IsNullOrEmpty(appFeatureAddress))
            {
                var appFeatureUri = new Uri(appFeatureAddress.Replace("+", "localhost"));
                return new Uri($"{appFeatureUri.Scheme}://{listenAddress}:{appFeatureUri.Port}");
            }
            return new Uri($"http://localhost:5000");
        }
    }
}