namespace DocScheduler.SlotService
{
    public record TakeSlotRequest
    {
        public required Guid FacilityId { get; set; }
        public required string Start { get; set; }
        public required string End { get; set; }
        public required string Comments { get; set; }
        public required Patient Patient { get; set; }
    }

    public class Patient
    {
        public required string Name { get; set; }
        public required string SecondName { get; set; }
        public required string Email { get; set; }
        public required string Phone { get; set; }
    }
}
