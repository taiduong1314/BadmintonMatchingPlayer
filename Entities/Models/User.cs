namespace Entities.Models
{
    public partial class User
    {
        public User()
        {
            Posts = new HashSet<Post>();
            Transactions = new HashSet<Transaction>();
            UserRatings = new HashSet<UserRating>();
            Wallets = new HashSet<Wallet>();
        }

        public int Id { get; set; }
        public string UserName { get; set; } = null!;
        public string? UserPassword { get; set; }
        public string? FullName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? UserAddress { get; set; }
        public bool? IsActive { get; set; }
        public string? ImgUrl { get; set; }
        public int? TotalRate { get; set; }
        public double? Rate { get; set; }
        public int? UserRole { get; set; }
        public string? DeviceToken { get; set; }

        public virtual Role? UserRoleNavigation { get; set; }
        public virtual ICollection<Post> Posts { get; set; }
        public virtual ICollection<Transaction> Transactions { get; set; }
        public virtual ICollection<UserRating> UserRatings { get; set; }
        public virtual ICollection<Wallet> Wallets { get; set; }
    }
}
