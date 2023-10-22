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
        [Route("{post_id}\\avalable\\{num_slot}")]
        public IActionResult CheckAvailableAndCreateSlot(int post_id, int num_slot)
        {
            List<int> slotsId = _slotServices.GetAvailable(post_id, num_slot);
            if (slotsId.Count == num_slot)
            {
                return Ok(new { SlotsId = slotsId });
            }
            else
            {
                return BadRequest(new { ErrorMsg = "Not enought slot" });
            }
        }
    }
}
