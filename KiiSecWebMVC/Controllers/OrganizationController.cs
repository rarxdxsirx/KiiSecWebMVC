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
    public class OrganizationController : Controller
    {

        KiiSecAPI _api = new KiiSecAPI();
        private readonly UserManager<IdentityUser> _userManager;

        public OrganizationController(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            //TODO: Redirection deffs on employee/organization roles
            return View();
        }
        public IActionResult Success()
        {
            return View();
        }
        public IActionResult Fail()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> List()
        {
            return View(KiiSecAPI.GetOrganizations().Result);
        }

        [Authorize(Roles = "Admin, Organization")]
        public async Task<IActionResult> Register(Organization organization) 
        {
            IdentityUser user = await _userManager.GetUserAsync(User);
            // TODO Organization exists in API mb?
            if (KiiSecAPI.GetOrganizations().Result.FirstOrDefault(o => o.Email == user.Email) != null)
            {
                return Redirect("~/Home/Fail"); // same change
            }
            organization.Email = user.Email;
            ModelState.Remove("Email");
            if (ModelState.IsValid)
            {
                HttpClient client = _api.Initial();
                string json = JsonConvert.SerializeObject(organization);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = client.PostAsync("api/Organization", content).Result;
                if (response.IsSuccessStatusCode)
                {

                    return Redirect("~/Home/Success"); // TODO Change to RedirectToACtion
                }
                return Redirect("~/Home/Fail");
            }   
            return View();
            // same change

        }

        [Authorize(Roles = "Admin, Organization")]
        public async Task<IActionResult> Details()
        {
            IdentityUser user = await _userManager.GetUserAsync(User);
            int organizationId = KiiSecAPI.GetOrganizations().Result.Where(o => o.Email == user.Email).FirstOrDefault().ID;
            Organization organization = new Organization();
            HttpClient client = _api.Initial();
            HttpResponseMessage response = await client.GetAsync($"/api/Organization/{organizationId}");
            if (response.IsSuccessStatusCode)
            {              
                var result = response.Content.ReadAsStringAsync().Result;
                organization = JsonConvert.DeserializeObject<Organization>(result);
            }
            if (!(user.Email == organization.Email || _userManager.GetRolesAsync(user).Result.Contains("Admin")))
            {
                return BadRequest();
            }
            return View(organization);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DetailsForAdmins(int organizationId)
        {
            Organization organization = new Organization();
            HttpClient client = _api.Initial();
            HttpResponseMessage response = await client.GetAsync($"/api/Organization/{organizationId}");
            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                organization = JsonConvert.DeserializeObject<Organization>(result);
            }
            return View(organization);
        }

        [Authorize(Roles = "Admin, Organization")]
        public async Task<IActionResult> Edit(Organization organization)
        {
            //TODO Check for valid organization

            if (ModelState.IsValid)
            {
                HttpClient client = _api.Initial();
                string json = JsonConvert.SerializeObject(organization);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                //TODO Create Organization PUT API methid
                var response = client.PutAsync("api/Organization", content).Result;
                if (response.IsSuccessStatusCode)
                {
                    return Redirect("~/Home/Success");
                }
                return Redirect("~/Home/Fail");
            }
            return View(organization);
        }


        //TODO Organization delete method
        //[Authorize(Roles = "Admin, Organization")]
        //public async Task<IActionResult> Delete(int organizationId)
        //{
            //TODO Check for valid organization
        //}
    }
}