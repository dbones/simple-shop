using System;
using System.Linq;
using Marten;
using Microsoft.AspNetCore.Mvc;
using Warehouse.Models;

namespace Warehouse.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservedStockController : Controller
    {
        private readonly IDocumentSession _documentSession;

        public ReservedStockController(IDocumentSession documentSession)
        {
            _documentSession = documentSession;
        }

        [HttpGet("{id}")]
        public ActionResult Get(string id)
        {
            var reservedStock = _documentSession.Query<ReservedStock>().FirstOrDefault(x=> x.Id == id);

            if (reservedStock == null)
                return NotFound();
            
            return Json(reservedStock);
        }
        
        
        [HttpDelete("{id}")]
        public ActionResult Delete(string id)
        {
            var reservedStock = _documentSession.Query<ReservedStock>().FirstOrDefault(x=> x.Id == id);

            if (reservedStock == null)
                return NotFound();

            RemoveReservation(reservedStock);
            
            _documentSession.SaveChanges();
            
            return Ok();
        }
        
        [HttpDelete("prune")]
        public ActionResult Prune()
        {
            var reservedStock = _documentSession
                .Query<ReservedStock>()
                .Where(x => x.DateTime.AddMinutes(10) <= DateTime.UtcNow );

            foreach (var stock in reservedStock)
            {
                RemoveReservation(stock);
            }
            
             _documentSession.SaveChanges();
            
            return Ok();
        }
        
        

        private void RemoveReservation(ReservedStock reservedStock)
        {
            var itemIds = reservedStock.Items.Keys;
            var items = _documentSession.Query<StockItem>().Where(x => itemIds.Any(iid => iid == x.Id)).ToList();

            foreach (var stockItem in items)
            {
                stockItem.Quantity += reservedStock.Items[stockItem.Id];
            }
        }
        
    }
}