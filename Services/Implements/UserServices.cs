﻿using Entities.Models;
using Entities.RequestObject;
using Entities.ResponseObject;
using Microsoft.EntityFrameworkCore;
using Repositories.Intefaces;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata;
using System.Net.Http;
using System.Text.Json;
using System.Net.Http.Headers;
using CloudinaryDotNet.Actions;
using CloudinaryDotNet;

namespace Services.Implements
{
    public enum UserRole
    {
        Admin = 1,
        User = 0
    }

    public class UserServices : IUserServices
    {
        private readonly IRepositoryManager _repositoryManager;
        private readonly IJwtSupport _jwtSupport;
        private readonly HttpClient _httpClient;

        

        public UserServices(IRepositoryManager repositoryManager, IJwtSupport jwtSupport, HttpClient httpClient)
        {
            _repositoryManager = repositoryManager;
            _jwtSupport = jwtSupport;
            _httpClient = httpClient;
        }

        public void AddPlayingArea(int user_id, NewPlayingArea info)
        {
            if (info.ListArea != null)
            {
                var areaInfo = "";
                foreach (var area in info.ListArea)
                {
                    areaInfo += area+";";
                }
                var user = _repositoryManager.User.FindByCondition(x => x.Id == user_id, false).FirstOrDefault();
                if(user != null)
                {
                    user.PlayingArea = areaInfo;
                    _repositoryManager.User.Update(user);
                    _repositoryManager.SaveAsync().Wait();
                }
            }
        }
        public async Task<string> HandleImg(string base64encodedstring)
        {
            var revertbase64 = base64encodedstring.Replace("data:image/jpeg;base64,", "");
            try
            {
                var bytes = Convert.FromBase64String(revertbase64);
                var contents = new StreamContent(new MemoryStream(bytes));
                Account account = new Account(
                    "dbjvirvym",
                    "487892318776179",
                    "txx6fF8ZVsT72id6ySvqNqwrN0E");
                Cloudinary cloudinary = new Cloudinary(account);

                string publicId = Guid.NewGuid().ToString();
                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(publicId, new MemoryStream(bytes)),
                    PublicId = publicId
                };
                var uploadResult = await cloudinary.UploadAsync(uploadParams);

                return uploadResult.SecureUrl.ToString();

            }
            catch (Exception ex)
            {
                return base64encodedstring;
            }

        }

        public void AddPlayingLevel(int user_id, NewPlayingLevel info)
        {
            if (info.Point > 0)
            {
                var user = _repositoryManager.User.FindByCondition(x => x.Id == user_id, false).FirstOrDefault();
                if (user != null)
                {
                    user.PlayingLevel = info.Point;
                    _repositoryManager.User.Update(user);
                    _repositoryManager.SaveAsync().Wait();
                }
            }
        }

