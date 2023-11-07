namespace Entities.ResponseObject
{
    public class TransactionDetail
    {
        public int Id { get; set; }
        public int SlotCount { get; set; }
        public List<SlotBuy>? Slots { get; set; }
        public string? BuyerName { get; set; }
        public string? PayTime { get; set; }
        public string? Total { get; set; }
    }

}
