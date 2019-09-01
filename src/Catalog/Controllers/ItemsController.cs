using System.Collections.Generic;
using System.Linq;
using System.Net;
using Catalog.Models;
using Marten;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemsController : Controller
    {
        private readonly IDocumentSession _documentSession;
        private readonly IBus _bus;

        public ItemsController(IDocumentSession documentSession, IBus bus)
        {
            _documentSession = documentSession;
            _bus = bus;
        }

        [Route("")]
        [HttpGet]
        [ProducesResponseType(typeof(List<Models.Item>), (int)HttpStatusCode.Accepted)]
        public ActionResult Get()
        {
            return Json(_documentSession.Query<Item>().ToList());
        }

        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(Models.Item), (int)HttpStatusCode.Accepted)]
        public ActionResult Get(string id)
        {
            var storedItem = _documentSession.Query<Item>().FirstOrDefault(x => x.Id == id);
            if (storedItem == null)
                return NotFound();

            return Json(storedItem);
        }


        [HttpPut]
        [Route("")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(Models.Item), (int)HttpStatusCode.Accepted)]
        public ActionResult Put([FromBody] Item item)
        {
            var storedItem = _documentSession.Query<Item>().FirstOrDefault(x => x.Id == item.Id);

            if (storedItem == null)
                return NotFound();

            storedItem.Name = item.Name;
            storedItem.Price = item.Price;
            storedItem.Picture = item.Picture;

            var itemChanged = new Events.ItemChanged()
            {
                Item = new Events.Item()
                {
                    Id = item.Id,
                    Name = item.Name,
                    Picture = item.Picture,
                    Price = item.Price
                }
            };

            _bus.Publish(itemChanged);

            _documentSession.SaveChanges();
            return Json(item);
        }
    }
}