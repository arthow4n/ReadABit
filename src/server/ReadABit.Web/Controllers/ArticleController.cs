using Microsoft.AspNetCore.Mvc;

namespace ReadABit.Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ArticleController : ControllerBase
    {
        [HttpGet]
        public IActionResult List()
        {
            return Ok();
        }
    }
}
