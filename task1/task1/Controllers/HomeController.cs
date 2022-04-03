using FastMember;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using models.BaseClass;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;


namespace task1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        //Bu kısımda controller üzerinde belirtilen url'e günümüz tarihini parametre olarak yollayarak get request oluşturup,
        //Gelen veri üzerinde linq kullanarak değişiklikler yapıyoruz.Son olarak oluşturduğumuz listeyi view'e dönüyoruz.
        public async Task<IActionResult> Index()
        {
            var responseObj = new Result();
            using (var client = new HttpClient())
            {
                var dts = DateTime.Now;
                var requestParams = $"?endDate={dts.Year}-{dts.Month}-{dts.Day}&startDate={dts.Year}-{dts.Month}-{dts.Day}";
                var baseUrl = "https://seffaflik.epias.com.tr/transparency/service/market/intra-day-trade-history";

                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = new HttpResponseMessage();

                response = await client.GetAsync(baseUrl + requestParams).ConfigureAwait(false);

                if (response.IsSuccessStatusCode)
                {

                    string result = response.Content.ReadAsStringAsync().Result;
                    responseObj = JsonConvert.DeserializeObject<Result>(result);
                }
            }
            var list = responseObj.body.intraDayTradeHistoryList;
            //Başında pb olmayan conractlar
            var conracts = list.Where(x => !x.Conract.StartsWith("PB")).Select(x => x.Conract).Distinct().ToList();
            var table = new List<TableObject>();
            foreach (var conract in conracts)
            {
                //bu conractların aynı isimli olanları
                var currentConractList = list.Where(x => x.Conract == conract).ToList();

                var tableObject = new TableObject();
                //tablo içinde dönüyoruz, istenilen kısımları hesaplıyoruz
                foreach (var currentConract in currentConractList)
                {
                    tableObject.TotalProcessPrice += (currentConract.Price * currentConract.Quantity) / 10;
                    tableObject.TotalProcessCount += (currentConract.Quantity) / 10;
                }
                //son olarak oluşan datalarla ortalamayı hesaplıyoruz
                tableObject.AvaragePrice = tableObject.TotalProcessPrice / tableObject.TotalProcessCount;
                //conractın ismini substringlere bölerek ilgili formattan parse ediyoruz
                tableObject.DateTime = new DateTime(year: Int32.Parse(conract.Substring(2, 2)), month: Int32.Parse(conract.Substring(4, 2)), day: Int32.Parse(conract.Substring(6, 2)), hour: Int32.Parse(conract.Substring(8, 2)), minute: 0, second: 0);
                //oluşan objeleri bir tabloda topluyoruz
                table.Add(tableObject);
            }
            //bu tabloyu view a dönüyoruz
            return View(table);
        }
    }
}
