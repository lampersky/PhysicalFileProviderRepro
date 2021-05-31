using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        private readonly IMyFileProvider _myFileProvider;
        private readonly IApplicationLifetime _applicationLifetime;

        public HomeController(IMyFileProvider myFileProvider, IApplicationLifetime applicationLifetime)
        {
            _myFileProvider = myFileProvider;
            _applicationLifetime = applicationLifetime;
        }

        [HttpGet("restart")]
        public IActionResult Restart()
        {
            _applicationLifetime.StopApplication();
            return new EmptyResult();
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
            return Ok(new { HasErrors = result });
        }

        [HttpGet("read-them-all")]
        public async Task<IActionResult> ReadThemAll()
        {
            var result = await _myFileProvider.ReadThemAll();
            result.Add("Testing", "Testing123456");
            return Ok(new { Contents = result });
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
