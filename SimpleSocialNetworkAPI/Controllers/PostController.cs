using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SimpleSocialNetworkAPI.Model;
using SimpleSocialNetworkAPI.Services.PostService;
using SimpleSocialNetworkShared;

namespace SimpleSocialNetworkAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly IPostService _postService;

        public PostController(IPostService postService)
        {
            _postService = postService;
        }

        [HttpPost]
        public async Task<IActionResult> CreatePost(PostViewModel post)
        {
            var result = await _postService.CreatePost(post);
            if (result == null)
            {
                return BadRequest(result);
            }
            else if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPosts()
        {
            var result = await _postService.GetAllPosts();
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetPost(int id)
        {
            var result = await _postService.GetPost(id);
            if (result == null)
            {
                return NotFound("Post not found");
            }
            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> UpdatePost(PostViewModel post)
        {
            var result = await _postService.UpdatePost(post);
            if (result == null)
            {
                return NotFound("Post not found");
            }
            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> LikePost(int postId)
        {
            var result = await _postService.LikePost(postId);
            if (result == null)
            {
                return NotFound("Post or user not found");
            }
            return Ok(result.LikeCount);
        }

        [HttpDelete]
        public async Task<IActionResult> DeletePost(int postId)
        {
            var result = await _postService.DeletePost(postId);
            if (result.IsSuccess == false)
            {
                return NotFound("Post not found");
            }
            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> AddComment(int postId, string comment)
        {
            var result = await _postService.AddComment(postId, comment);
            if (result == null)
            {
                return BadRequest(result);
            }
            else
            {
                return Ok(result);
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteComment(int commentId)
        {
            var result = await _postService.DeleteComment(commentId);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }
    }
}
