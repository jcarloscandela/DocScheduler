namespace DocScheduler.SlotService
{
    public record TakeSlotResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}