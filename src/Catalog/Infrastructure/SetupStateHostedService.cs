using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Catalog.Models;
using Marten;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Catalog.Infrastructure
{
    public class SetupStateHostedService : IHostedService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<SetupStateHostedService> _logger;

        public SetupStateHostedService(IServiceScopeFactory scopeFactory, ILogger<SetupStateHostedService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }
        
        
        public Task StartAsync(CancellationToken cancellationToken)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var documentSession = scope.ServiceProvider.GetService<IDocumentSession>();

                var hasData = documentSession.Query<Item>().Any();
                _logger.LogInformation("checking state");

                if (hasData)
                    return Task.CompletedTask;

                _logger.LogInformation("inserting init data");

                string Location(string name) => $"./Images/{name}.jpg";

                documentSession.Insert(new Item()
                {
                    Id = "building-microservices-book",
                    Name = "Building Microservices",
                    Price = 22m,
                    Picture = Location("building-microservices")
                });


                documentSession.Insert(new Item()
                {
                    Id = "clean-architecture-book",
                    Name = "Clean Architecture",
                    Price = 29.99m,
                    Picture = Location("clean-architecture")
                });


                documentSession.Insert(new Item()
                {
                    Id = "devops-handbook-book",
                    Name = "Devops Handbook",
                    Price = 9.99m,
                    Picture = Location("devops-handbook")
                });

                documentSession.Insert(new Item()
                {
                    Id = "domain-driven-design-book",
                    Name = "Domain Driven Design",
                    Price = 27.99m,
                    Picture = Location("domain-driven-design")
                });


                documentSession.Insert(new Item()
                {
                    Id = "patterns-of-enterprise-architecture-book",
                    Name = "Patterns of Enterprise Architecture",
                    Price = 30m,
                    Picture = Location("patterns-of-enterprise-architecture")
                });


                documentSession.Insert(new Item()
                {
                    Id = "rabbit-in-depth-book",
                    Name = "Rabbit in Depth",
                    Price = 23m,
                    Picture = Location("rabbit-in-depth")
                });

                documentSession.Insert(new Item()
                {
                    Id = "terraform-up-and-running",
                    Name = "Terraform up and running",
                    Price = 24.99m,
                    Picture = Location("terraform-up-and-running")
                });

                documentSession.SaveChanges();
            }

            return Task.CompletedTask;

        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}