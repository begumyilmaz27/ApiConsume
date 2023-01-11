using Api_Consume.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Api_Consume.Controllers
{
    public class DefaultController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public DefaultController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Index()
        {
            var client = _httpClientFactory.CreateClient();
            var responseMessage = await client.GetAsync("https://localhost:44397/api/Category");

            if (responseMessage.StatusCode == System.Net.HttpStatusCode.OK)
            {
                //content'in içeriğini string formatta okusun demek.
                var jsondata = await responseMessage.Content.ReadAsStringAsync();
                //Json dosyasını nesneye çevirmemizi sağlayacak Deserialize;
                var result = JsonConvert.DeserializeObject<List<CategoryViewModel>>(jsondata);
                return View(result);
            }
            else
            {
                ViewBag.responseMessage = "Bir hata oluştu";
                return View();
            }
            
        }
        [HttpGet]
        public IActionResult AddCategory()
        {
            return View();
        }
        [HttpPost]
        public async Task< IActionResult > AddCategory(CategoryViewModel p)
        {
            p.Status = true; //ekleme işlemi yapmadan önce parametre ile atama yapmak için kullanılıyor. 
            var client = _httpClientFactory.CreateClient(); //istek oluşturduk.
            var jsonData= JsonConvert.SerializeObject(p); //serilize ediyoruz çünkü LİSTELEME YAPMIYORUZ. Kendimiz veri gönderiyoruz. 
            StringContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");
            var responseMessage = await client.PostAsync("https://localhost:44397/api/Category", content);
            if (responseMessage.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.responseMessage = "Bir hata ile karşılaşıldı";
                return View();
            }
            
        }

        [HttpGet]
        public async Task<IActionResult> UpdateCategory(int id)
        {
            var client = _httpClientFactory.CreateClient();
            var responseMessage = await client.GetAsync($"https://localhost:44397/api/Category/{id}");
            if (responseMessage.IsSuccessStatusCode) 
            {
                var jsonData = await responseMessage.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<CategoryViewModel>(jsonData); //VERİ LİSTELEMESİ YAPACAĞIZ BU YÜZDEN YENİDEN DESERİALİZE KULLANIYORUZ.
                    //NİYE LİST KULLANMADIK YUKARIDAKİ INDEX METODUNDA OLDUĞU GİBİ?
                    //ÇÜNKÜ BURADA 1 TANE VERİ DÖNECEK. 
                return View(result);
            }
            else
            {
                ViewBag.responseMessage = "Bir hata ile karşılaşıldı";
                return View();
            }
        }
        [HttpPost]
        public async Task<IActionResult> UpdateCategory(CategoryViewModel p)
        {
            
            var client = _httpClientFactory.CreateClient();
            var jsonData = JsonConvert.SerializeObject(p);  //GÜNCELLENECEK YENİ VERİLERİ GÖNDERDİĞİMİZ İÇİN SERİALİZE KULLANIYORUZ. 
            StringContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");
            var responseMessage = await client.PutAsync("https://localhost:44397/api/Category", content); 

            //VERİ EKLEME OLSUN İSTEDİĞİMİZ İÇİN PUTASYNC KULLANDIK.

            if (responseMessage.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
            return View();
        }
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var client = _httpClientFactory.CreateClient();
            await client.DeleteAsync($"https://localhost:44377/api/Category/{id}");
            return RedirectToAction("Index");
        }



    }
}
