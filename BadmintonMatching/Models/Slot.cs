using System;
using System.Collections.Generic;

namespace BadmintonMatching.Models
{
    public partial class Slot
    {
        public Slot()
        {
            Transactions = new HashSet<Transaction>();
        }

        public int Id { get; set; }
        public int? SlotNumber { get; set; }
        public bool? Status { get; set; }
        public decimal? Price { get; set; }
        public string? ContentSlot { get; set; }
        public int? IdPost { get; set; }
        public int? IdUser { get; set; }

        public virtual Post? IdPostNavigation { get; set; }
        public virtual ICollection<Transaction> Transactions { get; set; }
    }
}
