using Entities.RequestObject;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;

namespace BadmintonMatching.Controllers
{
    [Route("api/wallet")]
    [ApiController]
    public class WalletController : ControllerBase
    {
        private readonly IWalletServices _walletServices;

        public WalletController(IWalletServices walletServices)
        {
            _walletServices = walletServices;
        }

        [HttpPut]
        [Route("{user_id}")]
        public IActionResult UpdateWallet(int user_id, UpdateWallet updateWallet)
        {
            var newBalance = _walletServices.UpdateBalance(updateWallet.Changes, user_id);
            if(newBalance == -1)
            {
                return Ok(new { Message = "Balance not enough to charge" });
            }
            else if (newBalance == -2) 
            {
                return Ok(new { Message = $"Wallet of user {user_id} isn't found" });
            }
            else
            {
                return Ok(new {NewBalance = newBalance });
            }
        }
    }
}
