using ClientView.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.Text;
using TestProject.Dto;
using TestProject.Models;

namespace ClientView.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private string BaseUrl = "https://localhost:44301/api";

        private List<Product> products = new List<Product>();

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> Index(string name)
        {
            using (var client = new HttpClient())
            {
                PrepareClientHttp(client);
                var response = await client.GetAsync("api/MyProducts/" + name);

                if (response.IsSuccessStatusCode)
                {
                    var result = response.Content.ReadAsStringAsync().Result;
                    products = JsonConvert.DeserializeObject<List<Product>>(result);
                }
            }

            return View(products);
        }

        public async Task<IActionResult> Search(string name)
        {
            return await Index(name);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteProduct(Guid guid)
        {
            using (var client = new HttpClient())
            {
                PrepareClientHttp(client);

                var response = await client.DeleteAsync("api/MyProducts/" + guid);

                if (response.IsSuccessStatusCode)
                {
                    var result = response.Content.ReadAsStringAsync().Result;
                    Console.WriteLine(result);
                }
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> AddProduct(string productName, string productDescription)
        {
            using (var client = new HttpClient())
            {
                PrepareClientHttp(client);

                var product = new Product()
                {
                    Name = productName,
                    Description = productDescription
                };

                var json = JsonConvert.SerializeObject(product);

                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync("api/MyProducts/", content);

                if (response.IsSuccessStatusCode)
                {
                    var result = response.Content.ReadAsStringAsync().Result;
                    Console.WriteLine(result);
                }
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> EditProduct(Guid guid, string editName, string editDescription)
        {
            using (var client = new HttpClient())
            {
                PrepareClientHttp(client);

                var product = await GetProductAsync(guid);
                product.Name = editName;
                product.Description = editDescription;

                var json = JsonConvert.SerializeObject(product);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PutAsync("api/MyProducts/" + product.ID, content);

                if (response.IsSuccessStatusCode)
                {
                    var result = response.Content.ReadAsStringAsync().Result;
                    Console.WriteLine(result);
                }
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task<Product> GetProductAsync(Guid guid)
        {
            var product = new Product();

            using (var client = new HttpClient())
            {
                PrepareClientHttp(client);
                var response = await client.GetAsync("api/MyProducts/guid?guid=" + guid);

                if (response.IsSuccessStatusCode)
                {
                    var result = response.Content.ReadAsStringAsync().Result;
                    product = JsonConvert.DeserializeObject<Product>(result);
                }
            }

            return product;
        }

        private void PrepareClientHttp(HttpClient client)
        {
            client.BaseAddress = new Uri(BaseUrl);
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
