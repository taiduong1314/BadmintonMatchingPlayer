using Entities.RequestObject;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;

namespace BadmintonMatching.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly IUserServices _userServices;

        public UsersController(IUserServices userServices)
        {
            _userServices = userServices;
        }

        [HttpPost(Name = "email_login")]
        public IActionResult GetUserByEmail(LoginInformation info)
        {
            var userInfo = _userServices.GetExistUser(info);
            if(userInfo.Id == 0)
            {
                return Unauthorized();
            }
            return Ok(userInfo);
        }
    }
}
