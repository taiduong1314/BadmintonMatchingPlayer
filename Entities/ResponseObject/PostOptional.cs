using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.ResponseObject
{
    public class PostOptional
    {
        public string? ContentPost { get; set; }
        public string? ImgUrlPost { get; set; }
        public string? AddressSlot { get; set; }
        public string? Days { get; set; }
        public string? StartTime { get; set; }
        public string? EndTime { get; set; }
        public int? QuantitySlot { get; set; }
        public string? FullName { get; set; }
        public string? UserImgUrl { get; set; }
        public string? HighlightUrl { get; set; }
        public string? Price { get; set; }
    }
}
