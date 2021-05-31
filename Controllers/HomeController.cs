using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net.Http;
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
            return Ok(new { HasErrors = result });
        }

        [HttpGet("read-them-all")]
        public async Task<IActionResult> ReadThemAll()
        {
            var result = await _myFileProvider.ReadThemAll();
            return Ok(new { Contents = result });
        }

        [HttpGet("break-1")]
        public async Task<IActionResult> Break1()
        {
            Dictionary<string, string> r1 = null;
            Dictionary<string, string> r2 = null;
            bool r3 = false;
            Dictionary<string, string> r4 = null;
            bool r5 = false;
            Dictionary<string, string> r6 = null;

            for (int i = 0; i < 5; i++)
            {
                r1 = await _myFileProvider.Recreate();
                await Task.Delay(200);
                r2 = await _myFileProvider.ReadThemAll();
                await Task.Delay(200);
                r3 = await _myFileProvider.PurgeAsync();
                await Task.Delay(200);
                r4 = await _myFileProvider.ReadThemAll();
                await Task.Delay(200);
                r5 = await _myFileProvider.PurgeAsync();
                await Task.Delay(200);
                r6 = await _myFileProvider.Recreate();
            }

            return Ok(new { R1 = r1, R2 = r2, R3 = r3, R4 = r4, R5 = r5, R6 = r6 });
        }

        [HttpGet("break-2")]
        public async Task<IActionResult> Break2([FromServices] IHttpClientFactory httpClientFactory)
        {
            var httpClient = httpClientFactory.CreateClient();
            httpClient.BaseAddress = new System.Uri($"{Request.Scheme}://{Request.Host.Value}");

            string r1 = null;
            string r2 = null;
            string r3 = null;
            string r4 = null;
            string r5 = null;
            string r6 = null;

            for (int i = 0; i < 5; i++)
            {
                r1 = await httpClient.GetStringAsync("/recreate-files-and-directories");
                await Task.Delay(200);
                r2 = await httpClient.GetStringAsync("/read-them-all");
                await Task.Delay(200);
                r3 = await httpClient.GetStringAsync("/purge");
                await Task.Delay(200);
                r4 = await httpClient.GetStringAsync("/read-them-all");
                await Task.Delay(200);
                r5 = await httpClient.GetStringAsync("/purge");
                await Task.Delay(200);
                r6 = await httpClient.GetStringAsync("/recreate-files-and-directories");
            }

            return Ok(new { R1 = r1, R2 = r2, R3 = r3, R4 = r4, R5 = r5, R6 = r6 });
        }

        [HttpGet("break-3")]
        public async Task<IActionResult> Break3()
        {
            for (int i = 0; i < 5; i++)
            {
                await _myFileProvider.Recreate();
                await Task.Delay(200);
                await _myFileProvider.ReadThemAll();
                await Task.Delay(200);
                await _myFileProvider.PurgeAsync();
                await Task.Delay(200);
                await _myFileProvider.ReadThemAll();
                await Task.Delay(200);
                await _myFileProvider.PurgeAsync();
                await Task.Delay(200);
                await _myFileProvider.Recreate();
            }
            return Ok();
        }

        [HttpGet("break-4")]
        public async Task<IActionResult> Break4([FromServices] IHttpClientFactory httpClientFactory)
        {
            var httpClient = httpClientFactory.CreateClient();
            httpClient.BaseAddress = new System.Uri($"{Request.Scheme}://{Request.Host.Value}");

            for (int i = 0; i < 5; i++)
            {
                await httpClient.GetStringAsync("/recreate-files-and-directories");
                await Task.Delay(100);
                await httpClient.GetStringAsync("/read-them-all");
                await Task.Delay(100);
                await httpClient.GetStringAsync("/purge");
                await Task.Delay(100);
                await httpClient.GetStringAsync("/read-them-all");
                await Task.Delay(100);
                await httpClient.GetStringAsync("/purge");
                await Task.Delay(100);
                await httpClient.GetStringAsync("/recreate-files-and-directories");
            }

            return Ok();
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
