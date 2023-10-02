using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.RequestObject
{
    public class NewPostInfo
    {
        public int idType {  get; set; }
        public string? Title { get; set; }
        public string? Address { get; set; }
        public string? Day { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public string? StartTime { get; set; }
        public string? EndTime { get; set; }
        public string? Price { get; set; }
        public int AvailableSlot { get; set; }
        public string? Description { get; set; }
    }
}
