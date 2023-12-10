using BadmintonMatching.RealtimeHub;
using Entities.Models;
using Entities.RequestObject;
using Entities.ResponseObject;
using Hangfire;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Services.Interfaces;

namespace BadmintonMatching.Controllers
{
    [Route("api/chat")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly IChatServices _chatServices;
        private readonly IHubContext<ChatHub> _chatHub;
        private readonly IUserServices _userServices;
        private readonly INotificationServices _notificationServices;

        public ChatController(IChatServices chatServices, 
            IHubContext<ChatHub> chatHub,
            IUserServices userServices,
            INotificationServices notificationServices)
        {
            _chatServices = chatServices;
            _chatHub = chatHub;
            _userServices = userServices;
            _notificationServices = notificationServices;
        }

        [HttpPost]
        [Route("user/{user_id}/room/{room_id}/join")]
        public async Task<IActionResult> JoinChatRoom(int user_id, int room_id)
        {
            var joinSuccess = await _chatServices.JoinRoom(user_id, room_id);

            if (joinSuccess)
            {
                return Ok(new SuccessObject<object> { Data = true, Message = Message.SuccessMsg});
            }
            else
            {
                return Ok(new SuccessObject<object> { Message = "Fail to join" });
            }
        }

        [HttpGet]
        [Route("user/{user_id}/rooms")]
        public async Task<IActionResult> GetJoinedChatRoom(int user_id)
        {
            var joinedRooms = await _chatServices.GetRoomOfUser(user_id);

            if(joinedRooms.Count() > 0)
            {
                return Ok(new SuccessObject<List<JoinedChatRoom>> { Data = joinedRooms, Message = Message.SuccessMsg });
            }
            else
            {
                return Ok(new SuccessObject<List<JoinedChatRoom>> { Data = null, Message = "No chat room joined" });
            }
        }

        [HttpGet]
        [Route("{room_id}/detail")]
        public async Task<IActionResult> GetRoomDetail(int room_id, [FromQuery] int pageSize, [FromQuery] int pageNum)
        {
            var msgs = await _chatServices.GetRoomDetail(room_id, pageSize, pageNum);

            return Ok(new SuccessObject<List<MessageDetail>> { Data = msgs, Message = Message.SuccessMsg});
        }

        [HttpPost]
        [Route("user/{user_id}")]
        public async Task<IActionResult> SendMessage(int user_id, SendMessageRequest info)
        {
            var user = await _chatServices.SendMessage(user_id, info);
            
            if (user != null)
            {
                await _chatHub.Clients.User(info.RoomId.ToString()).SendAsync(info.Message, $"{user.FullName} Image:{user.ImgUrl}");
                await _chatHub.Clients.All.SendAsync(info.Message, $"{user.FullName} Image:{user.ImgUrl}");
                return Ok(new SuccessObject<object> { Data = true, Message = Message.SuccessMsg });
            }
            else
            {
                return Ok(new SuccessObject<object> { Data = null, Message = "Fail to saved message" });
            }
        }

        [HttpPost]
        [Route("by_admin/{admin_id}/report/{report_id}")]
        public async Task<IActionResult> CreateRoomFromReport(int admin_id, int report_id)
        {
            try
            {
                if (!_userServices.IsAdmin(admin_id))
                {
                    throw new Exception("Not admin to create");
                }
                int roomId = await _chatServices.CreateRoom(admin_id, report_id);

                if(roomId == 0)
                {
                    throw new Exception("Fail to create room");
                }

                return Ok(new SuccessObject<object> { Message = Message.SuccessMsg, Data = new { RoomId = roomId} });
            }
            catch(Exception ex) 
            {
                return Ok(new SuccessObject<object> { Message = ex.Message });
            }
        }
    }
}
