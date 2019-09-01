using System;
using System.Linq;
using System.Net;
using Basket.Events;
using Basket.Models;
using Core.Infrastructure;
using Core.Infrastructure.Serializing;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using BasketItem = Basket.Models.BasketItem;

namespace Basket.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BasketsController : Controller
    {
        private readonly IDatabase _database;
        private readonly IBus _bus;
        private readonly JsonSerializer _serializer;

        public BasketsController(IDatabase database, IBus bus,  JsonSerializer serializer)
        {
            _database = database;
            _bus = bus;
            _serializer = serializer;
        }

        /// <summary>
        /// create new basket
        /// </summary>
        [HttpPost]
        [Route("")]
        [ProducesResponseType(typeof(Models.Basket), (int) HttpStatusCode.OK)]
        public IActionResult Post([FromBody]User user)
        {
            var basketId = Guid.NewGuid().ToString("D");
            
            var basket = new Models.Basket()
            {
                Created = DateTime.Now,
                Id = basketId,
                UserId = user.Id
            };

            _database.StringSet(basketId, _serializer.Serialize(basket),new TimeSpan(0, 30, 0));
            return Json(basket);
        }
        

        [HttpPost]
        [Route("{basketId}/items")]
        [ProducesResponseType(typeof(Models.Basket), (int) HttpStatusCode.OK)]
        public ActionResult<Models.Basket> Post(string basketId, [FromBody]BasketItem item)
        {
            var basket = _serializer.Deserialize<Models.Basket>(_database.StringGet(basketId));
            var existingItem = basket.Items.FirstOrDefault(x => x.Id == item.Id);
            if (existingItem == null)
            {
                basket.Items.Add(item);
            }
            else
            {
                existingItem.Quantity = item.Quantity;
            }

            _database.StringSet(basketId, _serializer.Serialize(basket), new TimeSpan(0, 30, 0));
            return Json(basket);
        }
        
        [HttpPost]
        [Route("checkout")]
        [ProducesResponseType(typeof(Models.Basket), (int)HttpStatusCode.Accepted)]
        public ActionResult Post([FromBody]Models.Basket basket)
        {
            basket = _serializer.Deserialize<Models.Basket>(_database.StringGet(basket.Id));

            var basketCheckedOut = new BasketCheckedOut()
            {
                Basket = new Events.Basket()
                {
                    Created = basket.Created,
                    Id = basket.Id,
                    UserId = basket.UserId,
                    Items = basket.Items.Select(x=> new Events.BasketItem()
                    {
                        Quantity = x.Quantity,
                        Id = x.Id
                    }).ToList()
                }
            };

            _bus.Publish(basketCheckedOut);
            _database.KeyDelete(basket.Id);
            return Json(basket);
        }
        

        [HttpDelete]
        [Route("{basketId}")]
        [ProducesResponseType((int)HttpStatusCode.Accepted)]
        public ActionResult Delete(string basketId)
        {
            _database.KeyDelete(basketId);
            return Ok();
        }
    }
}