using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleSocialNetworkAPI.Model;
using SimpleSocialNetworkShared;
using System.Security.Claims;
using System.Web;

namespace SimpleSocialNetworkAPI.Services.PostService
{
    public class PostService : IPostService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PostService(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ResponseManager> CreatePost(PostViewModel post)
        {
            //ApplicationUser user = new ApplicationUser();

            var userId = _httpContextAccessor.HttpContext.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null || user.EmailConfirmed == false)
            {
                return new ResponseManager
                {
                    Message = "You must be logged in or you need to verify your account to create a post",
                    IsSuccess = false,
                };
            }

            Post postModel = new Post();
            postModel.Content = post.Content;
            postModel.MediaPath = post.MediaPath;
            postModel.User = user;

            _context.Posts.Add(postModel);
            var result = await _context.SaveChangesAsync();
            if (result > 0)
            {
                return new ResponseManager
                {
                    Message = "New post successfully created",
                    IsSuccess = true,
                };
            }
            return new ResponseManager
            {
                Message = "Can not create a new post",
                IsSuccess = false,
            };
        }

        public async Task<ResponseManager> DeletePost(int postId)
        {
            var userId = _httpContextAccessor.HttpContext.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            var post = await _context.Posts.Include(x => x.User).Include(x => x.Comments).FirstAsync(x => x.Id == postId);
            var likes = _context.Likes.Where(x => x.Post.Id == post.Id).ToList();

            if (post == null)
            {
                return new ResponseManager
                {
                    Message = "Post not found",
                    IsSuccess = false,
                };
            }
            else if (post.User.Id != userId)
            {
                return new ResponseManager
                {
                    Message = "You could not delete this post",
                    IsSuccess = false,
                };
            }
            _context.Comments.RemoveRange(post.Comments);
            _context.Likes.RemoveRange(likes);
            _context.Posts.Remove(post);

            var result = await _context.SaveChangesAsync();
            if (result > 0)
            {
                return new ResponseManager
                {
                    Message = "Post successfully deleted",
                    IsSuccess = true,
                };
            }
            return new ResponseManager
            {
                Message = "Failed to delete post",
                IsSuccess = true,
            };
        }

        public async Task<IEnumerable<PostViewModel>> GetAllPosts()
        {
            var posts = await _context.Posts.Include(x => x.User).Include(x => x.Comments).ThenInclude(c => c.User).ToListAsync();

            if (posts == null)
            {
                return null;
            }

            List<PostViewModel> postsList = new List<PostViewModel>();
            foreach (var post in posts)
            {
                List<CommentViewModel> commentList = new List<CommentViewModel>();
                foreach (var comment in post.Comments)
                {
                    CommentViewModel commentViewModel = new CommentViewModel
                    {
                        Id = comment.Id,
                        Content = comment.Content,
                        User = new ApplicationUserViewModel
                        {
                            Id = comment.User.Id,
                            Username = comment.User.UserName,
                            ProfilePicture = comment.User.ProfilePicture
                        }
                    };
                    commentList.Add(commentViewModel);
                }

                PostViewModel postViewModel = new PostViewModel
                {
                    Id = post.Id,
                    Content = post.Content,
                    MediaPath = post.MediaPath,
                    LikeCount = _context.Likes.Where(x => x.Post.Id == post.Id).ToList().Count,
                    Comments = commentList,
                    User = new ApplicationUserViewModel
                    {
                        Id = post.User.Id,
                        Username = post.User.UserName,
                        ProfilePicture = post.User.ProfilePicture
                    }
                };
                postsList.Add(postViewModel);
            }
            
            return postsList;
        }

        public async Task<PostViewModel> GetPost(int id)
        {
            var post = await _context.Posts.Include(user => user.User).FirstAsync(x => x.Id == id);
            if (post != null)
            {
                PostViewModel postViewModel = new PostViewModel
                {
                    Id = post.Id,
                    Content = post.Content,
                    MediaPath = post.MediaPath,
                    LikeCount = _context.Likes.Select(x => x.Post.Id == post.Id).ToList().Count,
                    User = new ApplicationUserViewModel
                    {
                        Id = post.User.Id,
                        Username = post.User.UserName,
                        ProfilePicture = post.User.ProfilePicture
                    }
                };
                return postViewModel;
            }
            return null;
        }

