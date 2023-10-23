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

        public decimal UpdateBalance(decimal changes, int user_id)
        {
            var wallet = _repositoryManager.Wallet.FindByCondition(x => x.IdUser == user_id, true).FirstOrDefault();
            if (wallet != null)
            {
                if (wallet.Balance + changes < 0)
                {
                    return -1;
                }
                else
                {
                    wallet.Balance += changes;
                    _repositoryManager.SaveAsync().Wait();
                    return wallet.Balance.Value;
                }
            }
            return -2;
        }
    }
}
