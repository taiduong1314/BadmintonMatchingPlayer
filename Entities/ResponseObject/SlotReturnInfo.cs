using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.ResponseObject
{
    public class SlotReturnInfo
    {
        public string? Date { get; set; }
        public List<int>? SlotIds { get; set; }
        public string? Message { get; set; }
    }
}
