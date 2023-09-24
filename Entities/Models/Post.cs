namespace Entities.Models
{
    public partial class Post
    {
        public Post()
        {
            Slots = new HashSet<Slot>();
        }

        public int Id { get; set; }
        public int? IdType { get; set; }
        public int? IdUserTo { get; set; }
        public string? Title { get; set; }
        public string? AddressSlot { get; set; }
        public DateTime? TimeStart { get; set; }
        public decimal? PriceSlot { get; set; }
        public int? QuantitySlot { get; set; }
        public string? LevelSlot { get; set; }
        public string? CategorySlot { get; set; }
        public string? ContentPost { get; set; }
        public string? ImgUrl { get; set; }
        public bool? Status { get; set; }
        public DateTime? TimeEnd { get; set; }

        public virtual TypePost? IdTypeNavigation { get; set; }
        public virtual User? IdUserToNavigation { get; set; }
        public virtual ICollection<Slot> Slots { get; set; }
    }
}
