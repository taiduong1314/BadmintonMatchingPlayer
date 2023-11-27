using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models
{
    public class Notification
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Content { get; set; }
        public DateTime NotiDate { get; set; }
        public bool IsRead { get; set; }
        public int UserId { get; set; }

        public virtual User? User { get; set; }
    }
}
