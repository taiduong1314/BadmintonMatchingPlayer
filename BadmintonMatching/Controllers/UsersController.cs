using Entities.Models;
using Entities.RequestFeatures;
using Entities.RequestObject;
using Entities.ResponseObject;
using Microsoft.AspNetCore.Connections.Features;
using Microsoft.AspNetCore.Mvc;
using Repositories.Intefaces;
using Services.Interfaces;

namespace BadmintonMatching.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly IUserServices _userServices;
        private readonly IJwtSupport _jwtServices;
        

        public UsersController(IUserServices userServices, IJwtSupport jwtServices)
        {
            _userServices = userServices;
            _jwtServices = jwtServices;
        }

        [HttpPost]
        [Route("email_login")]
        public IActionResult GetUserByEmail(LoginInformation info)
        {          
            if (!_userServices.IsUserExist(info.Email))
            {
                return Ok(new { ErrorEmail = "Tài khoản không tồn tại" });
            }
            var userInfo = _userServices.GetExistUser(info);
            if (userInfo.Id == 0)
            {
                return Ok(new { LoginError = "Tài khoản hoặc mật khẩu không đúng" });
            }
            return Ok(userInfo);
        }

        [HttpPost]
        [Route("register")]
        public IActionResult RegistUser(RegisInfomation info)
        {
            if (info.Password != info.ReEnterPass)
            {
                return Ok(new { ErrorPassword = "Mật khẩu không trùng khớp" });
            }

            if (_userServices.IsUserExist(info.Email))
            {
                return Ok(new { ErrorEmail = "Email này đã tồn tại" });
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
            return Ok(new { Message = "Playing Area is saved" });
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
            return Ok(new { Message = "Playing Level is saved" });
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
            return Ok(new { Message = "Playing Way is saved" });
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

            var otp = _userServices.CreateVerifyToken(email);
            var token = _jwtServices.CreateToken(otp);
            return Ok(new { Token = token });
        }
        [HttpGet]
        [Route("{email}/verify_otp")]
        public IActionResult GetVerifyOtp(string email)
        {
            if (!_userServices.IsUserExist(email))
            {
                return Ok(new { ErrorCode = "Can't found user" });
            }

            var otp = _userServices.CreateVerifyToken(email);
            var token = _jwtServices.CreateToken(otp);
            return Ok(new VerifyEmail { Token = token,Otp = otp}); 
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

        [HttpGet]
        [Route("{user_id}/banded_users")]
        public IActionResult GetBandedUsers(int user_id)
        {
            if (!_userServices.ExistUserId(user_id))
            {
                return Ok(new { ErrorCode = "Can't found user" });
            }

            List<BandedUsers> bandedLs = _userServices.GetBandedUsers(user_id);

            return Ok(bandedLs);
        }

        [HttpPut]
        [Route("{user_id}/ban_unban/{user_effect}")]
        public IActionResult BanUnban(int user_id, int user_effect)
        {
            bool updateSuccess = _userServices.BanUnband(user_id, user_effect);

            return Ok(updateSuccess ? new { Message = "Update Success" } : new { ErrorCode = "Update fail" });
        }

        [HttpGet]
        [Route("managed/{user_id}")]
        public IActionResult GetAllUserForManaged(int user_id)
        {
            if (!_userServices.IsAdmin(user_id))
            {
                return Ok(new { Error = "Not admin for get" });
            }

            var users = _userServices.GetUserForManaged();

            return Ok(users);
        }

        #region Get User Profile
        [HttpGet]
        [Route("{user_id}/profile")]
        public IActionResult GetUserProfile(int user_id)
        {
            if (!_userServices.ExistUserId(user_id))
            {
                return Ok(new { ErrorCode = "Can't found user" });
            }
            var res = _userServices.GetUserProfileSetting(user_id);
            res.Helpful = _userServices.GetHelpful(user_id);
            res.Friendly = _userServices.GetFriendly(user_id);
            res.Trusted = _userServices.GetTrusted(user_id);
            res.LevelSkill = _userServices.GetLevelSkill(user_id);

            return Ok(res);
        }
        #endregion

        #region Get all user
        [HttpGet]
        [Route("GetListUser")]
        public async Task<IActionResult> GetAllAccount()
        {
            var res = await _userServices.GetAllAccount();
            return Ok(res);
        }
        #endregion

        #region Update User's Profile
        /// <summary>
        /// Update user profile (Role: Customer)
        /// </summary>
        /// <param name="param"></param>
        /// <param name="user_id"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("{user_id}")]
        public async Task<IActionResult> UpdateUserProfile(UpdateProfileUser param, int user_id, bool trackChanges)
        {
          return  await _userServices.UpdateProfile(user_id, param, trackChanges);
            //await _repository.SaveAsync();
            //return Ok();
        }

        #endregion

        #region Report
        [HttpPost]
        [Route("report/{user_id}")]
        public IActionResult CreateReport(int user_id, int userreport_id, AddReport report)
        {
            if (!_userServices.ExistUserId(user_id) || !_userServices.ExistUserId(userreport_id))
            {
                return Ok(new { ErrorCode = "Can't found user" });
            }

            int commentId = _userServices.CreateReport(user_id, userreport_id, report);

            return Ok(commentId > 0 ? new { Message = "Report Successfull" } : new { ErrorCode = "Report fail" });
        }
        #endregion

        #region Get User Profile
        [HttpGet]
        [Route("user_id")]
        public IActionResult GetSelfProfile(int user_id)
        {
            if (!_userServices.ExistUserId(user_id))
            {
                return Ok(new { ErrorCode = "Can't found user" });
            }
            var res = _userServices.GetSelfProfile(user_id);
            return Ok(res);
        }
        #endregion
        
    }
}
