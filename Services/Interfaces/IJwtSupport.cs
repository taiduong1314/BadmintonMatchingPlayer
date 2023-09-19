namespace Services.Interfaces
{
    public interface IJwtSupport
    {
        string CreateToken(int role, int accountId);
    }
}
