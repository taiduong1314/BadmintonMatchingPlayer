using CloudinaryDotNet.Actions;
using Entities.Models;
using Entities.RequestObject;
using Entities.ResponseObject;
using Microsoft.AspNetCore.SignalR;
using Services.Implements;
using Services.Interfaces;

namespace BadmintonMatching.RealtimeHub
{
    public class ChatHub : Hub
    {
        private readonly IChatServices _chatService;
        private readonly IUserServices _userServices;

        public ChatHub(IChatServices chatService, IUserServices userServices)
        {
            _chatService = chatService;
            _userServices = userServices;
        }

        public async Task JoinRoom(int room_id, int user_id)
        {
            var joinSuccess = await _chatService.JoinRoom(user_id, room_id);

            if(!joinSuccess)
            {
                throw new Exception("không thể tham gia đoạn hội thoại");
            }

            await Groups.AddToGroupAsync(Context.ConnectionId, room_id.ToString());

            var fullName = _userServices.GetSelfProfile(user_id).FullName;

            await Clients.Group(room_id.ToString()).SendAsync("ReceiveMessage", "Bot chat", $"{fullName} đã tham gia hội thoại.");
        }

        public async Task LeaveRoom(int room_id, int user_id)
        {
            var leaveSuccess = await _chatService.LeaveRoom(user_id, room_id);

            if(!leaveSuccess)
            {
                throw new Exception("không thể rời đoạn hội thoại");
            }

            var fullName = _userServices.GetSelfProfile(user_id).FullName;

            await Clients.Group(room_id.ToString()).SendAsync("ReceiveMessage", "Bot chat", $"{fullName} đã tham gia hội thoại.");
        }

        public async Task SendMessage(string message, int user_id)
        {
            Console.WriteLine($"ContextId in chat: {Context.ConnectionId}");
            var user = await _chatService.SendMessage(user_id, new SendMessageRequest
            {
                Message = message,
                RoomId = int.Parse(Context.ConnectionId)
            });

            if (user != null)
            {
                await Clients.Group(Context.ConnectionId)
                    .SendAsync("ReceiveMessage", $"{user.FullName}", message);
            }
            else
            {
                throw new Exception("không thể gửi tin nhắn");
            }
        }
    }
}
