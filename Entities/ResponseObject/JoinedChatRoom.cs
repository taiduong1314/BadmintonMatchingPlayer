namespace Entities.ResponseObject
{
    public class JoinedChatRoom
    {
        public int RoomId { get; set; }
        public string? ChatTitle { get; set; }
        public string? CoverImg { get; set; }
        public string? LastSendMsg { get; set; }
        public string? LastSendTime { get; set; }
    }
}
