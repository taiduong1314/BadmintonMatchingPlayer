using CorePush.Apple;
using CorePush.Google;
using Entities.Models;
using Entities.RequestObject;
using Entities.ResponseObject;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Repositories.Intefaces;
using Services.Interfaces;
using static Entities.ResponseObject.GoogleNotification;

namespace Services.Implements
{
    public class NotificationServices : INotificationServices
    {
        private readonly IOptions<FcmNotificationSetting> _settings;
        private readonly IRepositoryManager _repositoryManager;

        public NotificationServices(IOptions<FcmNotificationSetting> settings, IRepositoryManager repositoryManager)
        {
            _settings = settings;
            _repositoryManager = repositoryManager;
        }

        public async Task<bool> ReadedAll(ReadedNoti info)
        {
            foreach(var id in info.NotiIds)
            {
                var noti = _repositoryManager.Notification.FindByCondition(x => x.Id == id, false)
                    .FirstOrDefault();

                noti.IsRead = true;
                _repositoryManager.Notification.Update(noti);
            }
            await _repositoryManager.SaveAsync();
            return true;
        }

        public async Task<NotiResponseModel> SendNotification(NotificationModel notificationModel)
        {
            var res = new NotiResponseModel();

            try
            {
                var payloadData = new DataPayload
                {
                    Body = notificationModel.Body,
                    Title = notificationModel.Title,
                };

                var notification = new GoogleNotification
                {
                    Data = payloadData,
                    Notification = payloadData
                };

                if (notificationModel.IsAndroiodDevice)
                {
                    var settings = new FcmSettings
                    {
                        SenderId = _settings.Value.SenderId,
                        ServerKey = _settings.Value.ServerKey,
                    };
                    var httpClient = new HttpClient();
                    var authorizationKey = $"key={settings.ServerKey}";
                    var deviceToken = notificationModel.DeviceId;

                    httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", authorizationKey);
                    httpClient.DefaultRequestHeaders.Accept
                        .Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                    var fcm = new FcmSender(settings, httpClient);
                    var fcmResponse = await fcm.SendAsync(deviceToken, notification);

                    if (fcmResponse.IsSuccess())
                    {
                        res.IsSuccess = true;
                        res.Message = "Notification sent successfully";
                    }
                    else
                    {
                        res.IsSuccess = false;
                        res.Message = fcmResponse.Results[0].Error;
                    }
                }
                else
                {
                    //Pending
                }
                return res;
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = "Some thing went wrong";
                return res;
            }
        }

        public async Task<NotiResponseModel> SendNotification(int userId, string title, string message, NotificationType type, int referenceInfo)
        {
            var user = await _repositoryManager.User.FindByCondition(x => x.Id == userId, false).FirstOrDefaultAsync();

            if(user == null)
            {
                throw new Exception("User not found");
            }

            var savedNoti = new Notification
            {
                Content = message,
                IsRead = false,
                NotiDate = DateTime.UtcNow.AddHours(7),
                Title = title,
                UserId = userId,
                About = (int)type,
                ReferenceInfo = referenceInfo
            };

            _repositoryManager.Notification.Create(savedNoti);
            await _repositoryManager.SaveAsync();

            //if(user.LogingingDevice == null)
            //{
            //    return new NotiResponseModel 
            //    {
            //        IsSuccess = true,
            //        Message = "User don't have device to send noti"
            //    };
            //}

            //var res = await SendNotification(new NotificationModel
            //{
            //    Body = message,
            //    DeviceId = user.LogingingDevice,
            //    IsAndroiodDevice = user.IsAndroidDevice,
            //    Title = title
            //});

            //return res;

            return new NotiResponseModel
            {
                IsSuccess = true,
                Message = "Send Success"
            };
        }

        public async Task<NotiResponseModel> SendNotification(List<int> userIds, string title, string message, NotificationType type, int referenceInfo)
        {
            foreach(var userId in userIds)
            {
                var user = await _repositoryManager.User.FindByCondition(x => x.Id == userId, false).FirstOrDefaultAsync();

                if (user == null)
                {
                    throw new Exception("User not found");
                }

                var savedNoti = new Notification
                {
                    Content = message,
                    IsRead = false,
                    NotiDate = DateTime.UtcNow.AddHours(7),
                    Title = title,
                    UserId = userId,
                    About = (int)type,
                    ReferenceInfo = referenceInfo
                };

                _repositoryManager.Notification.Create(savedNoti);

                //if(user.LogingingDevice == null)
                //{
                //    return new NotiResponseModel 
                //    {
                //        IsSuccess = true,
                //        Message = "User don't have device to send noti"
                //    };
                //}

                //var res = await SendNotification(new NotificationModel
                //{
                //    Body = message,
                //    DeviceId = user.LogingingDevice,
                //    IsAndroiodDevice = user.IsAndroidDevice,
                //    Title = title
                //});

                //return res;
            }
            await _repositoryManager.SaveAsync();

            return new NotiResponseModel
            {
                IsSuccess = true,
                Message = "Send Success"
            };
        }
    }
}
