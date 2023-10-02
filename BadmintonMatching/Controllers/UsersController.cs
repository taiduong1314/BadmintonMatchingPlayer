using Entities.RequestObject;
using Entities.ResponseObject;
using Microsoft.AspNetCore.Connections.Features;
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

        [HttpPost]
        [Route("email_login")]
        public IActionResult GetUserByEmail(LoginInformation info)
        {
            var userInfo = _userServices.GetExistUser(info);
            if (userInfo.Id == 0)
            {
                return Unauthorized();
            }
            return Ok(userInfo);
        }

        [HttpPost]
        [Route("register")]
        public IActionResult RegistUser(RegisInfomation info)
        {
            if (info.Password != info.ReEnterPass)
            {
                return Ok("Password not matches");
            }

            if (_userServices.IsUserExist(info.Email))
            {
                return Ok("Email already exist");
            }

            var userId = _userServices.RegistUser(info);
            return Ok(new { UserId = userId });
        }

        [HttpPost]
        [Route("{user_id}/playing_area")]
        public IActionResult AddPlayingArea(int user_id, NewPlayingArea info)
        {
            if (!_userServices.ExistUserId(user_id))
            {
                return Ok(new { ErrorCode = "Can't found user" });
            }
            if (info.ListArea != null)
            {
                _userServices.AddPlayingArea(user_id, info);
            }
            return Ok("Playing Area is saved");
        }

        [HttpPost]
        [Route("{user_id}/playing_level")]
        public IActionResult AddPlayingLevel(int user_id, NewPlayingLevel info)
        {
            if (!_userServices.ExistUserId(user_id))
            {
                return Ok(new { ErrorCode = "Can't found user" });
            }
            if (info.Point > 0)
            {
                _userServices.AddPlayingLevel(user_id, info);
            }
            return Ok("Playing Level is saved");
        }

        [HttpPost]
        [Route("{user_id}/playing_way")]
        public IActionResult AddPlayingWay(int user_id, NewPlayingWay info)
        {
            if (!_userServices.ExistUserId(user_id))
            {
                return Ok(new { ErrorCode = "Can't found user" });
            }
            if (info.PlayingWays != null)
            {
                _userServices.AddPlayingWay(user_id, info);
            }
            return Ok("Playing Way is saved");
        }

        [HttpGet]
        [Route("{user_id}/player_suggestion")]
        public IActionResult GetPlayerSuggestion(int user_id)
        {
            if (!_userServices.ExistUserId(user_id))
            {
                return Ok(new { ErrorCode = "Can't found user" });
            }
            var areas = _userServices.GetUserAreas(user_id);
            var res = new List<UserSuggestion>();
            if (areas.Count() > 0)
            {
                res = _userServices.FindUserByArea(areas);
            }
            if (res.Count() < 9)
            {
                int skill = _userServices.GetUserSkill(user_id);
                if (skill > 0)
                {
                    res = _userServices.FindUserBySkill(skill, res);
                }
            }
            if (res.Count() < 9)
            {
                List<string> ways = _userServices.GetUserPlayWay(user_id);
                if (ways.Count > 0)
                {
                    res = _userServices.FindUserByPlayWays(ways, res);
                }
            }
            return Ok(res);
        }

        [HttpGet]
        [Route("{email}/verify_token")]
        public IActionResult GetVerifyToken(string email)
        {
            if (!_userServices.IsUserExist(email))
            {
                return Ok(new { ErrorCode = "Can't found user" });
            }

            var token = _userServices.CreateVerifyToken(email);
            return Ok(new { Token = token });
        }

        [HttpPost]
        [Route("verify_token")]
        public IActionResult VerifyToken(UserVerifyToken info)
        {
            if (!_userServices.IsUserExist(info.Email))
            {
                return Ok(new { ErrorCode = "Can't found user" });
            }

            var success = _userServices.CheckRemoveVefToken(info);

            return Ok(success ? new { Message = "Verify Success" } : new { ErrorCode = "Invalid token" });
        }

        [HttpPut]
        [Route("{email}/new_pass")]
        public IActionResult ChangePassword(string email, UpdatePassword info)
        {
            if (!_userServices.IsUserExist(email))
            {
                return Ok(new { ErrorCode = "Can't found user" });
            }

            if (info.NewPassword != info.ReEnterPassword)
            {
                return Ok(new { ErrorCode = "Password verify not matches" });
            }

            var success = _userServices.UpdatePassword(email, info);

            return Ok(success ? new { Message = "Update Success" } : new { ErrorCode = "Update fail" });
        }

        [HttpGet]
        [Route("{user_id}/comments")]
        public IActionResult GetComments (int user_id)
        {
            if (!_userServices.ExistUserId(user_id))
            {
                return Ok(new { ErrorCode = "Can't found user" });
            }

            var res = _userServices.GetComments(user_id);

            return Ok(res);
        }

        [HttpPost]
        [Route("{user_id}/comments/{user_id_receive_comment}")]
        public IActionResult AddComment(int user_id, int user_id_receive_comment, AddCommentToUser comment)
        {
            if (!_userServices.ExistUserId(user_id) || !_userServices.ExistUserId(user_id_receive_comment))
            {
                return Ok(new { ErrorCode = "Can't found user" });
            }

            int commentId = _userServices.SaveComment(user_id, user_id_receive_comment, comment);

            return Ok(commentId > 0 ? new { Message = "Update Success" } : new { ErrorCode = "Update fail" });
        }
    }
}
