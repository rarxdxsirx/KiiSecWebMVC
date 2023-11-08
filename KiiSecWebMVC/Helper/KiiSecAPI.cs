using KiiSec.Models;
using Newtonsoft.Json;
using System.Net.Http;
namespace KiiSecWeb.Helper
{
    public class KiiSecAPI
    {
        public HttpClient Initial()
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:7288/");
            return client;
        }

        static public async Task<List<Organization>> GetOrganizations()
        {
            List<Organization> organizations = new List<Organization>();
            KiiSecAPI _api = new KiiSecAPI();
            HttpClient client = _api.Initial();
            HttpResponseMessage response = await client.GetAsync("api/Organization");
            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                organizations = JsonConvert.DeserializeObject<List<Organization>>(result);
            }
            return organizations;
        }

        static public async Task<List<Employee>> GetEmployeesByOrganization(int organizationId)
        {
            List<Employee> employees = new List<Employee>();
            KiiSecAPI _api = new KiiSecAPI();
            HttpClient client = _api.Initial();
            HttpResponseMessage response = await client.GetAsync($"/api/Employee/Organization/{organizationId}");
            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                employees = JsonConvert.DeserializeObject<List<Employee>>(result);
            }
            return employees;
        }

        static public async Task<Employee> GetEmployee(int employeeId)
        {
            Employee employee = new Employee();
            KiiSecAPI _api = new KiiSecAPI();
            HttpClient client = _api.Initial();
            var response = await client.GetAsync($"/api/Employee/{employeeId}");
            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                employee = JsonConvert.DeserializeObject<Employee>(result);
            }
            return employee;
        }

        static public async Task<List<Employee>> GetEmployees()
        {
            List<Employee> employee = new List<Employee>();
            KiiSecAPI _api = new KiiSecAPI();
            HttpClient client = _api.Initial();
            var response = await client.GetAsync($"/api/Employee");
            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                employee = JsonConvert.DeserializeObject<List<Employee>>(result);
            }
            return employee;
        }

        static public async Task<List<VisitStatus>> GetVisitStatus()
        {
            List<VisitStatus> visitStatus = new List<VisitStatus>();
            KiiSecAPI _api = new KiiSecAPI();
            HttpClient client = _api.Initial();
            var response = await client.GetAsync($"/api/VisitStatus");
            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                visitStatus = JsonConvert.DeserializeObject<List<VisitStatus>>(result);
            }
            return visitStatus;
        }

        static public async Task<List<Visitor>> GetVisitors()
        {
            List<Visitor> visitors= new List<Visitor>();
            KiiSecAPI _api = new KiiSecAPI();
            HttpClient client = _api.Initial();
            HttpResponseMessage response = await client.GetAsync("api/Visitor");
            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                visitors = JsonConvert.DeserializeObject<List<Visitor>>(result);
            }
            return visitors;
        }
    }
}
