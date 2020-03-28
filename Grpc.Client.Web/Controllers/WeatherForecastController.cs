using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Protobuf.Collections;
using GRpc.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace GRpc.Client.Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly ILogger<WeatherForecastController> _logger;

        private readonly Greeter.GreeterClient _greeterClient;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, Greeter.GreeterClient greeterClient)
        {
            _logger = logger;
            _greeterClient = greeterClient;
        }

        [HttpGet]
        public async Task<HelloReply> Get()
        {
            var testModel = new RepeatedField<TestModel>()
            {
                new TestModel
                {
                    Corpus = Corpus.Images,
                    Key = "aaa",
                    Mong = 999
                }
            };
            var helloRequest = new HelloRequest
            {
                Name = "那敌对",
                UpTime = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(DateTime.Now.ToUniversalTime())
            };
            helloRequest.ModelList.AddRange(testModel);
            var result = await _greeterClient.SayHelloAsync(helloRequest);
            return result;
        }
    }
}
