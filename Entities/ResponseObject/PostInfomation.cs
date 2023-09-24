using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.ResponseObject
{
    public class PostInfomation
    {
        public int PostId { get; set; }
        public int? UserId { get; set; }
        public string? UserName { get; set; }
        public string? SortDescript { get; set; }
        public string? Time { get; set; }
        public int? AvailableSlot { get; set; }
        public string? PostImgUrl { get; set; }
        public string? UserImgUrl { get; set; }
        public string? Address { get; set; }
    }
}
