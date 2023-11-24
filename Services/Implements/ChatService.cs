using Entities.Models;
using Entities.RequestObject;
using Entities.ResponseObject;
using Microsoft.EntityFrameworkCore;
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

        public async Task<List<ChatInfos>> GetChatRoom(int transactionId)
        {
            var slots = await _repositoryManager.Slot
                .FindByCondition(x => x.TransactionId == transactionId, false)
                .Include(x => x.IdPostNavigation)
                .ToListAsync();

            var playDate = string.Empty;
            var res = new List<ChatInfos>();
            foreach (var slot in slots)
            {
                if (playDate != slot.ContentSlot)
                {
                    var room = await _repositoryManager.ChatRoom
                        .FindByCondition(x => x.Code == $"{slot.IdPost}_{slot.ContentSlot}", false)
                        .Select(x => new ChatInfos
                        {
                            PlayDate = slot.ContentSlot,
                            RoomId = x.Id
                        })
                        .FirstOrDefaultAsync();

                    if (room == null)
                    {
                        var roomEnt = new Entities.Models.ChatRoom
                        {
                            Code = $"{slot.IdPost}_{slot.ContentSlot}",
                            Name = $"Play date: {slot.ContentSlot}",
                            CoverImage = slot.IdPostNavigation.ImgUrl
                        };
                        _repositoryManager.ChatRoom.Create(roomEnt);
                        await _repositoryManager.SaveAsync();

                        room = new ChatInfos
                        {
                            PlayDate = slot.ContentSlot,
                            RoomId = roomEnt.Id
                        };
                    }
                    else
                    {
                        var userJoined = await _repositoryManager.ChatRoomUser
                            .FindByCondition(x => x.UserId == slot.IdUser && x.RoomId == room.RoomId, false)
                            .FirstOrDefaultAsync();

                        if (userJoined == null)
                        {
                            _repositoryManager.ChatRoomUser.Create(new Entities.Models.UserChatRoom
                            {
                                RoomId = room.RoomId,
                                UserId = slot.IdUser
                            });
                            await _repositoryManager.SaveAsync();
                        }
                    }
                    res.Add(room);
                }
            }
            return res;
        }

        public async Task<List<MessageDetail>> GetRoomDetail(int roomIds, int pageSize, int pageNum)
        {
            var ss = await _repositoryManager.Message.FindByCondition(x => x.RoomId == roomIds, false).ToListAsync();
            var msg = await _repositoryManager.Message.FindByCondition(x => x.RoomId == roomIds, false)
                .OrderByDescending(x => x.Id)
                .Skip((pageNum - 1) * pageSize)
                .Take(pageSize)
                .Include(x => x.User)
                .Select(x => new MessageDetail
                {
                    Message = x.Message,
                    MessageId = x.Id,
                    SendTime = x.SendTime.ToString("ddd HH:mm"),
                    SendUserName = x.User.FullName,
                    UserId = x.UserId.Value
                })
                .OrderBy(x => x.MessageId)
                .ToListAsync();
            return msg;
        }

        public async Task<List<JoinedChatRoom>> GetRoomOfUser(int user_id)
        {
            var joinedRoom = await _repositoryManager.ChatRoomUser.FindByCondition(x => x.UserId == user_id, false)
                .Include(x => x.ChatRoom)
                .ThenInclude(x => x.Messages)
                .ThenInclude(x => x.User)
                .ToListAsync();

            var res = new List<JoinedChatRoom>();
            if (joinedRoom.Count() > 0)
            {
                res = joinedRoom.Select(x => new JoinedChatRoom
                {
                    ChatTitle = x.ChatRoom.Name,
                    CoverImg = x.ChatRoom.CoverImage,
                    LastSendMsg = x.ChatRoom.Messages.Count > 0 ? x.ChatRoom.Messages.Select(y => $"{y.User.FullName}: {y.Message}").Last() : string.Empty,
                    LastSendTime = x.ChatRoom.Messages.Count > 0 ? x.ChatRoom.Messages.Select(y => y.SendTime).Last().ToString("dd/MM/yyyy HH:mm") : string.Empty,
                    RoomId = x.RoomId.Value
                }).ToList();
            }
            return res;
        }

        public async Task<bool> JoinRoom(int user_id, int room_id)
        {
            var userJoined = await _repositoryManager.ChatRoomUser
                            .FindByCondition(x => x.UserId == user_id && x.RoomId == room_id, false)
                            .FirstOrDefaultAsync();

            if (userJoined == null)
            {
                _repositoryManager.ChatRoomUser.Create(new Entities.Models.UserChatRoom
                {
                    RoomId = room_id,
                    UserId = user_id
                });
                await _repositoryManager.SaveAsync();
            }

            return true;
        }

        public async Task<bool> LeaveRoom(int user_id, int room_id)
        {
            var userJoined = await _repositoryManager.ChatRoomUser
                .FindByCondition(x => x.UserId == user_id && x.RoomId == room_id, false)
                .FirstOrDefaultAsync();

            if (userJoined != null)
            {
                _repositoryManager.ChatRoomUser.Delete(userJoined);
                await _repositoryManager.SaveAsync();
            }

            return true;
        }

        public async Task<User> SendMessage(int user_id, SendMessageRequest info)
        {
            var room = await _repositoryManager.ChatRoom.FindByCondition(x => x.Id == info.RoomId, false).FirstOrDefaultAsync();

            if (room == null)
                return null;

            var msg = new Messages
            {
                Message = info.Message,
                RoomId = info.RoomId,
                SendTime = DateTime.UtcNow.AddHours(7),
                UserId = user_id
            };
            _repositoryManager.Message.Create(msg);
            await _repositoryManager.SaveAsync();
            var res = await _repositoryManager.User.FindByCondition(x => x.Id == user_id, false).FirstOrDefaultAsync();
            return res;
        }
    }
}
