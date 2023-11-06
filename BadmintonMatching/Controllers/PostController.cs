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
                return Ok(new ErrorObject { ErrorCode = "Can't found user" });
            }
            var res = _postServices.GetSuggestionPost(user_id);
            return Ok(new SuccessObject { Data = res, Message = Message.SuccessMsg });
        }

        [HttpPost]
        [Route("create_by/{user_id}")]
        public async Task<IActionResult> CreatePost(int user_id, NewPostInfo info)
        {
            if (!_userServices.ExistUserId(user_id))
            {
                Ok(new ErrorObject { ErrorCode = "Can't found user" });
            }
            var postId = await _postServices.CreatePost(user_id, info);
            if (postId != 0)
            {
                return Ok(new SuccessObject { Data = new { PostId = postId }, Message = Message.SuccessMsg });
            }
            else
            {
                return Ok(new ErrorObject { ErrorCode = "Save fail" });
            }
        }

        [HttpGet]
        [Route("play_ground")]
        public IActionResult GetPostPlayGround()
        {
            var res = _postServices.GetAllPlayGround();
            return Ok(new SuccessObject { Data = res, Message = Message.SuccessMsg });
        }

        [HttpGet]
        [Route("play_ground/{play_ground}")]
        public IActionResult GetPostByPlayGround(string play_ground)
        {
            List<PostInfomation> res = _postServices.GetPostByPlayGround(play_ground);
            return Ok(new SuccessObject { Data = res, Message = Message.SuccessMsg });
        }

        [HttpGet]
        [Route("{user_id}/managed_all_post")]
        public IActionResult GetPostByPlayGround(int user_id)
        {
            if (!_userServices.ExistUserId(user_id))
            {
                return Ok(new ErrorObject { ErrorCode = "Can't found user" });
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


            return Ok(new SuccessObject { Data = res, Message = Message.SuccessMsg });
        }
        [HttpGet]
        [Route("{post_id}/details")]
        public IActionResult GetDetailPost(int post_id)
        {
            var res = _postServices.GetPostDetail(post_id);
            return Ok(new SuccessObject { Data = res, Message = Message.SuccessMsg });
        }

        [HttpGet]
        [Route("{user_id}/post_suggestion")]
        public IActionResult GetListOptionalPost()
        {
            var res = _postServices.GetListOptionalPost();
            return Ok(new SuccessObject { Data = res, Message = Message.SuccessMsg });
        }
        [HttpGet]
        [Route("GetListPost")]
        public async Task<IActionResult> GetListPost()
        {
            var res = await _postServices.GetAllPost();
            return Ok(new SuccessObject { Data = res, Message = Message.SuccessMsg });
        }

        #region List Post at role Admin
        [HttpGet]
        [Route("{admin_id}/post")]
        public IActionResult GetListPostByAdmin(int admin_id)
        {

            var checkadmin = _userServices.IsAdmin(admin_id);
            if (checkadmin)
            {
                var res = _postServices.GetListPostByAdmin();
                return Ok(new SuccessObject { Data = res, Message = Message.SuccessMsg });
            }
            else
            {

                return Ok(new ErrorObject { ErrorCode = "Not admin" });
            }
        }
        #endregion
        #region Delete Post byAdmin
        [HttpPut]
        [Route("{admin_id}/delete/{post_id}")]
        public IActionResult DeletePost(int post_id, int admin_id)
        {
            if (_userServices.IsAdmin(admin_id) == false)
            {
                return Ok(new ErrorObject { ErrorCode = "Not admin" });
            }

            var res = _postServices.DeletePost(post_id);
            return Ok(res ? new SuccessObject { Data = null, Message = Message.SuccessMsg }: new ErrorObject { ErrorCode = "Update fail" });
        }


        #endregion
    }
}
