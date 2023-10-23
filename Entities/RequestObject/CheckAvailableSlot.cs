using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.RequestObject
{
    public class CheckAvailableSlot
    {
        public int UserId { get; set; }
        public int NumSlot { get; set; }
        public int PostId { get; set; }
        public string? DateRegis { get; set; }
    }
}
