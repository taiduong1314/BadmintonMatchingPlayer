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
        private readonly IUserServices _userServices;

        public TransactionController(ITransactionServices transactionRepository, IUserServices userServices)
        {
            _transactionRepository = transactionRepository;
            _userServices = userServices;
        }

        [HttpPost]
        [Route("buy_slot")]
        public async Task<IActionResult> CreateTransactionBuyingSlot(TransactionCreateInfo info)
        {
            var tranId = await _transactionRepository.CreateForBuySlot(info);
            if (tranId == 0)
            {
                return Ok(new SuccessObject{ Message = "Create not success" });
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
                return Ok(new SuccessObject { Message = "Update success", Data = true });
            }
            else
            {
                return Ok(new SuccessObject{ Message = "Invalid transaction" });
            }
        }

        [HttpGet]
        [Route("user/{user_id}")]
        public async Task<IActionResult> GetTransactionOfUser(int user_id)
        {
            if (!_userServices.ExistUserId(user_id))
            {
                return Ok(new SuccessObject { Message = "Invalid User" });
            }

            var data = await _transactionRepository.GetOfUser(user_id);
            return Ok(new SuccessObject { Data= data, Message = Message.SuccessMsg });
        }

        [HttpGet]
        [Route("{transaction_id}/detail")]
        public async Task<IActionResult> GetTransactionDetail(int transaction_id)
        {
            var data = await _transactionRepository.GetDetail(transaction_id);
            return Ok(new SuccessObject { Data= data, Message = Message.SuccessMsg });
        }

    }
}
