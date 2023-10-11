using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.ResponseObject
{
    public class PostDetail
    {
        public string? AddressSlot { get; set; }
        public decimal? PriceSlot { get; set; }
        public int? QuantitySlot { get; set; }
        public int? AvailableSlot { get; set; }
        public string? LevelSlot { get; set; }
        public string? CategorySlot { get; set; }
        public string? ContentPost { get; set; }
        public string? ImgUrl { get; set; }
        public string? Days { get; set; }
        public string? StartTime { get; set; }
        public string? EndTime { get; set; }
        public string? FullName { get; set; }
        public int? TotalRate { get; set; }
        public string? ImgUrlUser { get; set; }
        public string? SortProfile { get; set; }
        public int UserId { get; set; }
    }
}
