using Entities.Models;
using Entities.RequestObject;
using Entities.ResponseObject;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using Services.Interfaces;

namespace BadmintonMatching.Controllers
{
    [Route("api/transactions")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionServices _transactionRepository;

        public TransactionController(ITransactionServices transactionRepository)
        {
            _transactionRepository = transactionRepository;
        }

        [HttpPost]
        [Route("buy_slot")]
        public async Task<IActionResult> CreateTransactionBuyingSlot(TransactionCreateInfo info)
        {
            var tranId = await _transactionRepository.CreateForBuySlot(info);
            if (tranId == 0)
            {
                return Ok(new ErrorObject{ ErrorCode = "Create not success" });
            }
            else
            {
                return Ok(new SuccessObject { Data = new { TranSactionId = tranId }, Message = Message.SuccessMsg });
            }
        }

        [HttpPut]
        [Route("{tran_id}/status_info/{status_info}")]
        public async Task<IActionResult> SuccessPayment(int tran_id, int status_info)
        {
            if (_transactionRepository.ExistTran(tran_id))
            {
                await _transactionRepository.UpdateStatus(tran_id, (TransactionStatus)status_info);
                return Ok(new SuccessObject { Message = "Update success" });
            }
            else
            {
                return Ok(new ErrorObject{ ErrorCode = "Invalid transaction" });
            }
        }

    }
}
