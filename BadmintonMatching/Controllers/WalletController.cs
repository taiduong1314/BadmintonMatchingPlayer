using BadmintonMatching.Payment;
using Entities.RequestObject;
using Entities.ResponseObject;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.Implements;
using Services.Interfaces;

namespace BadmintonMatching.Controllers
{
    [Route("api/wallet")]
    [ApiController]
    public class WalletController : ControllerBase
    {
        private readonly IWalletServices _walletServices;
        private readonly IVNPayService _vnPayService;

        public WalletController(IWalletServices walletServices, IVNPayService vnPayService)
        {
            _walletServices = walletServices;
            _vnPayService = vnPayService;
        }

        [HttpPut]
        [Route("{user_id}")]

        public IActionResult UpdateWallet(int user_id, UpdateWallet updateWallet)
        {
            var newBalance = _walletServices.UpdateBalance(updateWallet.Changes, user_id);
            if(newBalance == -1)
            {
                return Ok(new SuccessObject<object> { Message = "Balance not enough to charge" });
            }
            else if (newBalance == -2) 
            {
                return Ok(new SuccessObject<object> { Message = $"Wallet of user {user_id} isn't found" });
            }
            else
            {
                return Ok(new SuccessObject<object> { Data = new { NewBalance = newBalance }, Message = Message.SuccessMsg });
            }
        }
        [HttpPost]
        [Route("create-vnpay")]
        public async Task<IActionResult> CreateVnPay(UpdateWallet wallet)
        {
            var responseUriVnPay = _vnPayService.CreatePayment(new PaymentInfoModel()
            {
                TotalAmount = (double)wallet.Changes,
                PaymentCode = Guid.NewGuid().ToString()
            }, HttpContext);

            if (string.IsNullOrEmpty(responseUriVnPay.Uri))
            {
                return new BadRequestObjectResult(new
                {
                    Message = "Can't create payment url at this time"
                });
            }

            return Ok(new SuccessObject<object>
            {
                Message = "Create url successfully!",
                Data = responseUriVnPay
            });
        }
    }
}
