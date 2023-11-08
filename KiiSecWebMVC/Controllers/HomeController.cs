using KiiSecWeb.Helper;
using KiiSecWebMVC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace KiiSecWebMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger; 
        private readonly IEmailSender _emailSender;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;

        public HomeController(ILogger<HomeController> logger, IEmailSender emailSender,
            SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager)
        {
            _logger = logger;
            _emailSender = emailSender;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            IdentityUser user = await _userManager.GetUserAsync(User);
            if (_userManager.GetRolesAsync(user).Result.Contains("Admin")) {
                return Redirect("/Organization/List");
            }
            if (_userManager.GetRolesAsync(user).Result.Contains("Organization"))
            {
                if (KiiSecAPI.GetOrganizations().Result.FirstOrDefault(o => o.Email == user.Email) != null)
                {
                    return Redirect("/Organization/Details");
                }
                return Redirect("/Organization/Register");
            }
            if (_userManager.GetRolesAsync(user).Result.Contains("Visitor"))
            {
                if (KiiSecAPI.GetVisitors().Result.FirstOrDefault(o => o.Email == user.Email) != null)
                {
                    return Redirect("/Visitor/Details");
                }
                return Redirect("/Visitor/Create");
            }
            return View();
        }

        public IActionResult Fail()
        {
            return View();
        }
        public IActionResult Success()
        {
            return View();
        }
    }
}
