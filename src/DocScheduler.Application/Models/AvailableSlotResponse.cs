namespace DocScheduler.Application
{
    public record AvailableSlotResponse
    {
        public Guid FacilityId { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
    }
}
