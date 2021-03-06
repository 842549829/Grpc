using System.Threading.Tasks;
using Consul;
using Google.Protobuf.Collections;
using Grpc.Core;
using GRpc.Model;

namespace GRpc.Server.Services
{
    public class GreeterService : Greeter.GreeterBase
    {
        public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
        {
            var result = new HelloReply
            {
                Message = "Hello " + request.Name + "  " + new DateTime(request.UpTime.Seconds).ToString("yyyy-MM-dd hh:mm:ss")
            };
            result.ModelList.AddRange(request.ModelList);
            return Task.FromResult(result);
        }
    }
}
