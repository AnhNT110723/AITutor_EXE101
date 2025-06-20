using EXE_FAIEnglishTutor.Models;
using EXE_FAIEnglishTutor.Services.Interface.Mentee;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Configuration;
using System.Diagnostics;
using System.Text;

namespace EXE_FAIEnglishTutor.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ISituationService _situationService;
        private readonly IConfiguration _configuration;
        private readonly string _translatorEndpoint;
        private readonly string _apiKey;
        private readonly string _region;
        public HomeController(ILogger<HomeController> logger, ISituationService situationService, IConfiguration configuration)
        {
            _logger = logger;
            _situationService = situationService;
            _configuration = configuration;
            _translatorEndpoint = _configuration["AzureTranslator:Endpoint"] + "translate?api-version=3.0";
            _apiKey = _configuration["AzureTranslator:ApiKey"];
            _region = _configuration["AzureTranslator:Region"];
        }

        [Route("/Mentee")]
        public async Task<IActionResult> Index()
        {
            var situations = await _situationService.GetAllSituation();
            return View(situations);
        }


        public IActionResult homePage()
        {

            return View("homepage");
        }


        [HttpPost]
        public async Task<IActionResult> Translate(TranslationViewModel model)
        {
            if (string.IsNullOrEmpty(model.Text) || model.SourceLanguage == model.TargetLanguage)
            {
                ModelState.AddModelError("", "Văn bản không hợp lệ hoặc ngôn ngữ nguồn và đích giống nhau.");
                return View("Index", model);
            }

            try
            {
                using (var client = new HttpClient())
                {
                    var requestUri = $"{_translatorEndpoint}&from={model.SourceLanguage}&to={model.TargetLanguage}";
                    var requestBody = new[] { new { Text = model.Text } };
                    var jsonRequest = JsonConvert.SerializeObject(requestBody);

                    var request = new HttpRequestMessage
                    {
                        Method = HttpMethod.Post,
                        RequestUri = new Uri(requestUri),
                        Content = new StringContent(jsonRequest, Encoding.UTF8, "application/json")
                    };

                    request.Headers.Add("Ocp-Apim-Subscription-Key", _apiKey);
                    request.Headers.Add("Ocp-Apim-Subscription-Region", _region);

                    var response = await client.SendAsync(request);
                    if (response.IsSuccessStatusCode)
                    {
                        var jsonResponse = await response.Content.ReadAsStringAsync();
                        var result = JsonConvert.DeserializeObject<dynamic>(jsonResponse);
                        model.TranslatedText = result[0].translations[0].text;
                    }
                    else
                    {
                        model.TranslatedText = $"Lỗi khi dịch: {response.StatusCode}. Vui lòng thử lại.";
                    }
                }
            }
            catch (Exception ex)
            {
                model.TranslatedText = $"Lỗi: {ex.Message}";
            }

            return View("Index", model);
        }
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

