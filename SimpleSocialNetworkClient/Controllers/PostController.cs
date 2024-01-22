using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NuGet.Protocol;
using SimpleSocialNetworkShared;
using System.Net.Http;
using System.Text;

namespace SimpleSocialNetworkClient.Controllers
{
    public class PostController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public PostController(IHttpClientFactory httpClientFactory, IWebHostEnvironment webHostEnvironment, IHttpContextAccessor httpContextAccessor)
        {
            _httpClientFactory = httpClientFactory;
            _webHostEnvironment = webHostEnvironment;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync("http://localhost:5012/api/Post/GetAllPosts");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var postViewModel = JsonConvert.DeserializeObject<List<PostViewModel>>(json);
                //SimpleSocialNetworkAPI
                //string currentPath = AppContext.BaseDirectory;
                //string parentPath = Path.Combine(currentPath, "..", "..", "..");
                //string basePath = Path.GetFullPath(parentPath);

                //ViewBag.MediaPath = fullPath;

                return View(postViewModel);
            }

            return NotFound();
        }

        [HttpGet]
        public async Task<IActionResult> CreatePost()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreatePost(PostViewModel postViewModel)
        {
            if (postViewModel.Content == null && postViewModel.MediaFile == null)
            {
                var emptyModel = new ResponseManager
                {
                    IsSuccess = false,
                    Message = "You can not create an empty post"
                };
                ViewBag.Response = emptyModel;

                return View();
            }
            else if (postViewModel.Content != null && postViewModel.MediaFile == null)
            {
                ResponseManager? responseObject = await CreatePostApi(postViewModel);

                ViewBag.Response = responseObject;

                return View();

            }
            else if (postViewModel.Content == null && postViewModel.MediaFile != null)
            {
                postViewModel = await StoreMedia(postViewModel);

                ResponseManager? responseObject = await CreatePostApi(postViewModel);

                ViewBag.Response = responseObject;

                return View();
            }
            else
            {
                postViewModel = await StoreMedia(postViewModel);

                ResponseManager? responseObject = await CreatePostApi(postViewModel);

                ViewBag.Response = responseObject;

                return View();
            }
        }

        [HttpPut]
        public async Task<string> Like(int postId)
        {
            var client = _httpClientFactory.CreateClient();
            var token = _httpContextAccessor.HttpContext.Session.GetString("JWTtoken");
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

            var response = await client.PutAsync($"http://localhost:5012/api/Post/LikePost?postId={postId}", null);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }

            return null;
        }

        [HttpPut]
        public async Task<CommentViewModel> Comment(int postId, string comment)
        {
            var client = _httpClientFactory.CreateClient();
            var token = _httpContextAccessor.HttpContext.Session.GetString("JWTtoken");
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

            var response = await client.PutAsync($"http://localhost:5012/api/Post/AddComment?postId={postId}&comment={comment}", null);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var commentViewModel = JsonConvert.DeserializeObject<CommentViewModel>(json);
                return commentViewModel;
            }

            return null;
        }

        [HttpGet]
        public async Task<IActionResult> Post(int postId)
        {
            var client = _httpClientFactory.CreateClient();
            var token = _httpContextAccessor.HttpContext.Session.GetString("JWTtoken");
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

            var response = await client.GetAsync($"http://localhost:5012/api/Post/GetPost?id={postId}");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var postViewModel = JsonConvert.DeserializeObject<PostViewModel>(json);
                return View(postViewModel);
            }

            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> EditPost(PostViewModel post)
        {
            var client = _httpClientFactory.CreateClient();
            var token = _httpContextAccessor.HttpContext.Session.GetString("JWTtoken");
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

            var jsonData = JsonConvert.SerializeObject(post);
            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

            var response = await client.PutAsync($"http://localhost:5012/api/Post/UpdatePost", content);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var postViewModel = JsonConvert.DeserializeObject<PostViewModel>(json);
                return RedirectToAction("Post", new { postId = postViewModel.Id });
            }


            return NotFound();
        }

        [HttpDelete]
        public async Task<IActionResult> DeletePost(int postId)
        {
            var client = _httpClientFactory.CreateClient();
            var token = _httpContextAccessor.HttpContext.Session.GetString("JWTtoken");
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

            var response = await client.DeleteAsync($"http://localhost:5012/api/Post/DeletePost?postId={postId}");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var responseManager = JsonConvert.DeserializeObject<ResponseManager>(json);
                return Ok(responseManager);
            }


            return NotFound();
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteComment(int commentId)
        {
            var client = _httpClientFactory.CreateClient();
            var token = _httpContextAccessor.HttpContext.Session.GetString("JWTtoken");
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

            var response = await client.DeleteAsync($"http://localhost:5012/api/Post/DeleteComment?commentId={commentId}");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var responseManager = JsonConvert.DeserializeObject<ResponseManager>(json);
                return Ok(responseManager);
            }


            return BadRequest();
        }

        private async Task<PostViewModel> StoreMedia(PostViewModel postViewModel)
        {
            string mediaFolder = "Media\\";
            mediaFolder += Guid.NewGuid().ToString() + "_" + postViewModel.MediaFile.FileName;

            postViewModel.MediaPath = mediaFolder;

            string serverFolder = Path.Combine(_webHostEnvironment.WebRootPath, mediaFolder);
            await postViewModel.MediaFile.CopyToAsync(new FileStream(serverFolder, FileMode.Create));

            postViewModel.MediaPath = mediaFolder;

            return postViewModel;
        }

        private async Task<ResponseManager?> CreatePostApi(PostViewModel postViewModel)
        {
            var client = _httpClientFactory.CreateClient();
            var token = _httpContextAccessor.HttpContext.Session.GetString("JWTtoken");
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

            var jsonData = JsonConvert.SerializeObject(postViewModel);
            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
            var response = await client.PostAsync("http://localhost:5012/api/Post/CreatePost", content);
            var responseBody = await response.Content.ReadAsStringAsync();
            var responseObject = JsonConvert.DeserializeObject<ResponseManager>(responseBody);
            return responseObject;
        }
    }
}
