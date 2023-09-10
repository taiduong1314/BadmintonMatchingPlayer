namespace Entities.Models
{
    public partial class Subscription
    {
        public int UserId { get; set; }
        public int UserSubId { get; set; }

        public virtual User User { get; set; } = null!;
        public virtual User UserSub { get; set; } = null!;
    }
}
