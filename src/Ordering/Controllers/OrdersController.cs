using System.Collections.Generic;
using System.Linq;
using Marten;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Ordering.Events;
using Ordering.Models;

namespace Ordering.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : Controller
    {
        private readonly IDocumentSession _documentSession;
        private readonly IBus _bus;

        public OrdersController(IDocumentSession documentSession, IBus bus)
        {
            _documentSession = documentSession;
            _bus = bus;
        }

        [HttpGet("{id}")]
        public ActionResult Get(string id)
        {
            var order = _documentSession.Query<Order>().FirstOrDefault(x => x.Id == id);
            
            if (order == null)
                return NotFound();
            
            return Json(order);
        }

        // POST api/values
        [HttpPost]
        [Route("complete")]
        
        public ActionResult Post([FromBody] Order order)
        {
            var retrievedOrder = _documentSession.Query<Order>().FirstOrDefault(x => x.Id == order.Id);

            if (retrievedOrder.IsComplete)
                return Ok();

            retrievedOrder.IsComplete = true;

            var orderCompleted = new OrderCompleted()
            {
                Id = retrievedOrder.Id
            };
            
            
            _bus.Publish(orderCompleted);
            _documentSession.SaveChanges();
            return Ok();

        }
    }
}