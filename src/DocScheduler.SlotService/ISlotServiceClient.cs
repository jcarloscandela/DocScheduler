namespace DocScheduler.SlotService
{
    public interface ISlotServiceClient
    {
        Task<WeeklyAvailabilityResponse> GetWeeklyAvailabilityAsync(string monday);

        Task<TakeSlotResponse> TakeSlotAsync(TakeSlotRequest request);
    }
}