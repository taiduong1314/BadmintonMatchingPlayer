using Entities.RequestObject;
using Entities.ResponseObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface ISlotServices
    {
        List<SlotReturnInfo> GetAvailable(CheckAvailableSlot info);
    }
}
