using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SimpleSocialNetworkClient.Models;
using SimpleSocialNetworkShared;
using System.Diagnostics;
using System.Net.Http;
using System.Text;

namespace SimpleSocialNetworkClient.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HomeController(ILogger<HomeController> logger, IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterDTO registerDTO)
        {
            var responseObject = await ConsumePostApi(registerDTO, "api/Auth/Register");

            ViewBag.Response = responseObject;

            return View();
        }


        [HttpGet]
        public IActionResult Login() 
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginDTO loginDTO)
        {
            var responseObject = await ConsumePostApi(loginDTO, "api/Auth/Login");

            ViewBag.Response = responseObject;

            if (responseObject.IsSuccess)
            {
                _httpContextAccessor.HttpContext.Session.SetString("JWTtoken", responseObject.Message);
                return RedirectToAction("Index", "Post");
            }

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            _httpContextAccessor.HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpGet]
        public IActionResult ForgetPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgetPassword(string email)
        {
            var client = _httpClientFactory.CreateClient();

            var response = await client.PostAsync($"http://localhost:5012/api/Auth/ForgetPassword?email={email}", new StringContent(email));
            var respnseBody = await response.Content.ReadAsStringAsync();
            var responseObject = JsonConvert.DeserializeObject<ResponseManager>(respnseBody);

            if (!responseObject.IsSuccess)
            {
                return RedirectToAction("ForgetPassword");
            }
            return RedirectToAction("Login");
        }

        private async Task<ResponseManager?> ConsumePostApi<T>(T registerDTO, string path)
        {
            var client = _httpClientFactory.CreateClient();

            var jsonData = JsonConvert.SerializeObject(registerDTO);
            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
            var response = await client.PostAsync("http://localhost:5012/" + path, content);
            var responseBody = await response.Content.ReadAsStringAsync();
            var responseObject = JsonConvert.DeserializeObject<ResponseManager>(responseBody);
            return responseObject;
        }
    }
}