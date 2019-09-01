using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Basket.Events;
using Marten;
using MassTransit;
using Ordering.Models;

namespace Ordering.Consumers
{
    public class BasketCheckedOutConsumer : IConsumer<Basket.Events.BasketCheckedOut>
    {
        private readonly IDocumentSession _documentSession;

        public BasketCheckedOutConsumer(IDocumentSession documentSession)
        {
            _documentSession = documentSession;
        }
        
        public Task Consume(ConsumeContext<BasketCheckedOut> context)
        {
            var basket = context.Message.Basket;

            var order = new Order
            {
                Id = basket.Id,
                UserId = basket.UserId,
                Items = basket.Items.Select(x => new OrderItem()
                {
                    Id = x.Id,
                    Quantity = x.Quantity
                }).ToList()
            };

            var itemIds = order.Items.Select(x => x.Id).ToList();
      
            //this does not look to be supported (will have to do a number of selects)
            //var items = _documentSession.Query<Item>().Where(x => itemIds.Any(id => id == x.Id)).ToList();


            foreach (var orderItem in order.Items)
            {
                //orderItem.Price = items.FirstOrDefault(x => x.Id == orderItem.Id)?.Price;
                orderItem.Price = _documentSession.Query<Item>().First(x => x.Id == orderItem.Id)?.Price;
            }
            
            _documentSession.Insert(order);
            _documentSession.SaveChanges();
            return Task.CompletedTask;
        }
    }
}