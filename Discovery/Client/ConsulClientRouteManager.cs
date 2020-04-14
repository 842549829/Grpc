using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Timers;
using Consul;

namespace Discovery.Client
{
    public class ConsulClientRouteManager : IClientRouteManager
    {
        private readonly IConsulClient _consulClient;
        private static readonly object SyncObject = new object();
        private readonly Timer _timer = new Timer();

        private readonly ConcurrentDictionary<string, IEnumerable<ServiceEntry>> _serviceRoutes =
            new ConcurrentDictionary<string, IEnumerable<ServiceEntry>>();

        public ConsulClientRouteManager(
            IConsulClient consulClient)
        {
            _consulClient = consulClient;
            _timer.Interval = TimeSpan.FromSeconds(10).TotalMilliseconds;
            _timer.Elapsed += _timer_Elapsed;
            _timer.Start();
        }

        private void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            lock (SyncObject)
            {
                UpdateRouteFromConsul();
            }
        }

        public IEnumerable<ServiceEntry> GetRoutes(string serviceName)
        {
            lock (SyncObject)
            {
                if (_serviceRoutes.Count == 0)
                {
                    UpdateRouteFromConsul();
                }
            }

            lock (SyncObject)
            {
                _serviceRoutes.TryGetValue(serviceName, out var serviceEntries);
                return serviceEntries;
            }
        }
        
        public Task ClearAsync()
        {
            lock (SyncObject)
            {
                _serviceRoutes.Clear();
                return Task.CompletedTask;
            }
        }

        private void UpdateRouteFromConsul()
        {
            var result = GetServiceRoutesRemoteAsync();
            UpdateRoutes(result);
        }

        private IDictionary<string, IEnumerable<ServiceEntry>> GetServiceRoutesRemoteAsync()
        {
            var serviceRoutes = new Dictionary<string, IEnumerable<ServiceEntry>>();
            var serviceQuery = _consulClient.Catalog.Services().Result;
            foreach (var (key, _) in serviceQuery.Response)
            {
                var query = _consulClient.Health.Service(key).Result;
                var serviceEntries = query.Response;
                if (serviceEntries == null)
                {
                    continue;
                }
                serviceRoutes.TryAdd(key, serviceEntries);
            }
            return serviceRoutes;
        }

        private void UpdateRoutes(IDictionary<string, IEnumerable<ServiceEntry>> serviceRoutes)
        {
            foreach (var (key, value) in serviceRoutes)
            {
                lock (SyncObject)
                {
                    _serviceRoutes.GetOrAdd(key, value);
                }
            }
        }
    }
}