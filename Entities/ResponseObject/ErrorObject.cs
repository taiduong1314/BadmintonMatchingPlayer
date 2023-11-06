namespace Entities.ResponseObject
{
    public class ErrorObject
    {
        public string? ErrorCode { get; set; }
        public object? Data { get; set; } = null;
    }
}
