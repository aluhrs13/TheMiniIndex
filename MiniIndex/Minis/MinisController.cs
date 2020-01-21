using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace MiniIndex.Controllers
{
    [Route("minis")]
    public class MinisController : Controller
    {
        [HttpGet("browse")]
        public async Task<IActionResult> BrowseMinis()
        {
            return View("BrowseMinis");
        }
    }
}