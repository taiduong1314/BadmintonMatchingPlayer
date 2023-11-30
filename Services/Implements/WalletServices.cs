using Entities.Models;
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
    public class WalletServices : IWalletServices
    {
        private readonly IRepositoryManager _repositoryManager;

        public WalletServices(IRepositoryManager repositoryManager)
        {
            _repositoryManager = repositoryManager;
        }

        public List<HistoryWalletModel> GetHistory(int user_id)
        {
            var histories = _repositoryManager.HistoryWallet
                .FindByCondition(x => x.IdUser == user_id, false)
                .Select(x => new HistoryWalletModel
                {
                    IdWallet = x.IdWallet,
                    Id = x.Id,
                    IdUser = x.IdUser,
                    Amount = x.Amount,
                    Status = ((HistoryWalletStatus)x.Status).ToString(),
                    Time = x.Time.Value.ToString("dd/MM/yyyy hh:mm")
                })
                .ToList();

            return histories;
        }

        public decimal UpdateBalance(decimal changes, int user_id)
        {
            var wallet = _repositoryManager.Wallet.FindByCondition(x => x.IdUser == user_id, true).FirstOrDefault();
            if (wallet != null)
            {
                if (wallet.Balance + changes < 0)
                {
                    _repositoryManager.HistoryWallet.Create(new Entities.Models.HistoryWallet
                    {
                        Amount = changes.ToString(),
                        IdUser = user_id,
                        IdWallet = wallet.Id,
                        Status = (int)HistoryWalletStatus.Fail,
                        Time = DateTime.UtcNow.AddHours(7)
                    });

                    _repositoryManager.SaveAsync().Wait();
                    return -1;
                }
                else
                {
                    wallet.Balance += changes;
                    _repositoryManager.SaveAsync().Wait();

                    _repositoryManager.HistoryWallet.Create(new Entities.Models.HistoryWallet
                    {
                        Amount = changes.ToString(),
                        IdUser = user_id,
                        IdWallet = wallet.Id,
                        Status = (int)HistoryWalletStatus.Success,
                        Time = DateTime.UtcNow.AddHours(7)
                    });
                    _repositoryManager.SaveAsync().Wait();

                    return wallet.Balance.Value;
                }
            }
            return -2;
        }
    }
}
