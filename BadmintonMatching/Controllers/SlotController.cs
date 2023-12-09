using Entities.RequestObject;
using Entities.ResponseObject;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;

namespace BadmintonMatching.Controllers
{
    [Route("api/slots")]
    [ApiController]
    public class SlotController : ControllerBase
    {
        private readonly ISlotServices _slotServices;
        private readonly ITransactionServices _transactionRepository;
        private readonly IWalletServices _walletServices;
        private readonly IChatServices _chatServices;

        public SlotController(ISlotServices slotServices,
            ITransactionServices transactionRepository,
            IWalletServices walletServices,
            IChatServices chatServices)
        {
            _slotServices = slotServices;
            _transactionRepository = transactionRepository;
            _walletServices = walletServices;
            _chatServices = chatServices;
        }

        [HttpPost]
        [Route("booking")]
        public async Task<IActionResult> CheckAvailableAndCreateSlot(CheckAvailableSlot info)
        {
            try
            {
                List<SlotReturnInfo> slotInfos = _slotServices.GetAvailable(info);

                var lsSlot = new List<int>();
                var isDeleteSlot = false;
                foreach (var item in slotInfos)
                {
                    if (item.SlotIds != null)
                    {
                        foreach (var slotId in item.SlotIds)
                        {
                            lsSlot.Add(slotId);
                        }
                    }
                    else
                    {
                        isDeleteSlot = true;
                    }
                }

                if (isDeleteSlot)
                {
                    _slotServices.Delete(lsSlot);
                    return Ok(new SuccessObject<SlotIncludeTransaction>
                    {
                        Message = "Invalid num of available slot input"
                    });
                }

                var createInfo = new TransactionCreateInfo
                {
                    IdSlot = lsSlot,
                    IdUser = info.UserId
                };
                var tran = await _transactionRepository.CreateForBuySlot(createInfo);

                if (tran == null)
                {
                    _slotServices.Delete(lsSlot);
                    return Ok(new SuccessObject<object> { Message = "Slot not found" });
                }

                var newBalance = _walletServices.UpdateBalance(-tran.MoneyTrans.Value, createInfo.IdUser.Value, true);
                if (newBalance == -1 || newBalance == -2)
                {
                    await _transactionRepository.DeleteSlot(tran.Id);
                    await _transactionRepository.DeleteTran(tran.Id);
                    if (newBalance == -1)
                    {
                        return Ok(new SuccessObject<object> { Message = "Balance not enough to charge" });
                    }
                    else if (newBalance == -2)
                    {
                        return Ok(new SuccessObject<object> { Message = $"Wallet of user {createInfo.IdUser.Value} isn't found" });
                    }
                }
                else
                {
                    await _transactionRepository.UpdateStatus(tran.Id, Entities.Models.TransactionStatus.PaymentSuccess);
                }

                var chatRoom = await _chatServices.GetChatRoom(tran.Id);

                return Ok(new SuccessObject<SlotIncludeTransaction>
                {
                    Data = new SlotIncludeTransaction
                    {
                        TransactionId = tran.Id,
                        ChatInfos = chatRoom
                    },
                    Message = Message.SuccessMsg
                });
            }
            catch (FieldAccessException)
            {
                return Ok(new SuccessObject<object> { Message = "Can't subcript to your post" });
            }
        }
    }
}
