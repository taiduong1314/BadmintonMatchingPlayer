using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.ResponseObject
{
    public class JoinedPost
    {
        public string? AreaName { get; set; }
        public decimal? MoneyPaid { get; set; }
        public List<BookedSlotInfo>? BookedInfos { get; set; }
        public int TransacionId { get; set; }
    }

    public class BookedSlotInfo
    {
        public int CreateSlot { get; set; }
        public int BookedSlot { get; set; }
        public List<string> ImageUrls { get; set; }
    }
}
