using System.Threading.Tasks;
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
                Message = "Hello " + request.Name + "  " + request.UpTime.ToDateTime().ToString("yyyy-mm-dd:mm:ss")
            };
            result.ModelList.AddRange(request.ModelList);
            return Task.FromResult(result);
        }
    }
}
