using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Catalog.Events;
using Jaeger.Thrift.Agent;
using Jaeger.Thrift.Agent.Zipkin;
using Marten;
using MassTransit;
using Ordering.Models;
using Warehouse.Events;
using Item = Ordering.Models.Item;

namespace Ordering.Consumers
{
    public class ItemChangedConsumer : IConsumer<ItemChanged>
    {
        private readonly IDocumentSession _documentSession;

        public ItemChangedConsumer(IDocumentSession documentSession)
        {
            _documentSession = documentSession;
        }
        
        public Task Consume(ConsumeContext<ItemChanged> context)
        {
            var item = _documentSession.Query<Item>().FirstOrDefault(x => x.Id == context.Message.Item.Id);
            item.Price = context.Message.Item.Price;
            
            _documentSession.SaveChanges();
            
            return Task.CompletedTask;
        }
    }
    
    public class StockReservedConsumer : IConsumer<StockReserved>
    {
        private readonly IDocumentSession _documentSession;


        public StockReservedConsumer(IDocumentSession documentSession)
        {
            _documentSession = documentSession;
        }
        
        public Task Consume(ConsumeContext<StockReserved> context)
        {
            var reserved = context.Message;
            var order = _documentSession.Query<Order>().FirstOrDefault(x => x.Id == context.Message.Id);

            var subTotal = 0m;
            foreach (var orderItem in order.Items)
            {
                if (!reserved.Items.TryGetValue(orderItem.Id, out var quantity)) continue;
                
                orderItem.ValidatedStock = true;
                var price = orderItem.Price ?? 0;
                subTotal += (price * quantity);
            }

            order.SubTotal = subTotal;
            order.IsOrderReady = true;
            
            _documentSession.SaveChanges();
            return Task.CompletedTask;
        }
    }
}