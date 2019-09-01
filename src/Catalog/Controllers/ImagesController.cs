using System.IO;
using System.Linq;
using System.Net;
using Catalog.Models;
using Marten;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;


namespace Catalog.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImagesController : Controller
    {
        private readonly IDocumentSession _documentSession;
        private readonly IHostingEnvironment _env;
        private readonly ILogger<ImagesController> _logger;

        public ImagesController(
            IDocumentSession documentSession, 
            IHostingEnvironment env, 
            ILogger<ImagesController> logger)
        {
            _documentSession = documentSession;
            _env = env;
            _logger = logger;
        }
        
        [HttpGet]
        [Route("item/{id}")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(byte[]), (int)HttpStatusCode.Accepted)]
        public ActionResult Get(string id)
        {
            var storedItem = _documentSession.Query<Item>().FirstOrDefault(x => x.Id == id);
            if (storedItem == null)
                return NotFound();
            
            var webRoot = _env.ContentRootPath;
            var path = Path.Combine(webRoot, storedItem.Picture);
            
            _logger.LogDebug($"image location: {path}");

            if (!System.IO.File.Exists(path))
            {
                _logger.LogWarning($"cannot find image {path}");
                return NotFound();
            }
            
            var buffer = System.IO.File.ReadAllBytes(path);
            return File(buffer, "image/jpeg");
            
        }
        
    }
}