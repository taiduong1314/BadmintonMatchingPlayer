using Entities.Models;
using Entities.RequestObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface ITransactionServices
    {
        Task<int> CreateForBuySlot(TransactionCreateInfo info);
        bool ExistTran(int tran_id);
        Task UpdateStatus(int tran_id, TransactionStatus tranStatus);
    }
}
