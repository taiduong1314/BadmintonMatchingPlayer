using Entities.Models;
using Entities.RequestObject;
using Microsoft.EntityFrameworkCore;
using Repositories.Intefaces;
using Services.Interfaces;

namespace Services.Implements
{
    public class TransactionServices : ITransactionServices
    {
        private readonly IRepositoryManager _repositoryManager;

        public TransactionServices(IRepositoryManager repositoryManager)
        {
            _repositoryManager = repositoryManager;
        }

        public async Task<int> CreateForBuySlot(TransactionCreateInfo info)
        {
            var slot = await _repositoryManager.Slot.FindByCondition(x => x.Id == info.IdSlot, false).FirstOrDefaultAsync();
            if (slot == null || slot.ContentSlot == null)
            {
                return 0;
            }

            var date = slot.ContentSlot.Split('/');
            var tran = new Transaction
            {
                DeadLine = new DateTime(int.Parse(date[2]), int.Parse(date[1]), int.Parse(date[0])).AddDays(1),
                IdSlot = slot.Id,
                IdUser = info.IdUser,
                MoneyTrans = slot.Price,
                MethodTrans = "buy_slot",
                TypeTrans = "buy _slot",
                Status = (int)TransactionStatus.Processing,
            };
            _repositoryManager.Transaction.Create(tran);
            await _repositoryManager.SaveAsync();
            return tran.Id;
        }

        public bool ExistTran(int tran_id)
        {
            return _repositoryManager.Transaction.FindByCondition(x => x.Id == tran_id, false).FirstOrDefault() != null;
        }

        public async Task UpdateStatus(int tran_id, TransactionStatus tranStatus)
        {
            var tran = await _repositoryManager.Transaction.FindByCondition(x => x.Id == tran_id, true)
                .Include(x => x.IdSlotNavigation)
                .ThenInclude(x => x.IdPostNavigation)
                .FirstOrDefaultAsync();
            if (tran != null)
            {
                if (tranStatus == TransactionStatus.PaymentSuccess)
                {
                    //implement hangfire here
                    //Create auto charge for played
                }
                else if (tranStatus == TransactionStatus.Reporting)
                {
                    //implement hangfire here
                    //Delete auto charge
                }else if (tranStatus == TransactionStatus.Played)
                {

                    //Delete auto charge
                    var tranHistory = new HistoryTransaction
                    {
                        IdUserFrom = tran.IdUser,
                        IdUserTo = tran.IdSlotNavigation.IdPostNavigation.IdUserTo,
                        IdTransaction = tran.Id,
                        MoneyTrans = tran.MoneyTrans,
                        Status = true,
                        Deadline = tran.DeadLine
                    };
                    _repositoryManager.HistoryTransaction.Create(tranHistory);
                    await _repositoryManager.SaveAsync();
                }
                tran.Status = (int)tranStatus;
                await _repositoryManager.SaveAsync();
            }
        }
    }
}
