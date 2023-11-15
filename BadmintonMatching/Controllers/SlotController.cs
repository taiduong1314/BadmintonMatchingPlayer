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
                return Ok(new SuccessObject<object> { Data = new { SlotsId = slotsId }, Message = Message.SuccessMsg });
            }
            else if (slotsId[0] == 0)
            {
                return Ok(new SuccessObject<object> { Message = "Can't join your owned post" });
            }
            else
            {
                return BadRequest(new SuccessObject<object> { Message = "Not enought slot" });
            }
        }
    }
}