        public void AddPlayingWay(int user_id, NewPlayingWay info)
        {
            if (info.PlayingWays != null)
            {
                var waysInfo = "";
                foreach (var area in info.PlayingWays)
                {
                    waysInfo += area + ";";
                }
                var user = _repositoryManager.User.FindByCondition(x => x.Id == user_id, false).FirstOrDefault();
                if (user != null)
                {
                    user.PlayingWay = waysInfo;
                    _repositoryManager.User.Update(user);
                    _repositoryManager.SaveAsync().Wait();
                }
            }
        }
        public async Task SendEmailAsync(string? email)
        {
            var random = new Random();
            var token = random.Next(0, 999999).ToString();
            while (true)
            {
                if (token.Length < 6)
                {
                    var addNum = 6 - token.Length;
                    for (var i = 1; i <= addNum; i++)
                    {
                        token = "0" + token;
                    }
                }

                var user_id = _repositoryManager.User.FindByCondition(x => x.Email == email, true).Select(x => x.Id).FirstOrDefault();
                if (user_id > 0)
                {
                    if (_repositoryManager.VerifyToken.FindByCondition(x => x.Token == token && x.UserId == user_id, true) == null)
                    {
                        continue;
                    }
                    _repositoryManager.VerifyToken.Create(new VerifyToken
                    {
                        Token = token,
                        UserId = user_id
                    });
                    _repositoryManager.SaveAsync().Wait();
                    break;
                }
            }
            try
            {
                //var apiKey = "LoTT0MXpZNnm_YpCc";
                var serviceId = "service_qohde6p";
                var templateId = "template_k2ncebm";
                var userId = "v2YsGMt6K5MlrJD5S"; // Thay thế bằng user_id của bạn (nếu cần)
                var apiKey = "IEfA8kxTGe-JiqUqfUkUt";
                var emailJsServerUrl = $"https://api.emailjs.com/api/v1.0/email/send";

                var emailContent = new
                {
                    service_id = serviceId,
                    template_id = templateId,
                    user_id = userId,
                    template_params = new
                    {
                        to_email = email,
                        message = token // Nội dung email
                    }
                };

                var content = new StringContent(JsonSerializer.Serialize(emailContent), Encoding.UTF8, "application/json");
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
                var response = await _httpClient.PostAsync(emailJsServerUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    // Xử lý thành công
                    Console.WriteLine("Email đã được gửi thành công.");
                }
                else
                {
                    // Xử lý lỗi
                    var error = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Lỗi gửi email: {error}");
                }
            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ
                Console.WriteLine($"Lỗi: {ex.Message}");
            }
        }
        public string CreateVerifyToken(string? email)
        {
            var random = new Random();
            var token = random.Next(0, 999999).ToString();
            while (true)
            {
                if (token.Length < 6)
                {
                    var addNum = 6 - token.Length;
                    for (var i = 1; i <= addNum; i++)
                    {
                        token = "0" + token;
                    }
                }

                var user_id = _repositoryManager.User.FindByCondition(x => x.Email == email, true).Select(x => x.Id).FirstOrDefault();
                if (user_id > 0)
                {
                    if (_repositoryManager.VerifyToken.FindByCondition(x => x.Token == token && x.UserId == user_id, true) == null)
                    {
                        continue;
                    }
                    _repositoryManager.VerifyToken.Create(new VerifyToken
                    {
                        Token = token,
                        UserId = user_id
                    });
                    _repositoryManager.SaveAsync().Wait();
                    break;
                }
            }
            return token;
        }

        public bool BanUnband(int user_id, int user_effect)
        {
            var subcipt = _repositoryManager.Subscription.FindByCondition(x => x.UserId == user_id && x.UserSubId == user_effect, false).FirstOrDefault();

            if (subcipt != null)
            {
                subcipt.IsBanded = !subcipt.IsBanded;
                _repositoryManager.Subscription.Update(subcipt);
            }
            else
            {
                subcipt = new Subscription
                {
                    IsBanded = true,
                    IsSubcription = false,
                    UserId = user_id,
                    UserSubId = user_effect
                };
                _repositoryManager.Subscription.Create(subcipt);
            }
            _repositoryManager.SaveAsync().Wait();
            return true;
        }

        public bool BanUnbandLogin(int user_id)
        {
            var user = _repositoryManager.User.FindByCondition(X => X.Id == user_id, true).FirstOrDefault();
            if (user != null)
            {
                user.IsBanFromLogin = !user.IsBanFromLogin;
                _repositoryManager.SaveAsync().Wait();
                return user.IsBanFromLogin;
            }
            throw new NotImplementedException();
        }

        public bool CheckRemoveVefToken(UserVerifyToken info)
        {
            var user_id = _repositoryManager.User.FindByCondition(x => x.Email == info.Email, true).Select(x => x.Id).FirstOrDefault();
            if (user_id > 0)
            {
                var tokenInfo = _repositoryManager.VerifyToken.FindByCondition(x => x.UserId == user_id && x.Token == info.VerifyToken, true).FirstOrDefault();
                if (tokenInfo != null)
                {
                    _repositoryManager.VerifyToken.Delete(tokenInfo);
                    _repositoryManager.SaveAsync().Wait();
                    return true;
                }
            }
            return false;
        }

        public int CreateReport(int user_id, int userreport_id, AddReport report)
        {
            var saveReport = new Report
            {
                reportContent = report.Content,
                IdUserTo = user_id,
                IdUserFrom = userreport_id,
                TimeReport = DateTime.UtcNow.AddHours(7),
            };
            _repositoryManager.Report.Create(saveReport);
            _repositoryManager.SaveAsync().Wait();
            return saveReport.Id;
        }

        

