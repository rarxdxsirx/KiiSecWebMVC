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
    }
}
