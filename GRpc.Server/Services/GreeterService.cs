using System.Threading.Tasks;
using Grpc.Core;
using GRpc.Model;

namespace GRpc.Server.Services
{
    public class GreeterService : Greeter.GreeterBase
    {
        public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
        {
            return Task.FromResult(new HelloReply
            {
                Message = "Hello " + request.Name + "  " + request.UpTime.ToDateTime().ToString("yyyy-mm-dd:mm:ss")
            });
        }
    }
}
