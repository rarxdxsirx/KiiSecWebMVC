﻿using KiiSec.Models;
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

namespace KiiSecWebMVC.Controllers
{
    public class OrganizationController : Controller
    {

        KiiSecAPI _api = new KiiSecAPI();

        public IActionResult Index()
        {
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

        [Authorize(Roles = "Admin, Organization")]
        public async Task<IActionResult> Register(Organization organization) 
        {
            if (ModelState.IsValid)
            {
                HttpClient client = _api.Initial();
                string json = JsonConvert.SerializeObject(organization);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = client.PostAsync("api/Organization", content).Result;
                if (response.IsSuccessStatusCode)
                {
                    return Redirect("/Success");
                }
                return Redirect("/Fail");
                //TODO MB LATER....... NEDEED???
                //var result = "Произошла неизвестная ошибка!";
                //if (response.StatusCode == HttpStatusCode.UnprocessableEntity)
                //{
                //    result = "Такая организация уже существует!";
                //}
                //return RedirectToAction("Fail", "Organization", result);
            }
            return View();
        }

        [Authorize(Roles = "Admin, Organization")]
        public async Task<IActionResult> Details(int organizationId)
        {
            //TODO Check for valid organization

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
                    return Redirect("/Success");
                }
                return Redirect("/Fail");
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