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
            List<int> slotsId = _slotServices.GetAvailable(info);
            if (slotsId.Count == info.NumSlot)
            {
                return Ok(new SuccessObject { Data = new { SlotsId = slotsId }, Message = Message.SuccessMsg });
            }
            else
            {
                return BadRequest(new ErrorObject{ ErrorCode = "Not enought slot" });
            }
        }

        [HttpDelete]
        [Route("{post_id}/discard")]
        public IActionResult DiscardSlot(int post_id, DiscartSlotParam info)
        {
            if(info.SlotsId == null || info.SlotsId.Count == 0)
            {
                return Ok(new ErrorObject { ErrorCode = "Require slot to discard" });
            }
            else
            {
                bool isSuccess = _slotServices.Discard(info.SlotsId, post_id);
                return isSuccess ? Ok(new SuccessObject { Message = "Discard success", Data = null }) : Ok(new ErrorObject { ErrorCode = "Some Id is not match" });
            }
        }
    }
}
