using KiiSec.Models;
using KiiSecWeb.Helper;
using KiiSecWebMVC.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using Microsoft.Build.Evaluation;

namespace KiiSecWebMVC.Controllers
{
    public class OrganizationController : Controller
    {

        KiiSecAPI _api = new KiiSecAPI();

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> OrganizationsList()
        {
            List<Organization> organizations= new List<Organization>();
            HttpClient client = _api.Initial();
            HttpResponseMessage response = await client.GetAsync("api/Organization");
            if (response.IsSuccessStatusCode) 
            { 
                var result = response.Content.ReadAsStringAsync().Result;
                organizations = JsonConvert.DeserializeObject<List<Organization>>(result);
            }
            return View(organizations);
        }

        
        public async Task<IActionResult> OrganizationRegister(Organization organization) 
        {
            if (ModelState.IsValid)
            {
                HttpClient client = _api.Initial();
                string json = JsonConvert.SerializeObject(organization);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = client.PostAsync("api/Organization", content).Result;
                return Redirect("/");
                if (response.IsSuccessStatusCode)
                {

                }
                else 
                { 

                }
            }
            return View();
        }
    }
}