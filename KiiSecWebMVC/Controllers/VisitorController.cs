using KiiSec.Models;
using KiiSecWeb.Helper;
using KiiSecWebMVC.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using Microsoft.Build.Evaluation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using System.Net;
using Humanizer.Localisation.TimeToClockNotation;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace KiiSecWebMVC.Controllers
{
    public class VisitorController : Controller
    {
        KiiSecAPI _api = new KiiSecAPI();
        private readonly UserManager<IdentityUser> _userManager;

        public VisitorController(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }
        public ActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> Create(Visitor visitor)
        {
            IdentityUser user = await _userManager.GetUserAsync(User);
            if (KiiSecAPI.GetVisitors().Result.FirstOrDefault(o => o.Email == user.Email) != null)
            {
                return Redirect("~/Home/Fail"); // same change
            }
            visitor.Email = user.Email;
            ModelState.Remove("Email");
            if (ModelState.IsValid)
            {
                HttpClient client = _api.Initial();
                string json = JsonConvert.SerializeObject(visitor);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = client.PostAsync("api/Visitor", content).Result;
                if (response.IsSuccessStatusCode)
                {
                    return Redirect("~/Home/Success"); // TODO Change to RedirectToACtion
                }
                return Redirect("~/Home/Fail");
            }
            return View();
        }

        public async Task<ActionResult> Edit(Visitor visitor)
        {
            HttpClient client = _api.Initial();
            HttpResponseMessage response;
            if (ModelState.IsValid)
            {              
                string json = JsonConvert.SerializeObject(visitor);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                response = client.PutAsync("api/Visitor", content).Result;
                if (response.IsSuccessStatusCode)
                {
                    return Redirect("~/Home/Success");
                }
                return Redirect("~/Home/Fail");
            }
            response = await client.GetAsync($"/api/Visitor/{visitor.ID}");
            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                visitor = JsonConvert.DeserializeObject<Visitor>(result);
            }
            return View(visitor);
        }

        public async Task<ActionResult> Details()
        {
            IdentityUser user = await _userManager.GetUserAsync(User);
            int visitorId = KiiSecAPI.GetVisitors().Result.Where(o => o.Email == user.Email).FirstOrDefault().ID;
            Visitor visitor = new Visitor();
            HttpClient client = _api.Initial();
            HttpResponseMessage response = await client.GetAsync($"/api/Visitor/{visitorId}");
            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                visitor = JsonConvert.DeserializeObject<Visitor>(result);
            }
            if (!(user.Email == visitor.Email || _userManager.GetRolesAsync(user).Result.Contains("Admin")))
            {
                return BadRequest();
            }
            return View(visitor);
        }

    }
}
