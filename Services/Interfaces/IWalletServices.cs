namespace Services.Interfaces
{
    public interface IWalletServices
    {
        decimal UpdateBalance(decimal changes, int user_id);
    }
}
