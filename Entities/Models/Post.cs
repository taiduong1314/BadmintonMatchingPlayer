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
        public bool IsDeleted  { get; set; }
        public int? IdUserTo { get; set; }
        public string? Title { get; set; }
        public string? TotalViewer { get; set; }
        public string? AddressSlot { get; set; }
        public string? LevelSlot { get; set; }
        public string? CategorySlot { get; set; }
        public string? ContentPost { get; set; }
        public string? ImgUrl { get; set; }
        public bool? Status { get; set; }
        public string? SlotsInfo { get; set; }
        public virtual TypePost? IdTypeNavigation { get; set; }
        public virtual User? IdUserToNavigation { get; set; }
        public virtual ICollection<Slot> Slots { get; set; }
        public DateTime SavedDate { get; set; }
        public string? ImageUrls { get; set; }
    }
}
