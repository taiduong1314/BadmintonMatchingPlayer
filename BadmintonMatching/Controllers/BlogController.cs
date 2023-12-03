using CloudinaryDotNet.Actions;
using Entities.RequestObject;
using Entities.ResponseObject;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;

namespace BadmintonMatching.Controllers
{
    [Route("api/blogs")]
    [ApiController]
    public class BlogController : ControllerBase
    {
        private readonly IPostServices _postServices;
        private readonly IUserServices _userServices;

        public BlogController(IPostServices postServices, IUserServices userServices)
        {
            _postServices = postServices;
            _userServices = userServices;
        }

        [HttpPost]
        [Route("create_by/{user_id}")]
        public async Task<IActionResult> CreateBlog(int user_id, NewBlogInfo info)
        {
            try
            {
                if (! await _userServices.IsStaff(user_id))
                {
                    throw new Exception("Not staff to create");
                }

                if(await _postServices.CreateBlog(user_id, info))
                {
                    return Ok(new SuccessObject<object> { Message = Message.SuccessMsg, Data = true });
                }
                else
                {
                    return Ok(new SuccessObject<object> { Message = "Create fail" });
                }
            }
            catch (Exception ex)
            {
                return Ok(new SuccessObject<object> { Message = ex.Message});
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetBlogs(int user_id)
        {
            var res = await _postServices.GetAllBlogs();
            return Ok(new SuccessObject<List<BlogInList>> { Data = res, Message = Message.SuccessMsg});
        }

        [HttpGet]
        [Route("{blog_id}/details/by/{user_id}")]
        public async Task<IActionResult> GetBlogDetail(int user_id, int blog_id)
        {
            try
            {
                var detail = await _postServices.GetBlogDetail(blog_id);
                detail.CanDelete = await _userServices.IsStaff(user_id);
                return Ok(new SuccessObject<BlogDetail> { Data = detail, Message = Message.SuccessMsg});
            }
            catch (Exception ex)
            {
                return Ok(new SuccessObject<object> { Message = ex.Message });
            }
        }

        [HttpDelete]
        [Route("{blog_id}/by/{user_id}")]
        public async Task<IActionResult> DeleteBlog(int user_id, int blog_id)
        {
            try
            {
                if (!await _userServices.IsStaff(user_id))
                {
                    throw new Exception("Not staff to delete");
                }

                if (_postServices.DeletePost(blog_id))
                {
                    return Ok(new SuccessObject<object> { Message = Message.SuccessMsg, Data = true });
                }
                else
                {
                    return Ok(new SuccessObject<object> { Message = "Delete fail" });
                }
            }
            catch (Exception ex)
            {
                return Ok(new SuccessObject<object> { Message = ex.Message });
            }
        }
    }
}
