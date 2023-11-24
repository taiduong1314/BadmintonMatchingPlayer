using Entities.Models;
using Entities.RequestObject;
using Entities.ResponseObject;
using Microsoft.AspNetCore.Mvc;
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
                return Ok(new SuccessObject<object> { Message = "Tài khoản không tồn tại" });
            }
            var userInfo = _userServices.GetExistUser(info);

            if (userInfo.Id == -1)
            {
                return Ok(new SuccessObject<object> { Message = "UserId is banded by admin from login" });
            }
            else if (userInfo.Id == 0)
            {
                return Ok(new SuccessObject<object> { Message = "Tài khoản hoặc mật khẩu không đúng" });
            }
            return Ok(new SuccessObject<UserInformation> { Data = userInfo, Message = Message.SuccessMsg });
        }

        [HttpPost]
        [Route("register")]
        public IActionResult RegistUser(RegisInfomation info)
        {
            if (info.Password != info.ReEnterPass)
            {
                return Ok(new SuccessObject<object> { Message = "Mật khẩu không trùng khớp" });
            }

            if (_userServices.IsUserExist(info.Email))
            {
                return Ok(new SuccessObject<object> { Message = "Email này đã tồn tại" });
            }

            var userId = _userServices.RegistUser(info);
            return Ok(new SuccessObject<object> { Data = new { UserId = userId }, Message = Message.SuccessMsg });
        }

        [HttpPost]
        [Route("{user_id}/playing_area")]
        [ProducesResponseType(typeof(SuccessObject<List<Reports>>), 200)]
        public IActionResult AddPlayingArea(int user_id, NewPlayingArea info)
        {
            if (!_userServices.ExistUserId(user_id))
            {
                return Ok(new SuccessObject<object> { Message = "Can't found user" });
            }
            if (info.ListArea != null)
            {
                _userServices.AddPlayingArea(user_id, info);
            }
            return Ok(new SuccessObject<object> { Message = "Playing Area is saved", Data = true });
        }

        [HttpPost]
        [Route("{user_id}/playing_level")]
        [ProducesResponseType(typeof(SuccessObject<List<Reports>>), 200)]
        public IActionResult AddPlayingLevel(int user_id, NewPlayingLevel info)
        {
            if (!_userServices.ExistUserId(user_id))
            {
                return Ok(new SuccessObject<object> { Message = "Can't found user" });
            }
            if (info.Point > 0)
            {
                _userServices.AddPlayingLevel(user_id, info);
            }
            return Ok(new SuccessObject<object> { Message = "Playing Level is saved", Data = true });
        }

        [HttpPost]
        [Route("{user_id}/playing_way")]
        [ProducesResponseType(typeof(SuccessObject<List<Reports>>), 200)]
        public IActionResult AddPlayingWay(int user_id, NewPlayingWay info)
        {
            if (!_userServices.ExistUserId(user_id))
            {
                return Ok(new SuccessObject<object> { Message = "Can't found user" });
            }
            if (info.PlayingWays != null)
            {
                _userServices.AddPlayingWay(user_id, info);
            }
            return Ok(new SuccessObject<object> { Message = "Playing Way is saved" , Data = true });
        }

        [HttpGet]
        [Route("{user_id}/player_suggestion")]
        [ProducesResponseType(typeof(SuccessObject<List<UserSuggestion>>), 200)]
        public IActionResult GetPlayerSuggestion(int user_id)
        {
            if (!_userServices.ExistUserId(user_id))
            {
                return Ok(new SuccessObject<List<UserSuggestion?>> { Message = "Can't found user" });
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
            return Ok(new SuccessObject<List<UserSuggestion>> { Data = res, Message = Message.SuccessMsg });
        }

        [HttpGet]
        [Route("{email}/verify_token")]
        [ProducesResponseType(typeof(SuccessObject<List<Reports>>), 200)]
        public async Task<IActionResult> GetVerifyToken(string email)
        {
            if (!_userServices.IsUserExist(email))
            {
                return Ok(new SuccessObject<object> { Message = "Can't found user" });
            }

            await _userServices.SendEmailAsync(email);
            return Ok(new SuccessObject<object> { Data = true, Message = "Send mail success" });
        }

        [HttpPost]
        [Route("verify_token")]
        public IActionResult VerifyToken(UserVerifyToken info)
        {
            if (!_userServices.IsUserExist(info.Email))
            {
                return Ok(new SuccessObject<object> { Message = "Can't found user" });
            }

            var success = _userServices.CheckRemoveVefToken(info);

            return Ok(success ? new SuccessObject<object> { Message = "Verify Success", Data = true } 
            : new SuccessObject<object> { Message = "Invalid token" });
        }

        [HttpPut]
        [Route("{email}/new_pass")]
        public IActionResult ChangePassword(string email, UpdatePassword info)
        {
            if (!_userServices.IsUserExist(email))
            {
                return Ok(new SuccessObject<object> { Message = "Can't found user" });
            }

            if (info.NewPassword != info.ReEnterPassword)
            {
                return Ok(new SuccessObject<object> { Message = "Password verify not matches" });
            }

            var success = _userServices.UpdatePassword(email, info);

            return Ok(success ? new SuccessObject<object> { Message = "Update Success", Data = true } : new SuccessObject<object> { Message = "Update fail" });
        }

        [HttpGet]
        [Route("{user_id}/comments")]
        [ProducesResponseType(typeof(SuccessObject<List<CommentInfos>>), 200)]
        public IActionResult GetComments(int user_id)
        {
            if (!_userServices.ExistUserId(user_id))
            {
                return Ok(new SuccessObject<List<CommentInfos?>> { Message = "Can't found user" });
            }

            var res = _userServices.GetComments(user_id);

            return Ok(new SuccessObject<List<CommentInfos>> { Data = res, Message = Message.SuccessMsg });
        }

        [HttpPost]
        [Route("{user_id}/comments/{user_id_receive_comment}")]
        public IActionResult AddComment(int user_id, int user_id_receive_comment, AddCommentToUser comment)
        {
            if (!_userServices.ExistUserId(user_id) || !_userServices.ExistUserId(user_id_receive_comment))
            {
                return Ok(new SuccessObject<object> { Message = "Can't found user" });
            }

            int commentId = _userServices.SaveComment(user_id, user_id_receive_comment, comment);

            return Ok(commentId > 0 ? new SuccessObject<object> { Message = "Update Success", Data = true } 
            : new SuccessObject<object> { Message = "Update fail" });
        }

        [HttpGet]
        [Route("{user_id}/banded_users")]
        [ProducesResponseType(typeof(SuccessObject<List<BandedUsers>>), 200)]
        public IActionResult GetBandedUsers(int user_id)
        {
            if (!_userServices.ExistUserId(user_id))
            {
                return Ok(new SuccessObject<List<BandedUsers?>> { Message = "Can't found user" });
            }

            List<BandedUsers> bandedLs = _userServices.GetBandedUsers(user_id);

            return Ok(new SuccessObject<List<BandedUsers>> { Data = bandedLs, Message = Message.SuccessMsg });
        }

        [HttpPut]
        [Route("{user_id}/ban_unban/{user_effect}")]
        public IActionResult BanUnban(int user_id, int user_effect)
        {
            bool updateSuccess = _userServices.BanUnband(user_id, user_effect);

            return Ok(updateSuccess ? new SuccessObject<object> { Message = "Update Success", Data = true } 
            : new SuccessObject<object> { Message = "Update fail" });
        }

        [HttpGet]
        [Route("managed/{user_id}")]
        [ProducesResponseType(typeof(SuccessObject<List<UserManaged>>), 200)]
        public IActionResult GetAllUserForManaged(int user_id)
        {
            if (!_userServices.IsAdmin(user_id))
            {
                return Ok(new SuccessObject<List<UserManaged?>> { Message = "Not admin for get" });
            }

            var users = _userServices.GetUserForManaged();

            return Ok(new SuccessObject<List<UserManaged>> { Data = users, Message = Message.SuccessMsg });
        }

        [HttpPut]
        [Route("{admin_id}/banded/{user_id}")]
        public IActionResult BanUnbanUser(int admin_id, int user_id)
        {
            if (!_userServices.IsAdmin(admin_id))
            {
                return Ok(new SuccessObject<object> { Message = "Not admin for update" });
            }

            try
            {
                bool isBanded = _userServices.BanUnbandLogin(user_id);

                return Ok(new SuccessObject<object> { Data = new { status = isBanded ? "Banded" : "Unbaded" }, Message = Message.SuccessMsg });
            }
            catch (NotImplementedException)
            {
                return Ok(new SuccessObject<object> { Message = "not found user_id" });
            }
        }

        [HttpGet]
        [Route("{admin_id}/report/{user_id}")]
        [ProducesResponseType(typeof(SuccessObject<List<UserReport>>), 200)]
        public IActionResult GetUserReports(int admin_id, int user_id)
        {
            if (!_userServices.IsAdmin(admin_id))
            {
                return Ok(new SuccessObject<List<UserReport?>> { Message = "Not admin for update" });
            }
            List<UserReport> reports = _userServices.GetReports(user_id);
            return Ok(new SuccessObject<List<UserReport>> { Data = reports, Message = Message.SuccessMsg });
        }

        #region Get User Profile
        [HttpGet]
        [Route("{user_id}/profile")]
        [ProducesResponseType(typeof(SuccessObject<UserProfile>), 200)]
        public IActionResult GetUserProfile(int user_id)
        {
            if (!_userServices.ExistUserId(user_id))
            {
                return Ok(new SuccessObject<UserProfile?> { Message = "Can't found user" });
            }
            var res = _userServices.GetUserProfileSetting(user_id);
            res.Helpful = _userServices.GetHelpful(user_id);
            res.Friendly = _userServices.GetFriendly(user_id);
            res.Trusted = _userServices.GetTrusted(user_id);
            res.LevelSkill = _userServices.GetLevelSkill(user_id);

            return Ok(new SuccessObject<UserProfile> { Data = res, Message = Message.SuccessMsg });
        }
        #endregion

        #region Get all user
        [HttpGet]
        [Route("GetListUser")]
        [ProducesResponseType(typeof(SuccessObject<List<User>>), 200)]
        public async Task<IActionResult> GetAllAccount()
        {
            var res = await _userServices.GetAllAccount();
            return Ok(new SuccessObject<List<User>> { Data = res, Message = Message.SuccessMsg });
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
            try
            {
                var data = await _userServices.UpdateProfile(user_id, param, trackChanges);
                return Ok(new SuccessObject<object> { Data = data, Message = Message.SuccessMsg });
            }
            catch
            {
                return Ok(new SuccessObject<object> { Message = "Invalid base 64 string" });
            }
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
                return Ok(new SuccessObject<object> { Message = "Can't found user" });
            }

            int commentId = _userServices.CreateReport(user_id, userreport_id, report);

            return Ok(commentId > 0 ? new SuccessObject<object> { Message = "Report Successfull", Data = true } : new SuccessObject<object> { Message = "Report fail" });
        }
        #endregion

        #region Get User Profile
        [HttpGet]
        [Route("user_id")]
        [ProducesResponseType(typeof(SuccessObject<SelfProfile>), 200)]
        public IActionResult GetSelfProfile(int user_id)
        {
            if (!_userServices.ExistUserId(user_id))
            {
                return Ok(new SuccessObject<SelfProfile?> { Message = "Can't found user" });
            }
            var res = _userServices.GetSelfProfile(user_id);
            return Ok(new SuccessObject<SelfProfile> { Data = res, Message = Message.SuccessMsg });
        }
        #endregion

        [HttpPut]
        [Route("{user_id}/sub_unsub/{target_id}")]
        public async Task<IActionResult> SubUnsub(int user_id, int target_id)
        {
            var res = await _userServices.SubUnSub(user_id, target_id);
            switch (res)
            {
                case -2:
                    {
                        return Ok(new SuccessObject<object?> { Message = "Can't found user" });
                    }
                case -1:
                    {
                        return Ok(new SuccessObject<object?> { Message = "Can't found target user" });
                    }
                default:
                    {
                        var addTxt = res == 0 ? "Unsubcribe" : "Subcribe";
                        return Ok(new SuccessObject<object?> { Data = true, Message = $"{addTxt} success!" });
                    }
            }
        }

        [HttpPut]
        [Route("{user_id}/setting_password")]
        public async Task<IActionResult> SettingPassword(int user_id, SettingPasswordRequest info)
        {
            var res = await _userServices.SettingPassword(user_id, info);
            if (res == 1)
            {
                return Ok(new SuccessObject<object?> { Data = true, Message = $"Update success!" });
            }
            else if (res == 0)
            {
                return Ok(new SuccessObject<object?> { Message = "Invalid old password" });
            }
            else if (res == -1)
            {
                return Ok(new SuccessObject<object?> { Message = "Wrong re-input password" });
            }
            else if (res == -2)
            {
                return Ok(new SuccessObject<object?> { Message = "Invalid user" });
            }
            else
            {
                return Ok(new SuccessObject<object?> { Message = "Update Fail" });
            }
        }
        #region List Subcribe By User
        [HttpGet]
        [Route("user_id/listsubed")]
        public IActionResult GetListSubed(int user_id,int usertarget_id)
        {
           var subed = _userServices.Subcr(user_id, usertarget_id);

            if (subed)
            {
                // Subcribed
                return Ok(new { message = "Đã Subcribed" });
            }
            else
            {
                // Not Subcribe
                return Ok(new { message = "Chưa Subcribed" });
            }
        }
        #endregion

    }
}