        public async Task<PostViewModel> UpdatePost(PostViewModel post)
        {
            var userId = _httpContextAccessor.HttpContext.User?.FindFirstValue(ClaimTypes.NameIdentifier);

            var newPost = await _context.Posts.Include(x => x.User).FirstAsync(x => x.Id == post.Id);

            if (newPost.User.Id != userId)
            {
                return null;
            }
            else if (newPost == null)
            {
                return null;
            }
            newPost.Content = post.Content;
            var result = await _context.SaveChangesAsync();

            if (result > 0)
            {
                PostViewModel postViewModel = new PostViewModel
                {
                    Id = newPost.Id,
                    Content = newPost.Content,
                    MediaPath = newPost.MediaPath,
                    LikeCount = _context.Likes.Select(x => x.Post.Id == post.Id).ToList().Count,
                    User = new ApplicationUserViewModel
                    {
                        Id = newPost.User.Id,
                        Username = newPost.User.UserName,
                        ProfilePicture = newPost.User.ProfilePicture
                    }
                };
                return postViewModel;
            }
            return null;
        }

        public async Task<PostViewModel> LikePost(int postId)
        {
            var userId = _httpContextAccessor.HttpContext.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);
            var post = await _context.Posts.FindAsync(postId);

            if (user != null && post != null)
            {
                if (user.EmailConfirmed == false)
                {
                    return null;
                }
                var query = _context.Likes.Where(x => x.Post.Id == postId && x.User.Id == user.Id);
                var oldLikedPost = query.FirstOrDefault<Like>();
                if (oldLikedPost == null)
                {
                    Like like = new Like
                    {
                        Post = post,
                        User = user
                    };
                    _context.Likes.Add(like);
                    var result = await _context.SaveChangesAsync();

                    if (result > 0)
                    {
                        return await ReturnPosts(postId);
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    _context.Likes.Remove(oldLikedPost);
                    var result = await _context.SaveChangesAsync();
                    
                    if (result > 0)
                    {
                        return await ReturnPosts(postId);
                    }
                    return null;
                }
            }
            return null;
        }

        private async Task<PostViewModel> ReturnPosts(int postId)
        {
            return await GetPost(postId);
        }

        public async Task<CommentViewModel> AddComment(int postId, string commentContent)
        {
            var userId = _httpContextAccessor.HttpContext.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);

            var post = await _context.Posts.FindAsync(postId);

            if (user != null && post != null)
            {
                if (user.EmailConfirmed == false)
                {
                    return null;
                }
                Comment comment = new Comment
                {
                    Content = commentContent,
                    Post = post,
                    User = user,
                    CreatedOn = DateTime.Now
                };
                _context.Comments.Add(comment);
                var result = await _context.SaveChangesAsync();

                if (result > 0)
                {
                    return new CommentViewModel
                    {
                        Id = comment.Id,
                        Content = comment.Content,
                        Post = await GetPost(postId),
                        User = new ApplicationUserViewModel
                        {
                            Id = comment.User.Id,
                            Username = comment.User.UserName,
                            ProfilePicture = comment.User.ProfilePicture
                        }
                    };
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        public async Task<ResponseManager> DeleteComment(int commentId)
        {
            var userId = _httpContextAccessor.HttpContext.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            var comment = await _context.Comments.Include(x => x.User).FirstAsync(x => x.Id == commentId);

            if (comment != null && comment.User.Id == userId)
            {
                _context.Comments.Remove(comment);
                var result = await _context.SaveChangesAsync();

                if (result > 0)
                {
                    return new ResponseManager
                    {
                        Message = "Comment deleted successfully",
                        IsSuccess = true
                    };
                }
                else
                {
                    return new ResponseManager
                    {
                        Message = "Comment can't be deleted",
                        IsSuccess = false
                    };
                }
            }
            return new ResponseManager
            {
                Message = "Comment not found",
                IsSuccess = false
            };
        }
    }
}
