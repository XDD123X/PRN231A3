using BusinessObject;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;

namespace eStore.Controllers
{
    public class StartController : Controller
    {


        private readonly HttpClient client = null;
        private string ProductApiUrl = "";

        public StartController()
        {
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            ProductApiUrl = "https://localhost:7064/api/ProductAPI";
        }
        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public async Task<ActionResult> ReturnToProductPage()
        {
            HttpResponseMessage httpResponseMessage = await client.GetAsync(ProductApiUrl);
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                List<Product> products = new List<Product>();
                products = await httpResponseMessage.Content.ReadFromJsonAsync<List<Product>>();
                return View("Index", products);
            }
            else
            {
                return View("Index");
            }



        }



    }
}
