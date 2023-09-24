using Entities.RequestObject;
using Entities.ResponseObject;

namespace Services.Interfaces
{
    public interface IPostServices
    {
        int CreatePost(int user_id, NewPostInfo info);
        List<string?> GetAllPlayGround();
        List<PostInfomation> GetPostByPlayGround(string play_ground);
        List<PostInfomation> GetSuggestionPost(int user_id);
    }
}
