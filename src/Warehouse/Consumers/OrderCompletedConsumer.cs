using System.Linq;
using System.Threading.Tasks;
using Marten;
using MassTransit;
using Ordering.Events;
using Warehouse.Models;

namespace Warehouse.Consumers
{
    public class OrderCompletedConsumer : IConsumer<OrderCompleted>
    {
        private readonly IDocumentSession _documentSession;

        public OrderCompletedConsumer(IDocumentSession documentSession)
        {
            _documentSession = documentSession;
        }
        
        public Task Consume(ConsumeContext<OrderCompleted> context)
        {
            var reservedStock = _documentSession
                .Query<ReservedStock>()
                .FirstOrDefault(x => x.Id == context.Message.Id);
            
            _documentSession.Delete(reservedStock);
            _documentSession.SaveChanges();
            
            return Task.CompletedTask;
        }
    }
}