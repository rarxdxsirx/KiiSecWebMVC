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
    }
}
