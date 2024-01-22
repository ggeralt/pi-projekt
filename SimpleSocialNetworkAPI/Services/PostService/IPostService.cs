using SimpleSocialNetworkAPI.Model;
using SimpleSocialNetworkShared;

namespace SimpleSocialNetworkAPI.Services.PostService
{
    public interface IPostService
    {
        Task<IEnumerable<PostViewModel>> GetAllPosts();
        Task<PostViewModel> GetPost(int id);
        Task<ResponseManager> CreatePost(PostViewModel post);
        Task<PostViewModel> UpdatePost(PostViewModel post);
        Task<PostViewModel> LikePost(int postId);
        Task<ResponseManager> DeletePost(int id);
        Task<CommentViewModel> AddComment(int postId, string comment);
        Task<ResponseManager> DeleteComment(int commentId);
    }
}
