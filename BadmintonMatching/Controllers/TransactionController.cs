using Entities.Models;
using Entities.RequestObject;
using Entities.ResponseObject;
using Hangfire;
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
            if (_transactionRepository.IsFromTwoPost(info.IdSlot))
            {
                return Ok(new SuccessObject<object> { Message = "Can't create from slots of more than 1 post" });
            }
            var tranId = await _transactionRepository.CreateForBuySlot(info);
            if (tranId.Id == 0)
            {
                return Ok(new SuccessObject<object>{ Message = "Create not success" });
            }
            else
            {
                return Ok(new SuccessObject<object> { Data = new { TranSactionId = tranId }, Message = Message.SuccessMsg });
            }
        }

       

        [HttpPut]
        [Route("{tran_id}/status_info/{status_info}")]
        [ProducesResponseType(typeof(SuccessObject<List<Reports>>), 200)]
        public async Task<IActionResult> SuccessPayment(int tran_id, int status_info)
        {
            if (_transactionRepository.ExistTran(tran_id))
            {
                await _transactionRepository.UpdateStatus(tran_id, (TransactionStatus)status_info);
                //if(status_info == (int)TransactionStatus.PaymentSuccess)
                //{
                //    var jobId = BackgroundJob.Schedule(() => AutoTransferMoney(tran_id), )
                //}
                return Ok(new SuccessObject<object> { Message = "Cập nhật thành công", Data = true });
            }
            else
            {
                return Ok(new SuccessObject<object> { Message = "Invalid transaction" });
            }
        }

        [HttpGet]
        [Route("user/{user_id}")]
        [ProducesResponseType(typeof(SuccessObject<List<TransactionInfo>>), 200)]
        public async Task<IActionResult> GetTransactionOfUser(int user_id)
        {
            if (!_userServices.ExistUserId(user_id))
            {
                return Ok(new SuccessObject<List<TransactionInfo?>> { Message = "Invalid User" });
            }

            var data = await _transactionRepository.GetOfUser(user_id);
            return Ok(new SuccessObject<List<TransactionInfo>> { Data= data, Message = Message.SuccessMsg });
        }

        [HttpGet]
        [Route("{transaction_id}/detail")]
        [ProducesResponseType(typeof(SuccessObject<TransactionDetail>), 200)]
        public async Task<IActionResult> GetTransactionDetail(int transaction_id)
        {
            var data = await _transactionRepository.GetDetail(transaction_id);
            return Ok(new SuccessObject<TransactionDetail> { Data= data, Message = Message.SuccessMsg });
        }

        [HttpDelete]
        [Route("{transaction_id}/discard")]
        public async Task<IActionResult> DiscardTransaction(int transaction_id)
        {
            var transaction = await _transactionRepository.GetTransaction(transaction_id);
            if(transaction.Id > 0)
            {
                if(transaction.Status != (int)TransactionStatus.ReportResolved && transaction.Status != (int)TransactionStatus.Played)
                {
                    await _transactionRepository.DeleteSlot(transaction_id);
                    await _transactionRepository.DeleteTran(transaction_id);
                    return Ok(new SuccessObject<object> { Message = Message.SuccessMsg, Data = true });
                }
                else
                {
                    return Ok(new SuccessObject<object> { Message = "Completed transaction not allow to delete" });
                }
            }
            else
            {
                return Ok(new SuccessObject<object> { Message = "Invalid transaction id" });
            }
        }
    }
}
