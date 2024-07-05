namespace DocScheduler.Application
{
    public class BookSlotRequest
    {
        public required Guid FacilityId { get; set; }
        public required DateTime Start { get; set; }
        public required DateTime End { get; set; }
        public required string Comments { get; set; }
        public required string Name { get; set; }
        public required string SecondName { get; set; }
        public required string Email { get; set; }
        public required string Phone { get; set; }
    }
}
