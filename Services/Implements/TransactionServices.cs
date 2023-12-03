﻿using Entities.Models;
using Entities.RequestObject;
using Entities.ResponseObject;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using Repositories.Intefaces;
using Services.Interfaces;
using System.Globalization;

namespace Services.Implements
{
    public class TransactionServices : ITransactionServices
    {
        private readonly IRepositoryManager _repositoryManager;

        public TransactionServices(IRepositoryManager repositoryManager)
        {
            _repositoryManager = repositoryManager;
        }

        public async Task<Transaction> CreateForBuySlot(TransactionCreateInfo info)
        {
            var slots = await _repositoryManager.Slot.FindByCondition(x => info.IdSlot.Contains(x.Id) && !x.IsDeleted, false).ToListAsync();
            if (slots.Count() == 0)
            {
                return null;
            }

            var deadLine = DateTime.MinValue;
            decimal price = 0;

            foreach(var slot in slots)
            {
                var slotTime = DateTime.Parse(slot.ContentSlot);
                if(slotTime > deadLine)
                {
                    deadLine = slotTime;
                }
                price += slot.Price.Value;
            }

            var tran = new Transaction
            {
                DeadLine = deadLine.AddDays(1),
                IdUser = info.IdUser,
                MoneyTrans = price,
                MethodTrans = "buy_slot",
                TypeTrans = "buy _slot",
                TimeTrans = DateTime.UtcNow.AddHours(7),
                Status = (int)TransactionStatus.Processing,
            };
            _repositoryManager.Transaction.Create(tran);
            await _repositoryManager.SaveAsync();
            if (tran.Id > 0)
            {
                var slotsEnt = _repositoryManager.Slot.FindByCondition(x => info.IdSlot.Contains(x.Id), true).ToList();
                tran.Slots = slotsEnt;
                _repositoryManager.Transaction.Update(tran);
                await _repositoryManager.SaveAsync();
            }
            return tran;
        }

        public async Task DeleteSlot(int transaction_id)
        {
            var slots = _repositoryManager.Slot.FindByCondition(x => x.TransactionId == transaction_id, true).ToList();
            foreach (var slot in slots)
            {
                _repositoryManager.Slot.Delete(slot);
            }
            await _repositoryManager.SaveAsync();
        }

