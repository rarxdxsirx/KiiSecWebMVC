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

namespace KiiSecWebMVC.Controllers
{
    public class EmployeesController : Controller
    {

        public class EmployeeModel
        {
            public string Email { get; set; }
            public int OrganizationId { get; set; }
        }

        public class EmployeePermissionModel
        {
            public int ID { get; set; }
            public int OrganizationId { get; set; }
            public string Email { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string? MiddleName { get; set; }
            public bool ValidateRequestInfo { get; set; }
            public bool SetVisitDate { get; set; }
            public bool UpdateVisitStatus { get; set; }
            public bool MarkArrival { get; set; }
        }

        KiiSecAPI _api = new KiiSecAPI();
        private readonly UserManager<IdentityUser> _userManager;

        static public EmployeeModel employeeModel { get; set; } = new EmployeeModel();

        public EmployeesController(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> List(int organizationId)
        {
            var user = await _userManager.GetUserAsync(User);
            var organization = KiiSecAPI.GetOrganizations().Result.FirstOrDefault(o => o.Email == user.Email);
            if (organizationId == 0 && (organization.Email == user.Email))
            {
                organizationId = organization.ID;
            }
            return View(KiiSecAPI.GetEmployeesByOrganization(organizationId).Result);
        }

        public async Task<IActionResult> Create(string email, int organizationId, Employee employee)
        {
            if (email != null)
            {
                employeeModel.Email = email;
                employeeModel.OrganizationId = organizationId;
            }
            var user = await _userManager.FindByEmailAsync(employeeModel.Email);
            if (user == null)
            {
                return NotFound($"Пользователь с почтой '{employeeModel.Email}' не найден.");
            }

            employee.Email = employeeModel.Email;
            employee.OrganizationId = employeeModel.OrganizationId;
            ModelState.Remove("Email");
            ModelState.Remove("OrganizationId");

            if (ModelState.IsValid)
            {
                HttpClient client = _api.Initial();
                string json = JsonConvert.SerializeObject(employee);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = client.PostAsync("api/Employee", content).Result;
                if (response.IsSuccessStatusCode)
                {
                    return Redirect("~/Home/Success"); // TODO Change to RedirectToACtion
                }
                return Redirect("~/Home/Fail");
            }
            return View();
        }

        public async Task<IActionResult> Details(int id)
        {
            IdentityUser user = await _userManager.GetUserAsync(User);
            Employee employee = new Employee();
            HttpClient client = _api.Initial();
            HttpResponseMessage response = await client.GetAsync($"/api/Employee/{id}");
            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                employee = JsonConvert.DeserializeObject<Employee>(result);
            }
            return View(employee);
        }

        public async Task<IActionResult> Edit(Employee employee)
        {
            HttpClient client = _api.Initial();
            HttpResponseMessage response;
            if (ModelState.IsValid)
            {
                string json = JsonConvert.SerializeObject(employee);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                response = client.PutAsync("api/Employee", content).Result;
                if (response.IsSuccessStatusCode)
                {
                    return Redirect("~/Home/Success");
                }
                return Redirect("~/Home/Fail");
            }
            response = await client.GetAsync($"/api/Employee/{employee.ID}");
            if (response.IsSuccessStatusCode) 
            {
                var result = response.Content.ReadAsStringAsync().Result;
                employee = JsonConvert.DeserializeObject<Employee>(result);
            }           
            return View(employee);
        }

        public async Task<Employee> GetEmployee(int employeeId)
        {
            Employee employee = new Employee();
            HttpClient client = _api.Initial();
            var response = await client.GetAsync($"/api/Employee/{employeeId}");
            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                employee = JsonConvert.DeserializeObject<Employee>(result);
            }
            return employee;
        }

        public async Task<Permission> GetPermission(int permissionId)
        {
            Permission permission = new Permission();
            HttpClient client = _api.Initial();
            var response = await client.GetAsync($"/api/Permission");
            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                permission = JsonConvert.DeserializeObject<List<Permission>>(result).FirstOrDefault(p => p.ID == permissionId); 
            }
            return permission;
        }

        public async Task<HttpResponseMessage> AddEmployeePermission(int employeeId, int permissionId)
        {
            HttpClient client = _api.Initial();
            var response = await client.PostAsync($"/api/EmployeePermission?employeeId={employeeId}&permissionId={permissionId}", null);
            return response;
        }

