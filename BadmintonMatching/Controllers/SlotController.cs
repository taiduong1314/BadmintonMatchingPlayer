using Entities.RequestObject;
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

        [HttpGet]
        [Route("available")]
        public IActionResult CheckAvailableAndCreateSlot(CheckAvailableSlot info)
        {
            List<int> slotsId = _slotServices.GetAvailable(info);
            if (slotsId.Count == info.NumSlot)
            {
                return Ok(new { SlotsId = slotsId });
            }
            else
            {
                return BadRequest(new { ErrorMsg = "Not enought slot" });
            }
        }

        [HttpDelete]
        [Route("{post_id}/discard")]
        public IActionResult DiscardSlot(int post_id, DiscartSlotParam info)
        {
            if(info.SlotsId == null || info.SlotsId.Count == 0)
            {
                return Ok(new { ErrorMsg = "Require slot to discard" });
            }
            else
            {
                bool isSuccess = _slotServices.Discard(info.SlotsId, post_id);
                return isSuccess ? Ok(new { Message = "Discard success" }) : Ok(new { ErrorMsg = "Some Id is not match" });
            }
        }
    }
}
