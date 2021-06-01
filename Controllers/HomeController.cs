using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        private readonly IMyFileProvider _myFileProvider;

        public HomeController(IMyFileProvider myFileProvider)
        {
            _myFileProvider = myFileProvider;
        }

        [HttpGet("recreate-files-and-directories")]
        public async Task<IActionResult> Recreate()
        {
            var result = await _myFileProvider.Recreate();
            return Ok(new { Errors = result });
        }

        [HttpGet("purge")]
        public async Task<IActionResult> Purge()
        {
            var result = await _myFileProvider.PurgeAsync();
            return Ok(new { Errors = result });
        }

        [HttpGet("read-them-all")]
        public async Task<IActionResult> ReadThemAll()
        {
            var result = await _myFileProvider.ReadThemAll();
            return Ok(new { Contents = result });
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
