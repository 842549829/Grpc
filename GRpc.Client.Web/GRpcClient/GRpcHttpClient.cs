using Microsoft.Extensions.Options;
using System;
using System.Net.Http;

namespace GRpc.Client.Web.GRpcClient
{
    public class GRpcHttpClient
    {
        public HttpClient Client { get; private set; }

        public GRpcHttpClient(IOptionsSnapshot<GRpcHttpClientOptions> optionsSnapshot, HttpClient httpClient)
        {
            var options = optionsSnapshot.Value;
            httpClient.BaseAddress = new Uri($"{options.Scheme}://{options.Host}:{options.Port}");
            Client = httpClient;
        }
    }
}