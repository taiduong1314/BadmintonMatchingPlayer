using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.RequestObject
{
    public class NewPostInfo
    {
        public string? LevelSlot { get; set; }
        public string? CategorySlot { get; set; }
        public string? Title { get; set; }
        public string? Address { get; set; }
        public string? Day { get; set; }
        public string? Month { get; set; }
        public string? Year { get; set; }
        public string? StartTime { get; set; }
        public string? EndTime { get; set; }
        public string? Price { get; set; }
        public int AvailableSlot { get; set; }
        public string? Description { get; set; }
        public string? HighlightUrl { get; set; }
        public List<string>? ImgUrls { get; set; }
    }
}