        public async Task<HttpResponseMessage> RemoveEmployeePermission(int employeeId, int permissionId)
        {
            HttpClient client = _api.Initial();
            EmployeePermissions ep = new EmployeePermissions()
            {
                Employee = GetEmployee(employeeId).Result,
                EmployeeID = employeeId,
                Permission = GetPermission(permissionId).Result,
                PermissionID = permissionId
            };
            string json = JsonConvert.SerializeObject(ep);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.DeleteAsync($"/api/EmployeePermission?employeeId={employeeId}&permissionId={permissionId}");
            return response;
        }

        public async Task<bool> employeeHavePermission(List<EmployeePermissions> employeePermissions, int employeeId, int permissionId)
        {
            if (employeePermissions.Where(e => e.EmployeeID == employeeId).Where(p => p.PermissionID == permissionId).FirstOrDefault() == null)
            {
                return false;
            }
            return true;
        }

        public async Task<IActionResult> EditPermissions(EmployeePermissionModel employeeModel)
        {
            HttpClient client = _api.Initial();
            HttpResponseMessage response;
            List<EmployeePermissions> employeePermissions;
            response = await client.GetAsync($"/api/EmployeePermission/employee/{employeeModel.ID}");
            if (!response.IsSuccessStatusCode)
            {
                return Redirect("~/Home/Fail");
            }
            var result = response.Content.ReadAsStringAsync().Result;
            employeePermissions = JsonConvert.DeserializeObject<List<EmployeePermissions>>(result);
            if (ModelState.IsValid)
            {
                List<int> permissionsIds = new List<int>();
                if (employeeModel.UpdateVisitStatus)
                {
                    permissionsIds.Add(1);
                }
                if (employeeModel.SetVisitDate)
                {
                    permissionsIds.Add(2);               
                }
                if (employeeModel.ValidateRequestInfo)
                {
                    permissionsIds.Add(3);
                }
                if (employeeModel.MarkArrival)
                {
                    permissionsIds.Add(3);
                }
                foreach (var item in permissionsIds)
                {
                    if (!employeeHavePermission(employeePermissions, employeeModel.ID, item).Result)
                    {
                        await AddEmployeePermission(employeeModel.ID, item);
                    }
                }
                foreach(var item in employeePermissions)
                {
                    if (!permissionsIds.Contains(item.PermissionID))
                    {
                        await RemoveEmployeePermission(employeeModel.ID, item.PermissionID);
                    }
                }
            }
            response = await client.GetAsync($"/api/Employee/{employeeModel.ID}");
            if (response.IsSuccessStatusCode)
            {
                result = response.Content.ReadAsStringAsync().Result;
                var employee = JsonConvert.DeserializeObject<Employee>(result);
                var permissionsIds = employeePermissions.Select(i  => i.PermissionID).ToArray();
                if (employee != null)
                {
                    employeeModel.Email = employee.Email;
                    employeeModel.LastName = employee.LastName;
                    employeeModel.FirstName = employee.FirstName;
                    employeeModel.MiddleName = employee.MiddleName;
                    employeeModel.OrganizationId = employee.OrganizationId;
                    if (permissionsIds.Contains(1))
                    {
                        employeeModel.UpdateVisitStatus = true;
                    }
                    if (permissionsIds.Contains(2))
                    {
                        employeeModel.SetVisitDate = true;
                    }
                    if (permissionsIds.Contains(3))
                    {
                        employeeModel.ValidateRequestInfo = true;
                    }
                    if (permissionsIds.Contains(4))
                    {
                        employeeModel.MarkArrival = true;
                    }
                }
            }
            return View(employeeModel);
        }

        public async Task<IActionResult> Delete(Employee employee)
        {
            HttpClient client = _api.Initial();
            HttpResponseMessage response;
            if (ModelState.IsValid)
            {
                string json = JsonConvert.SerializeObject(employee);
                var user = _userManager.Users.ToList().FirstOrDefault(u => u.Email == employee.Email);
                var response2 = await _userManager.DeleteAsync(user);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                response = client.DeleteAsync($"api/Employee/{employee.ID}").Result;
                if (response.IsSuccessStatusCode && response2.Succeeded)
                {
                    return Redirect("~/Home/Success");
                }
                return Redirect("~/Home/Fail");
            }
            response = await client.GetAsync($"/api/Employee/{employee.ID}");
            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                employee = JsonConvert.DeserializeObject<Employee>(result);
            }
            return View(employee);
        }
    }
}
