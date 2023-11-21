using Entities.Models;
using Entities.ResponseObject;
using Repositories.Intefaces;
using Services.Interfaces;

namespace Services.Implements
{
    public class ChatService : IChatServices
    {
        private readonly IRepositoryManager _repositoryManager;

        public ChatService(IRepositoryManager repositoryManager)
        {
            _repositoryManager = repositoryManager;
        }

        public List<ChatInfos> GetChatRoom(int transactionId)
        {
            var slots = _repositoryManager.Slot.FindByCondition(x => x.TransactionId == transactionId, false).ToList();

            var playDate = string.Empty;
            var res = new List<ChatInfos>();
            foreach (var slot in slots)
            {
                if (playDate != slot.ContentSlot)
                {
                    var room = _repositoryManager.ChatRoom
                        .FindByCondition(x => x.Code == $"{slot.IdPost}_{slot.ContentSlot}", false)
                        .Select(x => new ChatInfos
                        {
                            ClientName = x.Code,
                            RoomId = x.Id
                        })
                        .FirstOrDefault();

                    if (room == null)
                    {
                        var roomEnt = new Entities.Models.ChatRoom
                        {
                            Code = $"{slot.IdPost}_{slot.ContentSlot}",
                            Name = $"Play date: {slot.ContentSlot}"
                        };
                        _repositoryManager.ChatRoom.Create(roomEnt);
                        _repositoryManager.SaveAsync().Wait();

                        _repositoryManager.ChatRoomUser.Create(new Entities.Models.UserChatRoom
                        {
                            RoomId = roomEnt.Id,
                            UserId = slot.IdUser
                        });
                        _repositoryManager.SaveAsync().Wait();

                        room = new ChatInfos
                        {
                            ClientName = roomEnt.Code,
                            RoomId = roomEnt.Id
                        };
                    }
                    else
                    {
                        var userJoined = _repositoryManager.ChatRoomUser
                            .FindByCondition(x => x.UserId == slot.IdUser && x.RoomId == room.RoomId, false)
                            .FirstOrDefault();

                        if(userJoined == null)
                        {
                            _repositoryManager.ChatRoomUser.Create(new Entities.Models.UserChatRoom
                            {
                                RoomId = room.RoomId,
                                UserId = slot.IdUser
                            });
                            _repositoryManager.SaveAsync().Wait();
                        }
                    }
                    res.Add(room);
                }
            }
            return res;
        }
    }
}
