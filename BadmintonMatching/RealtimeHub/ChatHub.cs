using Microsoft.AspNetCore.SignalR;

namespace BadmintonMatching.RealtimeHub
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(string message, string user)
        {
            await Clients.All.SendAsync(message, user);
        }
        public async Task SendMessageByRoom(string message, string user, int roomId)
        {
            await Clients.User(roomId.ToString()).SendAsync(message, user);
        }
    }
}
