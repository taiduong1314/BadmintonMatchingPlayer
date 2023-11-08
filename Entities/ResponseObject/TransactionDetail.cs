﻿namespace Entities.ResponseObject
{
    public class TransactionDetail
    {
        public int Id { get; set; }
        public int SlotCount { get; set; }
        public List<SlotBuy>? Slots { get; set; }
        public string? BuyerName { get; set; }
        public string? PayTime { get; set; }
        public string? Total { get; set; }
        public PostInTransaction? Post { get; set; }
    }

    public class PostInTransaction
    {
        public int? Id { get; set; }
        public string? Title { get; set; }
        public string? TitleImage { get; set; }
        public string? ImageUrls { get; set; }
        public string? PricePerSlot { get; set; }
        public string? Address { get; set; }
        public string? StartTime { get; set; }
        public string? EndTime { get; set; }
    }
}
