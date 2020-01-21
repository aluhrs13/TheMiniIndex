using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace MiniIndex.Minis
{
    [Route("dev/minis")]
    public class MinisController : Controller
    {
        [HttpGet("")]
        public async Task<IActionResult> BrowseMinis()
        {
            return View("BrowseMinis", new BrowseModel());
        }
    }
}