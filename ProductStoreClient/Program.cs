using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ProductStoreClient
{
    class Program
    {
        static void Main()
        {
            while (true)
            {
                RunAsync().Wait();
                System.Threading.Thread.Sleep(5000);
            }
           
        }

        static async Task RunAsync()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://192.168.1.31:8081/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                try
                {
                    //HttpResponseMessage response = await DoGet(client, "api/products/1");

                    // HTTP POST
                    var gizmo = new Product() { Name = "Gizmo", Price = 100, Category = "Widget" };
                    HttpResponseMessage response = await client.PostAsJsonAsync("api/products", gizmo);
                    response.EnsureSuccessStatusCode();

                    Uri gizmoUrl = response.Headers.Location;

                    response = await DoGet(client, gizmoUrl.ToString());

                    // HTTP PUT
                    gizmo.Price = 80;   // Update price
                    response = await client.PutAsJsonAsync(gizmoUrl, gizmo);

                    response = await DoGet(client, gizmoUrl.ToString());

                    // HTTP DELETE
                    response = await client.DeleteAsync(gizmoUrl);

                    // expect 404
                    response = await DoGet(client, gizmoUrl.ToString());

                }
                catch (HttpRequestException e)
                {
                    Console.WriteLine("exception: " + e);
                }

            }
        }

        private static async Task<HttpResponseMessage> DoGet(HttpClient client, String uri)
        {
            // HTTP GET
            HttpResponseMessage response = await client.GetAsync(uri);
            response.EnsureSuccessStatusCode();

            Product product = await response.Content.ReadAsAsync<Product>();
            Console.WriteLine("{0}\t${1}\t{2}", product.Name, product.Price, product.Category);
            return response;
        }
    }
}