        public bool ExistUserId(int userId)
        {
            return _repositoryManager.User.FindByCondition(x => x.Id == userId, false).FirstOrDefault() != null;
        }

        public List<UserSuggestion> FindUserByArea(List<string> areas)
        {
            var res = new List<UserSuggestion>();
            foreach (var area in areas)
            {
                var userInArea = _repositoryManager.User.FindByCondition(x => x.PlayingArea != null && x.PlayingArea.Contains(area), false)
                    .Select(x => new UserSuggestion
                    {
                        Id = x.Id,
                        Name = x.FullName,
                        Rating = x.Rate,
                        ShortProfile = x.SortProfile
                    }).ToList();
                res.AddRange(userInArea);
                if(res.Count > 16)
                {
                    break;
                }
            }
            return res;
        }

        public List<UserSuggestion> FindUserByPlayWays(List<string> ways, List<UserSuggestion> res)
        {
            foreach (var way in ways)
            {
                var userInArea = _repositoryManager.User.FindByCondition(x => x.PlayingWay != null && x.PlayingWay.Contains(way), false)
                    .Select(x => new UserSuggestion
                    {
                        Id = x.Id,
                        Name = x.FullName,
                        Rating = x.Rate,
                        ShortProfile = x.SortProfile
                    }).ToList();
                res.AddRange(userInArea);
                if (res.Count > 16)
                {
                    break;
                }
            }
            return res;
        }

        public List<UserSuggestion> FindUserBySkill(int skill, List<UserSuggestion> res)
        {
            var users = _repositoryManager.User.FindByCondition(x => x.PlayingLevel >= skill - 1 && x.PlayingLevel <= skill + 1, false)
                    .Select(x => new UserSuggestion
                    {
                        Id = x.Id,
                        Name = x.FullName,
                        Rating = x.Rate,
                        ShortProfile = x.SortProfile
                    }).ToList();
            if (users.Count > 0)
            {
                res.AddRange(users);
            }
            return res;
        }

        public async Task<List<User>> GetAllAccount()
        {
            var res = await _repositoryManager.User.FindAll(false)
                .Select( x => new User
                {
                    Id= x.Id,
                    DeviceToken = x.DeviceToken,
                    Email = x.Email,
                    FullName = x.FullName,
                    ImgUrl = x.ImgUrl,
                    IsActive = x.IsActive,
                    Notifications = x.Notifications,
                    PhoneNumber = x.PhoneNumber,
                    PlayingArea = x.PlayingArea,
                    PlayingLevel = x.PlayingLevel,
                    PlayingWay = x.PlayingWay,
                    Posts = x.Posts,
                    Rate = x.Rate,
                    SortProfile = x.SortProfile,Tokens = x.Tokens,
                    TotalRate = x.TotalRate,
                    Transactions = x.Transactions,
                    UserName = x.UserName,
                    UserPassword = x.UserPassword,
                    UserRatings = x.UserRatings
                }).ToListAsync();
            return res;
        }

        public List<BandedUsers> GetBandedUsers(int user_id)
        {
            var banded_user = _repositoryManager.Subscription.FindByCondition(x => x.UserId == user_id && x.IsBanded, true)
                .Include(x => x.UserSub)
                .Select(x => new BandedUsers
                {
                    UserId = x.UserSubId,
                    ImgUrl = x.UserSub.ImgUrl,
                    UserName = x.UserSub.UserName
                })
                .ToList();
            return banded_user;
        }

        public List<CommentInfos> GetComments(int user_id)
        {
            var comments = _repositoryManager.Comment.FindByCondition(x => x.TargetTo == "User" && x.IdTarget == user_id, true)
                .Select(x => new CommentInfos
                {
                    Content = x.Content,
                    SavedDate = x.SavedDate,
                    UserId = user_id
                }).ToList();

            for(var i = 0; i < comments.Count; i++)
            {
                var comment = comments[i];
                var userCmt = _repositoryManager.User.FindByCondition(x => x.Id == comment.UserId, true).FirstOrDefault();
                if (userCmt != null)
                {
                    comment.UserName = userCmt.UserName;
                    comment.TotalRate = userCmt.TotalRate;
                    comment.UserAvatar = userCmt.ImgUrl;
                    comments[i] = comment;
                }
            }
            return comments;
        }

