using Entities.ResponseObject;

namespace Services.Interfaces
{
    public interface INotificationServices
    {
        Task<NotiResponseModel> SendNotification(NotificationModel notificationModel);
        Task<NotiResponseModel> SendNotification(int userId, string title, string message);
    }
}
