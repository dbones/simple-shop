using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Marten;
using Marten.Linq;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Swagger;
using Warehouse.Models;

namespace Warehouse.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StockController : Controller
    {
        private readonly IDocumentSession _documentSession;

        public StockController(IDocumentSession documentSession)
        {
            _documentSession = documentSession;
        }

        [HttpGet("{id}")]
        public ActionResult Get(string id)
        {
            var stock = _documentSession.Query<StockItem>().FirstOrDefault(x=> x.Id == id);

            if (stock == null)
                return NotFound();
            
            return Json(stock);
        }
        
        [HttpPost("restock")]
        public ActionResult Restock()
        {
            var stockItems = _documentSession
                .Query<StockItem>()
                .Where(x => x.Quantity < x.MinimumAllowedStock);

            foreach (var stock in stockItems)
            {
                stock.Quantity = stock.MaxAllowedStock;
            }
            
            _documentSession.SaveChanges();
            
            return Ok();
        }
    }
}