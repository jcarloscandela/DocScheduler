namespace DocScheduler.Application
{
    public record AvailableSlotRequest
    {
        public DateOnly MondayDate { get; set; }
    }
}
