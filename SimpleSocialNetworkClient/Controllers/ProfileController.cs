using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SimpleSocialNetworkShared;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Security.Claims;

namespace SimpleSocialNetworkClient.Controllers
{
    public class ProfileController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProfileController(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor, IWebHostEnvironment webHostEnvironment)
        {
            _httpClientFactory = httpClientFactory;
            _httpContextAccessor = httpContextAccessor;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        public async Task<IActionResult> UserProfile(string id)
        {
            var client = _httpClientFactory.CreateClient();

            var response = await client.GetAsync($"http://localhost:5012/api/Profile/GetUserProfileById?userId={id}");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var userViewModel = JsonConvert.DeserializeObject<ApplicationUserViewModel>(json);
                return View(userViewModel);
            }
            return NotFound();
        }
        [HttpGet]
        public async Task<IActionResult> MyProfile(string tokenString)
        {
            if (!string.IsNullOrEmpty(tokenString))
            {
                var handler = new JwtSecurityTokenHandler();
                var token = handler.ReadJwtToken(tokenString);
                var claims = token.Claims;
                var userIdClaim = claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

                if (userIdClaim != null)
                {
                    string id = userIdClaim.Value;
                    return RedirectToAction("UserProfile", new { id = id });
                }
            }
            return NotFound();
        }
        
        [HttpPost]
        public async Task<IActionResult> ChangeProfileImage(IFormFile file, string userId)
        {
            if (file != null && file.Length > 0)
            {
                var client = _httpClientFactory.CreateClient();
                var token = _httpContextAccessor.HttpContext.Session.GetString("JWTtoken");
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

                string mediaFolder = "Media\\";
                mediaFolder += Guid.NewGuid().ToString() + "_" + file.FileName;

                string serverFolder = Path.Combine(_webHostEnvironment.WebRootPath, mediaFolder);
                using (FileStream fileStream = new FileStream(serverFolder, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream); 
                }
                

                var response = await client.PutAsync($"http://localhost:5012/api/Profile/ChangeProfileImage?path={mediaFolder}&userId={userId}", null);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    serverFolder = Path.Combine(_webHostEnvironment.WebRootPath, result);
                    if (System.IO.File.Exists(serverFolder))
                    {
                        await Task.Run(() => System.IO.File.Delete(serverFolder));
                        return RedirectToAction("MyProfile", new { tokenString = token });
                    }
                }
            }
            return BadRequest();
        }

        [HttpPost]
        public async Task<IActionResult> DeleteProfile(string userId)
        {
            if (!string.IsNullOrEmpty(userId))
            {
                var client = _httpClientFactory.CreateClient();
                var token = _httpContextAccessor.HttpContext.Session.GetString("JWTtoken");
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

                var response = await client.DeleteAsync($"http://localhost:5012/api/Profile/DeleteUser?userId={userId}");

                if (response.IsSuccessStatusCode) 
                {
                    return RedirectToAction("Index", "Post");
                }
            }
            return BadRequest();
        }
    }
}
