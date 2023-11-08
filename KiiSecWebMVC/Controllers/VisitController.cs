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
using MessagePack;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using QRCoder;
using System.Drawing;
using System.Drawing.Imaging;

namespace KiiSecWebMVC.Controllers
{
    public static class BitmapExtension
    {
        public static byte[] BitmapToByteArray(this Bitmap bitmap)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                bitmap.Save(ms, ImageFormat.Png);
                return ms.ToArray();
            }
        }
    }
    public class VisitController : Controller
    {
        KiiSecAPI _api = new KiiSecAPI();
        private readonly UserManager<IdentityUser> _userManager;

        public class VisitModel
        {
            public VisitModel(Visit visit)
            {
                ID = visit.ID;
                Organization = KiiSecAPI.GetOrganizations().Result.Where(i => i.ID == visit.OrganizationID).FirstOrDefault().Name;
                Employee employee = KiiSecAPI.GetEmployee(visit.EmployeeID).Result;
                Employee = $"{employee.LastName} {employee.FirstName} {employee.MiddleName}";
                VisitDate = visit.VisitDate;
                VisitStatus = KiiSecAPI.GetVisitStatus().Result.Where(i => i.ID == visit.VisitStatusID).FirstOrDefault().Status;
                VisitPurpose = visit.VisitPurpose;
            }
            public int ID { get; set; } 
            public string Organization{ get; set;}
            public  string Employee { get; set;}
            public DateTime? VisitDate { get; set;}
            public string VisitPurpose { get; set;}
            public string VisitStatus { get; set;}
        }

        

        public class VisitModelForEmp
        {
            public VisitModelForEmp(Visit visit)
            {
                ID = visit.ID;
                Employee employee = KiiSecAPI.GetEmployee(visit.EmployeeID).Result;
                Employee = $"{employee.LastName} {employee.FirstName} {employee.MiddleName}";
                VisitDate = visit.VisitDate;
                ArrivalDateTime = visit.ArrivalDateTime;
                DateStart = visit.DateStart;
                DateEnd = visit.DateEnd;
                VisitStatus = KiiSecAPI.GetVisitStatus().Result.Where(i => i.ID == visit.VisitStatusID).FirstOrDefault().Status;
                VisitPurpose = visit.VisitPurpose;
            }
            public int ID { get; set; }
            public string Visitor { get; set; }
            public string Employee { get; set; }
            public DateTime? VisitDate { get; set; }
            public DateTime DateStart { get; set; }
            public DateTime DateEnd { get; set; }
            public DateTime? ArrivalDateTime { get; set; }
            public string VisitPurpose { get; set; }
            public string VisitStatus { get; set; }
        }

        public VisitController(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public VisitModel visitModel { get; set; }  

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Details(int id)
        {
            return View();
        }

        public ActionResult Delete(int id)
        {
            return View();
        }

        public async Task<List<Visit>> GetVisits()
        {
            List<Visit> visits = new List<Visit>();
            KiiSecAPI _api = new KiiSecAPI();
            HttpClient client = _api.Initial();
            var response = await client.GetAsync($"/api/Visit/");
            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                visits = JsonConvert.DeserializeObject<List<Visit>>(result);
            }
            return visits;
        }

        public async Task<Visit> GetVisit(int visitId)
        {
            Visit visits = new Visit();
            KiiSecAPI _api = new KiiSecAPI();
            HttpClient client = _api.Initial();
            var response = await client.GetAsync($"/api/Visit/{visitId}");
            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                visits = JsonConvert.DeserializeObject<Visit>(result);
            }
            return visits;
        }

        public async Task<List<VisitOfVisitor>> GetVisitsOfVisitors()
        {
            List<VisitOfVisitor> visitsOfVisitors = new List<VisitOfVisitor>();
            KiiSecAPI _api = new KiiSecAPI();
            HttpClient client = _api.Initial();
            var response = await client.GetAsync($"/api/VisitOfVisitor/");
            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                visitsOfVisitors = JsonConvert.DeserializeObject<List<VisitOfVisitor>>(result);
            }
            return visitsOfVisitors;
        }

        public async Task<List<VisitOfVisitor>> GetVisitsOfVisitor(int visitorId)
        {
            List<VisitOfVisitor> visitsOfVisitors = new List<VisitOfVisitor>();
            KiiSecAPI _api = new KiiSecAPI();
            HttpClient client = _api.Initial();
            var response = await client.GetAsync($"/api/VisitOfVisitor/visitor/{visitorId}");
            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                visitsOfVisitors = JsonConvert.DeserializeObject<List<VisitOfVisitor>>(result);
            }
            return visitsOfVisitors;
        }

        //TODO: Delete Visit, Show QR

        public async Task<ActionResult> List()
        {
            var user = await _userManager.GetUserAsync(User);
            var visitor = KiiSecAPI.GetVisitors().Result.FirstOrDefault(i => i.Email == user.Email); 
            int visitorId = visitor.ID;
            //List<Visit> visits = new List<Visit>();
            List<VisitModel> visitModels = new List<VisitModel>();
            List<VisitOfVisitor> visitsOfVisitor = GetVisitsOfVisitor(visitorId).Result;          
            foreach (var item in visitsOfVisitor)
            {
                visitModels.Add(new VisitModel(GetVisit(item.VisitId).Result));
                //visits.Add(GetVisit(item.VisitId).Result);
            }
            return View(visitModels);
        }

        public async Task<ActionResult> ListForEmp()
        {
            var user = await _userManager.GetUserAsync(User);
            var employee = KiiSecAPI.GetEmployees().Result.FirstOrDefault(i => i.Email == user.Email);
            int organizationId = employee.OrganizationId;
            //List<Visit> visits = new List<Visit>();
            List<VisitModelForEmp> visitModels = new List<VisitModelForEmp>();
            List<Visit> visits = visits = GetVisits().Result.Where(i => i.OrganizationID == organizationId).ToList();
            foreach (var item in visits)
            {
                visitModels.Add(new VisitModelForEmp(GetVisit(item.ID).Result));
                //visits.Add(GetVisit(item.VisitId).Result);
            }
            return View(visitModels);
        }

        public async Task<IActionResult> Create(Visit visit)
        {
            var user = await _userManager.GetUserAsync(User);
            var visitor = KiiSecAPI.GetVisitors().Result.FirstOrDefault(i => i.Email == user.Email);
            int visitorId = visitor.ID;
            visit.VisitStatusID = 1;
            List<Organization> organizations = KiiSecAPI.GetOrganizations().Result;
            List<SelectListItem> itemsOrg = new List<SelectListItem>();
            foreach (var item in organizations)
            {
                itemsOrg.Add(new SelectListItem { Text = item.Name, Value = item.ID.ToString() });
            }
            ViewBag.Organizations = itemsOrg;
            List<SelectListItem> itemsEmp = new List<SelectListItem>();
            List<Employee> employees = KiiSecAPI.GetEmployeesByOrganization(visit.OrganizationID).Result;
            foreach (var item in employees)
            {
                itemsEmp.Add(new SelectListItem { Text = $"{item.LastName} {item.FirstName} {item.MiddleName} | {item.Department}", Value = item.ID.ToString() });
            }
            ViewBag.Employees = itemsEmp;
            ViewBag.MinDate = DateTime.Today.ToString("yyyy-MM-dd");
            ViewBag.MaxDate = DateTime.Today.AddDays(15).ToString("yyyy-MM-dd");
            ModelState.Remove("VisitStatusId");
            ModelState.Remove("VisitsOfVisitors");
            if (ModelState.IsValid)
            {
                HttpClient client = _api.Initial();
                string json = JsonConvert.SerializeObject(visit);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var responseV = client.PostAsync("api/Visit", content).Result;
                List<Visit> visits = GetVisits().Result;
                var visitId = visits.Where(v => v.ID != visit.ID).Select(v => v.ID).FirstOrDefault();
                var responseVoV = client.PostAsync($"/api/VisitOfVisitor?visitorId={visitorId}&visitId={visitId}", null).Result;
                if (responseV.IsSuccessStatusCode && responseVoV.IsSuccessStatusCode)
                {
                    return Redirect("~/Home/Success"); // TODO Change to RedirectToACtion
                }
                return Redirect("~/Home/Fail");
            }
            return View(visit);
        }

        public async Task<IActionResult> Edit(Visit visit)
        {
            HttpClient client = _api.Initial();
            HttpResponseMessage response;
            List<VisitStatus> visitStatuses = KiiSecAPI.GetVisitStatus().Result;
            List<SelectListItem> itemsVS = new List<SelectListItem>();
            foreach (var item in visitStatuses)
            {
                itemsVS.Add(new SelectListItem { Text = item.Status, Value = item.ID.ToString() });
            }
            ViewBag.VisitStatuses = itemsVS;
            ModelState.Remove("VisitsOfVisitors");
            if (ModelState.IsValid)
            {
                string json = JsonConvert.SerializeObject(visit);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                response = client.PutAsync($"api/Visit/{visit.ID}", content).Result;
                if (response.IsSuccessStatusCode)
                {
                    return Redirect("~/Home/Success");
                }
                return Redirect("~/Home/Fail");
            }
            response = await client.GetAsync($"/api/Visit/{visit.ID}");
            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                visit = JsonConvert.DeserializeObject<Visit>(result);
            }
            return View(visit);
        }

        public async Task<IActionResult> GenerateQR(Visit visit)
        {
            HttpClient client = _api.Initial();
            HttpResponseMessage response;
            QRCodeGenerator QrGenerator = new QRCodeGenerator();
            QRCodeData QrCodeInfo = QrGenerator.CreateQrCode(visit.ID.ToString(), QRCodeGenerator.ECCLevel.Q);
            QRCode QrCode = new QRCode(QrCodeInfo);
            Bitmap QrBitmap = QrCode.GetGraphic(60);
            byte[] BitmapArray = QrBitmap.BitmapToByteArray();
            string QrUri = string.Format("data:image/png;base64,{0}", Convert.ToBase64String(BitmapArray));
            ViewBag.QrCodeUri = QrUri;

            response = await client.GetAsync($"/api/Visit/{visit.ID}");
            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                visit = JsonConvert.DeserializeObject<Visit>(result);
            }
            return View(visit);
        }

        
    }
}
