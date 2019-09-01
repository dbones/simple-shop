using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Core.Infrastructure.Application;
using Core.Infrastructure.Logging;
using Core.Infrastructure.Marten;
using Core.Infrastructure.MassTransit;
using Core.Infrastructure.Serializing;
using Core.Infrastructure.Swagger;
using Core.Infrastructure.Tracing;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Warehouse
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .AddCommandLine(args)
                .Build();

            var port = configuration.GetSection("Application").Get<ApplicationConfiguration>().PortNumber;

            var host = WebHost.CreateDefaultBuilder(args)

                .UseConfiguration(configuration)
                .ConfigureLogging((hostingContext, builder) =>
                {
                    builder.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                })
                .ConfigureLogging()
                .ConfigureMartin()
                .ConfigureSwagger()
                .ConfigureSerializer()
                .ConfigureMassTransit()
                .ConfigureTracing(configuration)
                //.ConfigureWebApi()
                .UseUrls($"http://*:{port}")
                .UseKestrel()
                .UseStartup<Startup>()
                .Build();

            host.Run();
        }

    }
}