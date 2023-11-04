using KiiSecWebMVC.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace KiiSecWebMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<OrganizationController> _logger;

        public HomeController(ILogger<OrganizationController> logger)
        {
            _logger = logger;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Privacy()
        {
            return View();
        }
    }
}
