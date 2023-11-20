using Entities.ResponseObject;
using Repositories.Intefaces;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            
            foreach(var slot in slots)
            {

            }

            return new List<ChatInfos>();
        }
    }
}
