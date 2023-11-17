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

        public SlotController(ISlotServices slotServices)
        {
            _slotServices = slotServices;
        }

        [HttpPost]
        [Route("available")]
        public IActionResult CheckAvailableAndCreateSlot(CheckAvailableSlot info)
        {
            try
            {
                List<SlotReturnInfo> slotInfos = _slotServices.GetAvailable(info);
                return Ok(new SuccessObject<List<SlotReturnInfo>> { Data = slotInfos, Message = Message.SuccessMsg });
            }
            catch (FieldAccessException)
            {
                return Ok(new SuccessObject<object> { Message = "Can't subcript to your post" });
            }
        }
    }
}