        public UserInformation GetExistUser(LoginInformation info)
        {
            var res = new UserInformation();
            var user = _repositoryManager.User.FindByCondition(x => x.Email == info.Email && x.UserPassword == info.Password, true)
                .Include(x => x.Wallets)
                .FirstOrDefault();
            if (user != null)
            {
                var isNewUser = user.PlayingArea == null || user.PlayingLevel == 0 || user.PlayingWay == null;
                res = new UserInformation
                {
                    Avatar = user.ImgUrl,
                    Id = user.Id,
                    UserName = user.FullName,
                    Token = _jwtSupport.CreateToken(1, user.Id, isNewUser),
                    PlayingArea = user.PlayingArea,
                    PlayingLevel = user.PlayingLevel,
                    PlayingWay = user.PlayingWay,
                    IsNewUser = isNewUser,
                    FullName = user.FullName,
                    PhoneNumber = user.PhoneNumber,
                    SortProfile = user.SortProfile,
                    Balance = user.Wallets != null ? user.Wallets.ToList()[0].Balance : 0,
                };

                if(user.IsBanFromLogin)
                {
                    res.Id = -1;
                }

                user.LastLoginDate = DateTime.UtcNow.AddHours(7);
                _repositoryManager.SaveAsync().Wait();
            }
            return res;
        }

        public double? GetFriendly(int user_id)
        {
            var res = _repositoryManager.UserRating.FindByCondition(x=> x.IdUserRated == user_id, true).Select(x => x.Friendly).Average();
            return res;
        }

        public double? GetHelpful(int user_id)
        {
            var res = _repositoryManager.UserRating.FindByCondition(x => x.IdUserRated == user_id, true).Select(x => x.Helpful).Average();
            return res;
        }

        public double? GetLevelSkill(int user_id)
        {
            var res = _repositoryManager.UserRating.FindByCondition(x => x.IdUserRated == user_id, true).Select(x => x.LevelSkill).Average();
            return res;
        }

        public List<UserReport> GetReports(int user_id)
        {
            var res = _repositoryManager.Report.FindByCondition(x => x.IdUserTo == user_id, false)
                .Select(X => new UserReport
                {
                    Content = X.reportContent,
                    Id = X.Id,
                    SaveDate = X.TimeReport.Value.ToString("dd/MM/yyyy"),
                    Status = ((ReportStatus)X.Status).ToString(),
                    Title = X.ReportTitle
                }).ToList();
            return res;
        }

        public SelfProfile GetSelfProfile(int user_id)
        {
            var res = _repositoryManager.User.FindByCondition(x => x.Id == user_id, true).Select(x => new SelfProfile
            {
                FullName = x.FullName,
                ImgUrl = x.ImgUrl,
                PhoneNumber = x.PhoneNumber,
                SortProfile =x.SortProfile,
                UserName = x.UserName
            }).FirstOrDefault();
           return res;

        }

        public double? GetTrusted(int user_id)
        {
            var res = _repositoryManager.UserRating.FindByCondition(x => x.IdUserRated == user_id, true).Select(x => x.Trusted).Average();
            return res;
        }

        public List<string> GetUserAreas(int user_id)
        {
            var res = new List<string>();
            var user = _repositoryManager.User.FindByCondition(x => x.Id == user_id, false).FirstOrDefault();
            if (user != null)
            {
                if(user.PlayingArea != null)
                {
                    foreach (var area in user.PlayingArea.Split(";"))
                    {
                        res.Add(area.Trim().Replace(";", ""));
                    }
                }
            }
            return res;
        }

        public List<UserManaged> GetUserForManaged()
        {
            var res = _repositoryManager.User.FindAll(false)
                .OrderByDescending(x => x.LastLoginDate)
                .Select(x => new UserManaged
                {
                    CreateDate = x.CreateDate.Value.ToString("dd/MM/yyyy"),
                    FullName = x.FullName,
                    LastLogin = x.LastLoginDate.Value.ToString("dd/MM/yyyy"),
                    Role = x.UserRole == (int)UserRole.User ? "User" : "Admin",
                    Status = "Active",
                    UserId = x.Id
                })
                .ToList();
            return res;
        }

