using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Marten;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Warehouse.Models;

namespace Warehouse.Infrastructure
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

                var hasData = documentSession.Query<StockItem>().Any();
                _logger.LogInformation("checking state");

                if (hasData)
                    return Task.CompletedTask;

                _logger.LogInformation("inserting init data");

                documentSession.Insert(new StockItem()
                {
                    Id = "building-microservices-book",
                    Quantity = 50,
                    MaxAllowedStock = 50,
                    MinimumAllowedStock = 5
                });


                documentSession.Insert(new StockItem()
                {
                    Id = "clean-architecture-book",
                    Quantity = 10,
                    MaxAllowedStock = 30,
                    MinimumAllowedStock = 5
                });


                documentSession.Insert(new StockItem()
                {
                    Id = "devops-handbook-book",
                    Quantity = 34,
                    MaxAllowedStock = 100,
                    MinimumAllowedStock = 5
                });

                documentSession.Insert(new StockItem()
                {
                    Id = "domain-driven-design-book",
                    Quantity = 23,
                    MaxAllowedStock = 30,
                    MinimumAllowedStock = 5
                });


                documentSession.Insert(new StockItem()
                {
                    Id = "patterns-of-enterprise-architecture-book",
                    Quantity = 13,
                    MaxAllowedStock = 50,
                    MinimumAllowedStock = 5
                });


                documentSession.Insert(new StockItem()
                {
                    Id = "rabbit-in-depth-book",
                    Quantity = 19,
                    MaxAllowedStock = 30,
                    MinimumAllowedStock = 5
                });

                documentSession.Insert(new StockItem()
                {
                    Id = "terraform-up-and-running",
                    Quantity = 10,
                    MaxAllowedStock = 50,
                    MinimumAllowedStock = 5
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