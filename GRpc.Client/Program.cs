using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using GRpc.Model;
using Grpc.Net.Client;

namespace GRpc.Client
{
    class Program
    {
        static async Task Main(string[] args)
        {
            AppContext.SetSwitch(
                "System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport",
                true);
            var httpClient = new HttpClient();
            // The port number(50051) must match the port of the gRPC server.
            httpClient.BaseAddress = new Uri("http://localhost:50051");
            var client = GrpcClient.Create<Greeter.GreeterClient>(httpClient);

            var st = new Stopwatch();
            while (Console.ReadKey().Key != ConsoleKey.Q)
            {
                st.Restart();
                for (int i = 0; i < 10000; i++)
                {
                    var reply = await client.SayHelloAsync(
                        new HelloRequest { Name = "GreeterClient", UpTime = new Google.Protobuf.WellKnownTypes.Timestamp { Seconds = 5851444 } });
                    //Console.WriteLine("Greeting: " + reply.Message);
                }

                st.Stop();
                Console.WriteLine($"time:{st.ElapsedMilliseconds}");
            }

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
