using Entities.Models;
using Entities.RequestObject;
using Entities.ResponseObject;

namespace Services.Interfaces
{
    public interface IPostServices
    {
        Task<int> CreatePost(int user_id, NewPostInfo info);
        bool DeletePost(int post_id);
        Task<List<PostOptional>> GetAllPost();
        List<JoinedPost> GetJoined(int user_id);
        List<PostOptional> GetListOptionalPost();
        List<ListPostByAdmin> GetListPostByAdmin();
        List<PostInfomation> GetManagedPost(int user_id);
        List<PostInfomation> GetManagedPostAdmin(int user_id);
        List<PostInfomation> GetPostByPlayGround(string play_ground);
        PostDetail GetPostDetail(int id_post);
        List<PostInfomation> GetSuggestionPost(int user_id);
        Task<string> HandleImg(string imgUrl);
    }
}