        public async Task DeleteTran(int transaction_id)
        {
            var transaction = await _repositoryManager.Transaction.FindByCondition(x => x.Id == transaction_id, true).FirstOrDefaultAsync();
            if (transaction != null)
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
            var tran = await _repositoryManager.Transaction.FindByCondition(x => x.Id == transaction_id, false)
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

            tran.IsCancel = tran.Slots.Select(x => DateTime.ParseExact(x.PlayDate, "dd/MM/yyyy", CultureInfo.InvariantCulture)).Min() > DateTime.UtcNow.AddHours(7);

            if (tran != null && tran.Slots != null && tran.Slots[0] != null)
            {
                var slotId = tran.Slots[0].Id;
                var slot = await _repositoryManager.Slot.FindByCondition(x => x.Id == slotId, false)
                    .Include(x => x.IdPostNavigation)
                    .ThenInclude(x => x.IdUserToNavigation)
                    .FirstOrDefaultAsync();
                if (slot != null && slot.IdPostNavigation != null)
                {
                    tran.Post = new PostInTransaction
                    {
                        Address = slot.IdPostNavigation.AddressSlot,
                        Id = slot.IdPostNavigation.Id,
                        ImageUrls = slot.IdPostNavigation.ImageUrls.Split(";").ToList(),
                        Title = slot.IdPostNavigation.Title,
                        TitleImage = slot.IdPostNavigation.ImgUrl,
                        CategorySlot = slot.IdPostNavigation.CategorySlot,
                        CreateUser = slot.IdPostNavigation.IdUserToNavigation.FullName
                    };
                    foreach (var infoSlot in slot.IdPostNavigation.SlotsInfo.Split(";"))
                    {
                        var info = new SlotInfo(infoSlot);
                        if (info.StartTime.Value.ToString("dd/MM/yyyy") == slot.ContentSlot)
                        {
                            tran.Post.StartTime = info.StartTime.Value.ToString("dd/MM/yyyy HH:mm");
                            tran.Post.EndTime = info.EndTime.Value.ToString("dd/MM/yyyy HH:mm");
                            tran.Post.PricePerSlot = info.Price.ToString();
                            break;
                        }
                    }
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

        public bool IsFromTwoPost(List<int>? idSlot)
        {
            var idPosts = _repositoryManager.Slot.FindByCondition(x => idSlot.Contains(x.Id), false)
                .Select(x => x.IdPost)
                .ToList();

            var postId = idPosts[0];
            foreach (var id in idPosts)
            {
                if (id != postId)
                    return true;
            }
            return false;
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
                    var deadline = DateTime.SpecifyKind(tran.DeadLine.Value, DateTimeKind.Local);
                    var scheduledId = BackgroundJob.Schedule(() => UpdateStatus(tran.Id, TransactionStatus.Played), deadline);
                    var newJob = new HangfireJob { ScheduledId = scheduledId, TransactionId = tran.Id };
                    _repositoryManager.HangfireJob.Create(newJob);
                    await _repositoryManager.SaveAsync();
                }
                else if (tranStatus == TransactionStatus.Reporting)
                {
                    var scheduled = await _repositoryManager.HangfireJob.FindByCondition(x => x.TransactionId == tran.Id, true).FirstOrDefaultAsync();
                    
                    if(scheduled != null)
                    {
                        BackgroundJob.Delete(scheduled.ScheduledId);
                        _repositoryManager.HangfireJob.Delete(scheduled);
                        await _repositoryManager.SaveAsync();
                    }
                }
                else if (tranStatus == TransactionStatus.Played)
                {
                    var idUserTo = await _repositoryManager.Slot
                        .FindByCondition(x => x.TransactionId == tran.Id, false)
                        .Include(x => x.IdPostNavigation)
                        .Select(x => x.IdPostNavigation.IdUserTo)
                        .FirstOrDefaultAsync();

                    var tranHistory = new HistoryTransaction
                    {
                        IdUserFrom = tran.IdUser,
                        IdUserTo = idUserTo,
                        IdTransaction = tran.Id,
                        MoneyTrans = tran.MoneyTrans,
                        Status = true,
                        Deadline = tran.DeadLine
                    };

                    _repositoryManager.HistoryTransaction.Create(tranHistory);

                    var wallet = await _repositoryManager.Wallet.FindByCondition(x => x.IdUser == tranHistory.IdUserTo, true).FirstOrDefaultAsync();
                    if(wallet != null)
                    {
                        wallet.Balance += tranHistory.MoneyTrans;

                        _repositoryManager.HistoryWallet.Create(new HistoryWallet
                        {
                            Amount = tranHistory.MoneyTrans.ToString(),
                            IdUser = tranHistory.IdUserTo,
                            IdWallet = wallet.Id,
                            Status = (int)HistoryWalletStatus.Success,
                            Time = DateTime.UtcNow.AddHours(7)
                        });
                    }

                    var scheduled = await _repositoryManager.HangfireJob.FindByCondition(x => x.TransactionId == tran.Id, true).FirstOrDefaultAsync();

                    if (scheduled != null)
                    {
                        BackgroundJob.Delete(scheduled.ScheduledId);
                        _repositoryManager.HangfireJob.Delete(scheduled);
                    }
                }
                else if (tranStatus == TransactionStatus.ReportResolved)
                {
                    var tranHistory = new HistoryTransaction
                    {
                        IdUserFrom = tran.IdUser,
                        IdUserTo = tran.IdUser,
                        IdTransaction = tran.Id,
                        MoneyTrans = tran.MoneyTrans,
                        Status = true,
                        Deadline = tran.DeadLine
                    };

                    _repositoryManager.HistoryTransaction.Create(tranHistory);

                    var wallet = await _repositoryManager.Wallet.FindByCondition(x => x.IdUser == tranHistory.IdUserTo, true).FirstOrDefaultAsync();
                    if(wallet != null)
                    {
                        wallet.Balance += tranHistory.MoneyTrans;

                        _repositoryManager.HistoryWallet.Create(new HistoryWallet
                        {
                            Amount = tranHistory.MoneyTrans.ToString(),
                            IdUser = tranHistory.IdUserTo,
                            IdWallet = wallet.Id,
                            Status = (int)HistoryWalletStatus.Success,
                            Time = DateTime.UtcNow.AddHours(7)
                        });
                    }
                }
                tran.Status = (int)tranStatus;
                await _repositoryManager.SaveAsync();
            }
        }
    }
}
