using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Netflix_clone.Controllers
{
    [AllowAnonymous]
    public class InfoController : Controller
    {
        [HttpGet("/About")]
        public IActionResult About() => View();

        [HttpGet("/Careers")]
        public IActionResult Careers() => View();

        [HttpGet("/Press")]
        public IActionResult Press() => View();

        [HttpGet("/Contact")]
        public IActionResult Contact() => View();

        [HttpGet("/Faq")]
        public IActionResult Faq() => View();

        [HttpGet("/Help")]
        public IActionResult Help() => View();

        [HttpGet("/Devices")]
        public IActionResult Devices() => View();

        [HttpGet("/Privacy")]
        public IActionResult Privacy() => View();

        [HttpGet("/Terms")]
        public IActionResult Terms() => View();

        [HttpGet("/Cookies")]
        public IActionResult Cookies() => View();
    }
}
