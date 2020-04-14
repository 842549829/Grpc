using System.Collections.Generic;
using System.Threading.Tasks;
using Consul;

namespace Discovery.Client
{
    public interface IClientRouteManager
    {
        IEnumerable<ServiceEntry> GetRoutes(string serviceName);

        Task ClearAsync();
    }
}