﻿namespace Entities.Models
{
    public partial class Transaction
    {
        public Transaction()
        {
            HistoryTransactions = new HashSet<HistoryTransaction>();
            Reports = new HashSet<Report>();
        }

        public int Id { get; set; }
        public int? IdUser { get; set; }
        public DateTime? TimeTrans { get; set; }
        public string? MethodTrans { get; set; }
        public string? TypeTrans { get; set; }
        public decimal? MoneyTrans { get; set; }
        public int? IdSlot { get; set; }
        public bool? Status { get; set; }
        public DateTime? DeadLine { get; set; }

        public virtual Slot? IdSlotNavigation { get; set; }
        public virtual User? IdUserNavigation { get; set; }
        public virtual ICollection<HistoryTransaction> HistoryTransactions { get; set; }
        public virtual ICollection<Report> Reports { get; set; }
    }
}
