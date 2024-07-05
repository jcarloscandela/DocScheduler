using AutoMapper;
using DocScheduler.SlotService;
using DocScheduler.Utils;

namespace DocScheduler.Application
{
    public class AppointmentService : IAppointmentService
    {
        private readonly ISlotServiceClient _slotServiceClient;
        private readonly IMapper _mapper;

        public AppointmentService(ISlotServiceClient slotServiceClient, IMapper mapper)
        {
            _slotServiceClient = slotServiceClient;
            _mapper = mapper;
        }

        public async Task<List<AvailableSlotResponse>> GetAvailableSlotsForWeekAsync(AvailableSlotRequest request)
        {
            await Validator.ValidateRequestAsync<AvailableSlotRequest, AvailableSlotRequestValidator>(request);

            DateTime startOfWeek = request.MondayDate.ToDateTime(TimeOnly.MinValue);

            string formattedDate = DateUtils.GetFormattedDate(startOfWeek);

            var weeklyAvailability = await _slotServiceClient.GetWeeklyAvailabilityAsync(formattedDate);

            var availableSlots = ComputeAvailableSlots(weeklyAvailability, startOfWeek);

            return availableSlots;
        }

        public async Task TakeSlotAsync(BookSlotRequest request)
        {
            await Validator.ValidateRequestAsync<BookSlotRequest, BookSlotRequestValidator>(request);

            DateTime monday = DateUtils.GetPreviousMonday(request.Start);

            AvailableSlotRequest availableSlotRequest = new AvailableSlotRequest
            {
                MondayDate = DateOnly.FromDateTime(monday)
            };

            // Get available slots for the week
            var availableSlots = await GetAvailableSlotsForWeekAsync(availableSlotRequest);

            // Check if there are available slots that match the requested time
            bool slotAvailable = availableSlots.Exists(slot =>
                slot.Start == request.Start && slot.End == request.End);

            if (!slotAvailable)
            {
                throw new SlotNotFoundException("No available slot found for the requested time.");
            }

            await _slotServiceClient.TakeSlotAsync(_mapper.Map<TakeSlotRequest>(request));
        }

        private List<AvailableSlotResponse> ComputeAvailableSlots(WeeklyAvailabilityResponse weeklyAvailability, DateTime startOfWeek)
        {
            var availableSlots = new List<AvailableSlotResponse>();

            // Iterate through each day of the week and compute available slots
            foreach (var dayAvailability in GetDayAvailabilities(weeklyAvailability))
            {
                if (dayAvailability.Value == null)
                {
                    continue;
                }

                var slotsInDay = GenerateSlotsForDay(weeklyAvailability.Facility.FacilityId, dayAvailability.Key, dayAvailability.Value.WorkPeriod, dayAvailability.Value.BusySlots, weeklyAvailability.SlotDurationMinutes, startOfWeek);
                availableSlots.AddRange(slotsInDay);
            }

            return availableSlots;
        }

        private IEnumerable<KeyValuePair<DayOfWeek, DayAvailability>> GetDayAvailabilities(WeeklyAvailabilityResponse weeklyAvailability)
        {
            yield return new KeyValuePair<DayOfWeek, DayAvailability>(DayOfWeek.Monday, weeklyAvailability.Monday);
            yield return new KeyValuePair<DayOfWeek, DayAvailability>(DayOfWeek.Tuesday, weeklyAvailability.Tuesday);
            yield return new KeyValuePair<DayOfWeek, DayAvailability>(DayOfWeek.Wednesday, weeklyAvailability.Wednesday);
            yield return new KeyValuePair<DayOfWeek, DayAvailability>(DayOfWeek.Thursday, weeklyAvailability.Thursday);
            yield return new KeyValuePair<DayOfWeek, DayAvailability>(DayOfWeek.Friday, weeklyAvailability.Friday);
            yield return new KeyValuePair<DayOfWeek, DayAvailability>(DayOfWeek.Saturday, weeklyAvailability.Saturday);
            yield return new KeyValuePair<DayOfWeek, DayAvailability>(DayOfWeek.Sunday, weeklyAvailability.Sunday);
        }

        private static List<AvailableSlotResponse> GenerateSlotsForDay(Guid facilityId, DayOfWeek dayOfWeek, WorkPeriod workPeriod, List<BusySlot> busySlots, int slotDurationMinutes, DateTime startOfWeek)
        {
            var availableSlots = new List<AvailableSlotResponse>();

            var startDateTime = startOfWeek.AddDays((int)dayOfWeek - (int)startOfWeek.DayOfWeek).Date;
            var endDateTime = startDateTime.AddHours(workPeriod.EndHour);

            var currentDateTime = startDateTime.AddHours(workPeriod.StartHour);

            while (currentDateTime <= endDateTime)
            {
                if ((currentDateTime.Hour >= workPeriod.StartHour && currentDateTime.Hour < workPeriod.LunchStartHour) ||
                    (currentDateTime.Hour >= workPeriod.LunchEndHour && currentDateTime.Hour < workPeriod.EndHour))
                {
                    var slotEndTime = currentDateTime.AddMinutes(slotDurationMinutes);

                    if (busySlots == null || !busySlots.Exists(slot => currentDateTime >= slot.Start && slotEndTime <= slot.End))
                    {
                        availableSlots.Add(new AvailableSlotResponse
                        {
                            FacilityId = facilityId,
                            Start = currentDateTime,
                            End = slotEndTime
                        });
                    }
                }

                currentDateTime = currentDateTime.AddMinutes(slotDurationMinutes);
            }

            return availableSlots;
        }
    }
}