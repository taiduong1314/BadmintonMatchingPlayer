namespace Entities.Models
{
    public enum HistoryWalletStatus
    {
        Success = 1,
        Fail = 0
    }

    public partial class HistoryWallet
    {
        public int Id { get; set; }
        public int? IdWallet { get; set; }
        public int? IdUser { get; set; }
        public string? Amount { get; set; }
        public int? Status { get; set; }
        public DateTime? Time { get; set; }
        public int? Type { get; set; }

        public virtual Wallet? IdWalletNavigation { get; set; }
    }
}
