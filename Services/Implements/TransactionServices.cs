using Entities.Models;
using Entities.RequestObject;
using Entities.ResponseObject;
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
            var slot = await _repositoryManager.Slot.FindByCondition(x => x.Id == info.IdSlot[0], false).FirstOrDefaultAsync();
            if (slot == null || slot.ContentSlot == null)
            {
                return 0;
            }

            var date = slot.ContentSlot.Split('/');
            var tran = new Transaction
            {
                DeadLine = new DateTime(int.Parse(date[2]), int.Parse(date[1]), int.Parse(date[0])).AddDays(1),
                IdUser = info.IdUser,
                MoneyTrans = slot.Price*info.IdSlot.Count,
                MethodTrans = "buy_slot",
                TypeTrans = "buy _slot",
                TimeTrans = DateTime.UtcNow,
                Status = (int)TransactionStatus.Processing,
            };
            _repositoryManager.Transaction.Create(tran);
            await _repositoryManager.SaveAsync();
            if(tran.Id > 0)
            {
                var slots = _repositoryManager.Slot.FindByCondition(x => info.IdSlot.Contains(x.Id), true).ToList();
                tran.Slots = slots;
                _repositoryManager.Transaction.Update(tran);
                await _repositoryManager.SaveAsync();
            }
            return tran.Id;
        }

        public async Task DeleteSlot(int transaction_id)
        {
            var slots = _repositoryManager.Slot.FindByCondition(x => x.TransactionId == transaction_id, true).ToList();
            foreach(var slot in slots)
            {
                _repositoryManager.Slot.Delete(slot);
            }
            await _repositoryManager.SaveAsync();
        }

        public async Task DeleteTran(int transaction_id)
        {
            var transaction = await _repositoryManager.Transaction.FindByCondition(x => x.Id == transaction_id, true).FirstOrDefaultAsync();
            if(transaction != null)
            {
                _repositoryManager.Transaction.Delete(transaction);
                await _repositoryManager.SaveAsync();
            }
        }

        public bool ExistTran(int tran_id)
        {
            return _repositoryManager.Transaction.FindByCondition(x => x.Id == tran_id, false).FirstOrDefault() != null;
        }

        public async Task<TransactionDetail> GetDetail(int transaction_id)
        {
            var tran = await _repositoryManager.Transaction.FindByCondition(x => x.Id ==  transaction_id, false)
                .Select(x => new TransactionDetail
                {
                    Id = x.Id,
                    BuyerName = x.IdUserNavigation.FullName,
                    PayTime = x.TimeTrans.Value.ToString("f"),
                    SlotCount = x.Slots.Count,
                    Slots = x.Slots.Select(y => new SlotBuy
                    {
                        Id = y.Id,
                        PlayDate = y.ContentSlot
                    }).ToList(),
                    Total = x.MoneyTrans.Value.ToString(),
                }).FirstOrDefaultAsync();
            if(tran != null && tran.Slots != null && tran.Slots[0] != null)
            {
                var slotId = tran.Slots[0].Id;
                var slot = await _repositoryManager.Slot.FindByCondition(x => x.Id == slotId, false).Include(x =>x.IdPostNavigation).FirstOrDefaultAsync();
                if(slot != null && slot.IdPostNavigation != null)
                {
                    tran.Post = new PostInTransaction
                    {
                        Address = slot.IdPostNavigation.AddressSlot,
                        EndTime = slot.IdPostNavigation.EndTime,
                        Id = slot.IdPostNavigation.Id,
                        ImageUrls = slot.IdPostNavigation.ImageUrls,
                        PricePerSlot = slot.IdPostNavigation.PriceSlot.ToString(),
                        StartTime = slot.IdPostNavigation.StartTime,
                        Title = slot.IdPostNavigation.Title,
                        TitleImage = slot.IdPostNavigation.ImgUrl
                    };
                }
            }
            return tran != null ? tran : new TransactionDetail { Id = 0 };
        }

        public async Task<List<TransactionInfo>> GetOfUser(int user_id)
        {
            var trans = await _repositoryManager.Transaction.FindByCondition(x => x.IdUser == user_id, false)
                .Include(x => x.Slots)
                .Select(x => new TransactionInfo
                {
                    Id = x.Id,
                    MoneyPaied = x.MoneyTrans.ToString(),
                    Slots = x.Slots.Select(y => new SlotBuy
                    {
                        Id = y.Id,
                        PlayDate = y.ContentSlot
                    }).ToList()
                }).ToListAsync();
            return trans;
        }

        public async Task<Transaction> GetTransaction(int transaction_id)
        {
            var tran = await _repositoryManager.Transaction.FindByCondition(x => x.Id == transaction_id, false)
                .FirstOrDefaultAsync();
            return tran == null ? new Transaction { Id = 0 } : tran;
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
