namespace Entities.ResponseObject
{
    public class UserInformation
    {
        public string? UserName { get; set; }
        public string? Avatar { get; set; }
        public string? Token { get; set; }
        public bool IsNewUser { get; set; }
        public string? PlayingArea { get; set; }
        public int PlayingLevel { get; set; }
        public string? PlayingWay { get; set; }
        public int Id { get; set; }
    }
}
