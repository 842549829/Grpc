using System;
using Discovery;
using GRpc.Client.Web.GRpcClient;
using GRpc.Model;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace GRpc.Client.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            // This switch must be set before creating the GrpcChannel/HttpClient.
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            Configuration = configuration;
        }

        private IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions();
            services.AddHttpClient<GRpcHttpClient>();
            services.Configure<DiscoveryOptions>(Configuration.GetSection("DiscoveryOptions"));
            services.AddClientDiscovery();
            services.AddScoped(serviceProvider =>
            {
                var httpClient = serviceProvider.GetRequiredService<GRpcHttpClient>();
                return GrpcClient.Create<Greeter.GreeterClient>(httpClient.Client);
            });
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
