using Entities.ResponseObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IChatServices
    {
        List<ChatInfos> GetChatRoom(int transactionId);
    }
}
