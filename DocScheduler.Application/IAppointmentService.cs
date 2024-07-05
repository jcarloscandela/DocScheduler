namespace DocScheduler.Application
{
    public interface IAppointmentService
    {
        Task<List<AvailableSlotResponse>> GetAvailableSlotsForWeekAsync(AvailableSlotRequest request);

        Task TakeSlotAsync(BookSlotRequest request);
    }
}