using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Marten;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Ordering.Models;

namespace Ordering.Infrastructure
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

                documentSession.Insert(new Item()
                {
                    Id = "building-microservices-book",
                    Price = 22m
                });


                documentSession.Insert(new Item()
                {
                    Id = "clean-architecture-book",
                    Price = 29.99m,
                });


                documentSession.Insert(new Item()
                {
                    Id = "devops-handbook-book",
                    Price = 9.99m,
                });

                documentSession.Insert(new Item()
                {
                    Id = "domain-driven-design-book",
                    Price = 27.99m,
                });


                documentSession.Insert(new Item()
                {
                    Id = "patterns-of-enterprise-architecture-book",
                    Price = 30m,
                });


                documentSession.Insert(new Item()
                {
                    Id = "rabbit-in-depth-book",
                    Price = 23m,
                });

                documentSession.Insert(new Item()
                {
                    Id = "terraform-up-and-running",
                    Price = 24.99m,
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