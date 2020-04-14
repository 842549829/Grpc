using Microsoft.Extensions.Options;
using System;
using System.Net.Http;
using Discovery;
using Discovery.Client;

namespace GRpc.Client.Web.GRpcClient
{
    public class GRpcHttpClient
    {
        public HttpClient Client { get; private set; }

        public GRpcHttpClient(IOptionsSnapshot<DiscoveryOptions> optionsSnapshot, HttpClient httpClient, IAddressSelector addressSelector)
        {
            var url = addressSelector.Selector(optionsSnapshot.Value.ServiceName);
            httpClient.BaseAddress = new Uri(url);
            Client = httpClient;
        }
    }
}