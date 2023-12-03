using Entities.Models;
using Entities.RequestObject;
using Entities.ResponseObject;

namespace Services.Interfaces
{
    public interface IPostServices
    {
        Task<bool> CreateBlog(int user_id, NewBlogInfo info);
        Task<int> CreatePost(int user_id, NewPostInfo info);
        bool DeletePost(int post_id);
        Task<List<BlogInList>> GetAllBlogs();
        Task<List<PostOptional>> GetAllPost();
        Task<BlogDetail> GetBlogDetail(int blog_id);
        Task<List<Room>> GetChatRooms(int post_id);
        Task<List<JoinedPost>> GetJoined(int user_id);
        List<PostOptional> GetListOptionalPost();
        List<ListPostByAdmin> GetListPostByAdmin();
        List<PostInfomation> GetManagedPost(int user_id);
        List<PostInfomation> GetManagedPostAdmin(int user_id);
        List<PostOptional> GetPostByPlayGround(string play_ground);
        PostDetail GetPostDetail(int id_post);
        List<PostInfomation> GetSuggestionPost(int user_id);
        Task<string> HandleImg(string imgUrl);
    }
}
