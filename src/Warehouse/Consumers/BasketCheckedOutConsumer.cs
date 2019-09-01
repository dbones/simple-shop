using System.Linq;
using System.Threading.Tasks;
using Basket.Events;
using Marten;
using Marten.Linq;
using MassTransit;
using Warehouse.Events;
using Warehouse.Models;

namespace Warehouse.Consumers
{
    public class BasketCheckedOutConsumer : IConsumer<Basket.Events.BasketCheckedOut>
    {
        private readonly IDocumentSession _documentSession;
        private readonly IBus _bus;

        public BasketCheckedOutConsumer(IDocumentSession documentSession, IBus bus)
        {
            _documentSession = documentSession;
            _bus = bus;
        }
        
        
        public Task Consume(ConsumeContext<BasketCheckedOut> context)
        {
            var basket = context.Message.Basket;
            
            //unabele to do this query, will have to do a number of selects
            //var itemIds = basket.Items.Select(x => x.Id).ToList();

            //var stockItems = _documentSession.Query<StockItem>().Where(x => itemIds.Any(id => id == x.Id)).ToList();

            var reserved = new ReservedStock()
            {
                Id = basket.Id
            };
            
            foreach (var basketItem in basket.Items)
            {
                //var stockItem = stockItems.FirstOrDefault(x => x.Id == basketItem.Id);
                var stockItem = _documentSession.Query<StockItem>().First(x=> x.Id == basketItem.Id);
                if (stockItem.Quantity < basketItem.Quantity) continue; 
                
                reserved.Items.Add(basketItem.Id, basketItem.Quantity);
                stockItem.Quantity -= basketItem.Quantity;
            }
            
            _documentSession.Insert(reserved);
            _documentSession.SaveChanges();
            
            var reservedStock = new StockReserved()
            {
                Id = reserved.Id,
                Items = reserved.Items
            };
            _bus.Publish(reservedStock);
            
            return Task.CompletedTask;
            
        }
    }
}