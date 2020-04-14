using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Discovery.Client
{
    public class PollingAddressSelector : IAddressSelector
    {
        private readonly IClientRouteManager _clientRouteManager;

        private readonly ConcurrentDictionary<string, Lazy<AddressEntry>> _concurrent = new ConcurrentDictionary<string, Lazy<AddressEntry>>();

        public PollingAddressSelector(IClientRouteManager clientRouteManager)
        {
            _clientRouteManager = clientRouteManager;
        }

        public string Selector(string serviceName)
        {
            var addressEntry = _concurrent.GetOrAdd(serviceName, k => new Lazy<AddressEntry>(() => new AddressEntry(GetRoutes(serviceName)))).Value;
            return addressEntry.GetAddress();
        }

        private IEnumerable<string> GetRoutes(string serviceName)
        {
            var routes = _clientRouteManager.GetRoutes(serviceName);
            return routes.Select(item => $"http://{item.Service.Address}:{item.Service.Port}");
        }

        protected class AddressEntry
        {
            #region Field

            private int _index;
            private int _lock;
            private readonly int _maxIndex;
            private readonly string[] _address;

            #endregion Field

            #region Constructor

            public AddressEntry(IEnumerable<string> address)
            {
                _address = address.ToArray();
                _maxIndex = _address.Length - 1;
            }

            #endregion Constructor

            #region Public Method

            public string GetAddress()
            {
                while (true)
                {
                    //如果无法得到锁则等待
                    if (Interlocked.Exchange(ref _lock, 1) != 0)
                    {
                        default(SpinWait).SpinOnce();
                        continue;
                    }

                    var address = _address[_index];

                    //设置为下一个
                    if (_maxIndex > _index)
                        _index++;
                    else
                        _index = 0;

                    //释放锁
                    Interlocked.Exchange(ref _lock, 0);

                    return address;
                }
            }

            #endregion Public Method
        }
    }
}