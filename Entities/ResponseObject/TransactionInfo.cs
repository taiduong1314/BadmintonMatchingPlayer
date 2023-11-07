namespace Entities.ResponseObject
{
    public class TransactionInfo
    {
        public int Id { get; set; }
        public string? MoneyPaied { get; set; }
        public List<SlotBuy>? Slots { get; set; }
    }

    public class SlotBuy
    {
        public int Id { get; set; }
        public string? PlayDate { get; set; }
    }
}
