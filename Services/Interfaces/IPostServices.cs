using Entities.Models;
using Entities.RequestObject;
using Entities.ResponseObject;

namespace Services.Interfaces
{
    public interface IPostServices
    {
        int CreatePost(int user_id, NewPostInfo info);
        List<string?> GetAllPlayGround();
        Task<List<Post>> GetAllPost();
        List<PostOptional> GetListOptionalPost();
        List<PostInfomation> GetManagedPost(int user_id);
        List<PostInfomation> GetManagedPostAdmin(int user_id);
        List<PostInfomation> GetPostByPlayGround(string play_ground);
        PostDetail GetPostDetail(int id_post);
        List<PostInfomation> GetSuggestionPost(int user_id);
    }
}
