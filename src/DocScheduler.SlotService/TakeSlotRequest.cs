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

    public record Patient
    {
        public string Name { get; set; }
        public string SecondName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
    }
}