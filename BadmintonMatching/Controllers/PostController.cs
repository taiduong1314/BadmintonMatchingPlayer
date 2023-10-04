using Entities.RequestObject;
using Entities.ResponseObject;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;

namespace BadmintonMatching.Controllers
{

    [ApiController]
    [Route("api/posts")]
    public class PostController : ControllerBase
    {
        private List<string> PlaceList = new List<string>
        {

        };
        private readonly IPostServices _postServices;
        private readonly IUserServices _userServices;

        public PostController(IPostServices postServices, IUserServices userServices)
        {
            _postServices = postServices;
            _userServices = userServices;
        }

        [HttpGet]
        [Route("user/{user_id}/suggestion")]
        public IActionResult GetSuggestionPost(int user_id)
        {
            if (!_userServices.ExistUserId(user_id))
            {
                return Ok(new { ErrorCode = "Can't found user" });
            }
            var res = _postServices.GetSuggestionPost(user_id);
            return Ok(res);
        }

        [HttpPost]
        [Route("create_by/{user_id}")]
        public IActionResult CreatePost(int user_id, NewPostInfo info)
        {
            if (!_userServices.ExistUserId(user_id))
            {
                return Ok(new { ErrorCode = "Can't found user" });
            }
            var postId = _postServices.CreatePost(user_id, info);
            if(postId != 0)
            {
                return Ok(new {PostId = postId});
            }
            else
            {
                return Ok(new { ErrorCode = "Save fail" });
            }
        }

        [HttpGet]
        [Route("play_ground")]
        public IActionResult GetPostPlayGround()
        {
            var res = _postServices.GetAllPlayGround();
            return Ok(res);
        }

        [HttpGet]
        [Route("play_ground/{play_ground}")]
        public IActionResult GetPostByPlayGround(string play_ground)
        {
            List<PostInfomation> res = _postServices.GetPostByPlayGround(play_ground);
            return Ok(res);
        }

        [HttpGet]
        [Route("{user_id}/managed_all_post")]
        public IActionResult GetPostByPlayGround(int user_id)
        {
            if (!_userServices.ExistUserId(user_id))
            {
                return Ok(new { ErrorCode = "Can't found user" });
            }

            List<PostInfomation> res = new List<PostInfomation>();

            if (_userServices.IsAdmin(user_id))
            {
                res = _postServices.GetManagedPostAdmin(user_id);
            }
            else
            {
                res = _postServices.GetManagedPost(user_id);
            }


            return Ok(res);
        }
        [HttpGet]
        [Route("{post_id}/details")]
        public IActionResult GetDetailPost(int id_post)
        {
           var res = _postServices.GetPostDetail(id_post);
           return Ok(res);
        }
        [HttpGet]
        [Route("{user_id}/post_suggestion")]
        public IActionResult GetListOptionalPost()
        {
            var res = _postServices.GetListOptionalPost();
            return Ok(res);
        }
    }
}
