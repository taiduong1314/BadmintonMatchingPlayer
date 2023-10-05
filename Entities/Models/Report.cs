namespace Entities.Models
{
    public partial class Report
    {
        public int Id { get; set; }
        public int? IdHistory { get; set; }
        public int? IdUserFrom { get; set; }
        public int? IdUserTo { get; set; }
        public int? IdRoom { get; set; }
        public DateTime? TimeReport { get; set; }
        public bool? Status { get; set; }
        public string? reportContent {  get; set; }

        public virtual HistoryTransaction? IdHistoryNavigation { get; set; }
    }
}