        public List<string> GetUserPlayWay(int user_id)
        {
            var res = new List<string>();
            var user = _repositoryManager.User.FindByCondition(x => x.Id == user_id, false).FirstOrDefault();
            if (user != null)
            {
                if (user.PlayingWay != null)
                {
                    foreach (var area in user.PlayingWay.Split(";"))
                    {
                        res.Add(area.Trim().Replace(";", ""));
                    }
                }
            }
            return res;
        }

        public UserProfile GetUserProfileSetting(int user_id)
        {
            var res = _repositoryManager.User.FindByCondition(x => x.Id == user_id, false)
                .Select(x => new UserProfile
                {
                    ImgUrl = x.ImgUrl,
                    FullName = x.FullName,
                    TotalRate = x.TotalRate,
                    SortProfile = x.SortProfile,
                }).FirstOrDefault();
            return res;
        }

        public int GetUserSkill(int user_id)
        {
            var res = 0;
            var user = _repositoryManager.User.FindByCondition(x => x.Id == user_id, false).FirstOrDefault();
            if (user != null)
            {
                res = user.PlayingLevel;
            }
            return res;
        }

        public bool IsAdmin(int user_id)
        {
            var res = false;
            var user = _repositoryManager.User.FindByCondition(x => x.Id == user_id, true).FirstOrDefault();
            if (user != null && user.UserRole != null)
            {
                res = user.UserRole == (int)UserRole.Admin;
            }
            return res;
        }

        public bool IsUserExist(string? email)
        {
            return _repositoryManager.User.FindByCondition(x => x.Email == email, false).FirstOrDefault() != null;
        }

        public int RegistUser(RegisInfomation info)
        {
            var user = new Entities.Models.User
            {
                Email = info.Email,
                FullName = info.FullName,
                PhoneNumber = info.PhoneNum,
                UserPassword = info.Password,
                UserName = info.UserName,
                CreateDate = DateTime.UtcNow.AddHours(7),
            };
            _repositoryManager.User.Create(user);
            _repositoryManager.SaveAsync().Wait();
            if(user.Id > 0)
            {
                _repositoryManager.Wallet.Create(new Wallet
                {
                    Balance = 0,
                    IdUser = user.Id,
                });
                _repositoryManager.SaveAsync().Wait();
            }
            return user.Id;
        }

        public int SaveComment(int user_id, int user_id_receive_comment, AddCommentToUser comment)
        {
            var saveComment = new Comment
            {
                Content = comment.Content,
                IdTarget = user_id_receive_comment,
                UserId = user_id,
                SavedDate = DateTime.UtcNow.AddHours(7),
                TargetTo = "User"
            };

            _repositoryManager.Comment.Create(saveComment);
            _repositoryManager.SaveAsync().Wait();
            return saveComment.Id;
        }

        public bool UpdatePassword(string email, UpdatePassword info)
        {
            var user = _repositoryManager.User.FindByCondition(x => x.Email == email, true).FirstOrDefault();
            if(user != null)
            {
                user.UserPassword = info.NewPassword;
                _repositoryManager.User.Update(user);
                _repositoryManager.SaveAsync().Wait();
                return true;
            }
            return false;
        }

        public async Task<ObjectResult> UpdateProfile(int user_id, UpdateProfileUser param, bool trackChanges)
        {
            var user = await _repositoryManager.User.FindByCondition(x => x.Id == user_id, trackChanges)
                .FirstOrDefaultAsync();
            if(user != null)
            {
                user.UserName = param.UserName;
                user.FullName = param.FullName;
                user.ImgUrl = await HandleImg(param.ImgUrl);
                user.PhoneNumber = param.PhoneNumber;
                user.SortProfile = param.SortProfile;
                user.PlayingArea = param.PlayingArea;
                _repositoryManager.User.Update(user);
                await _repositoryManager.SaveAsync();

                return new OkObjectResult(new { message = "Profile updated successfully" });
            }
            else
            {
                return new NotFoundObjectResult(new { message = "User with the specified ID not found" });

            }
        }
    }
}
