using AutoMapper;
using DocScheduler.SlotService;
using DocScheduler.Utils;
using Moq;

namespace DocScheduler.Application.Test
{
    public class AppointmentServiceTests
    {
        private readonly Mock<ISlotServiceClient> _slotServiceClientMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly AppointmentService _appointmentService;

        public AppointmentServiceTests()
        {
            _slotServiceClientMock = new Mock<ISlotServiceClient>();
            _mapperMock = new Mock<IMapper>();
            _appointmentService = new AppointmentService(_slotServiceClientMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task GetAvailableSlotsForWeekAsync_InvalidDateFormat_ThrowsArgumentException()
        {
            var invalidRequest = new AvailableSlotRequest { MondayDate = new DateOnly(2023, 6, 13) };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidModelException>(() => _appointmentService.GetAvailableSlotsForWeekAsync(invalidRequest));
            Assert.Equal("One or more validation errors occurred.", exception.Message);
            Assert.Contains("The date must be a Monday.", exception.ToString());
        }

        [Fact]
        public async Task GetAvailableSlotsForWeekAsync_ValidDateFormat_ReturnsExpectedAvailableSlots()
        {
            // Arrange
            var validRequest = new AvailableSlotRequest { MondayDate = new DateOnly(2023, 6, 12) };

            var facilityId = Guid.NewGuid();
            var weeklyAvailability = new WeeklyAvailabilityResponse
            {
                Facility = new Facility { FacilityId = facilityId },
                Monday = new DayAvailability
                {
                    WorkPeriod = new WorkPeriod { StartHour = 9, EndHour = 12, LunchStartHour = 10, LunchEndHour = 11 },
                    BusySlots = new List<BusySlot> { new BusySlot {
                        Start = new DateTime(2023, 6, 12,11, 0 ,0, DateTimeKind.Utc),
                        End = new DateTime(2023, 6, 12,11,30,0, DateTimeKind.Utc) } }
                },
                SlotDurationMinutes = 30
            };

            List<AvailableSlotResponse> expectedSlots = new List<AvailableSlotResponse>
            {
                new() {
                    FacilityId = facilityId,
                    Start = new DateTime(2023,6,12, 9,0,0, DateTimeKind.Utc),
                    End = new DateTime(2023,6,12,9,30,0, DateTimeKind.Utc)
                },
                new() {
                    FacilityId = facilityId,
                    Start = new DateTime(2023,6,12,9,30,0, DateTimeKind.Utc),
                    End = new DateTime(2023,6,12,10,0,0, DateTimeKind.Utc)
                },
                 new() {
                    FacilityId = facilityId,
                    Start = new DateTime(2023,6,12,11,30,0, DateTimeKind.Utc),
                    End = new DateTime(2023,6,12,12,0,0, DateTimeKind.Utc)
                }
            };

            _slotServiceClientMock.Setup(client => client.GetWeeklyAvailabilityAsync("20230612")).ReturnsAsync(weeklyAvailability);

            // Act
            var availableSlots = await _appointmentService.GetAvailableSlotsForWeekAsync(validRequest);

            // Assert
            Assert.NotNull(availableSlots);
            Assert.Equal(expectedSlots.Count, availableSlots.Count);
            Assert.All(expectedSlots, expectedSlot =>
                Assert.Contains(availableSlots, actualSlot =>
                    actualSlot.FacilityId == expectedSlot.FacilityId &&
                    actualSlot.Start == expectedSlot.Start &&
                    actualSlot.End == expectedSlot.End));
        }

        [Fact]
        public async Task TakeSlotAsync_ValidRequest_ReturnsTakeSlotResponse()
        {
            // Arrange
            var facilityId = Guid.NewGuid();

            // Calculate future Monday
            DateTime futureMonday = DateUtils.GetNextMonday(DateTime.UtcNow);

            var weeklyAvailability = new WeeklyAvailabilityResponse
            {
                Facility = new Facility { FacilityId = facilityId },
                Monday = new DayAvailability
                {
                    WorkPeriod = new WorkPeriod { StartHour = 9, EndHour = 12, LunchStartHour = 10, LunchEndHour = 11 },
                    BusySlots = new List<BusySlot> { new BusySlot {
                        Start = new DateTime(futureMonday.Year, futureMonday.Month, futureMonday.Day,11, 0 ,0, DateTimeKind.Utc),
                        End = new DateTime(futureMonday.Year, futureMonday.Month, futureMonday.Day,11,30,0, DateTimeKind.Utc) } }
                },
                SlotDurationMinutes = 30
            };

            var request = new BookSlotRequest
            {
                FacilityId = facilityId,
                Start = new DateTime(futureMonday.Year, futureMonday.Month, futureMonday.Day, 9, 0, 0, DateTimeKind.Utc),
                End = new DateTime(futureMonday.Year, futureMonday.Month, futureMonday.Day, 9, 30, 0, DateTimeKind.Utc),
                Comments = "Patient requires wheelchair access",
                Name = "John",
                SecondName = "Doe",
                Email = "john.doe@example.com",
                Phone = "1234567890"
            };

            var mappedRequest = new TakeSlotRequest
            {
                FacilityId = facilityId,
                Start = request.Start.ToString("yyyy-MM-ddTHH:mm:ss"),
                End = request.End.ToString("yyyy-MM-ddTHH:mm:ss"),
                Comments = "Patient requires wheelchair access",
                Patient = new Patient
                {
                    Name = "John",
                    SecondName = "Doe",
                    Email = "john.doe@example.com",
                    Phone = "1234567890"
                }
            };

            var response = new TakeSlotResponse { Success = true };

            _mapperMock.Setup(mapper => mapper.Map<TakeSlotRequest>(request)).Returns(mappedRequest);
            _slotServiceClientMock.Setup(client => client.GetWeeklyAvailabilityAsync(DateUtils.GetFormattedDate(futureMonday))).ReturnsAsync(weeklyAvailability);
            _slotServiceClientMock.Setup(client => client.TakeSlotAsync(mappedRequest)).ReturnsAsync(response);

            // Act
            await _appointmentService.TakeSlotAsync(request);

            // Assert
            _mapperMock.Verify(mapper => mapper.Map<TakeSlotRequest>(request), Times.Once());
            _slotServiceClientMock.Verify(client => client.TakeSlotAsync(mappedRequest), Times.Once);
        }

        [Fact]
        public async Task TakeSlotAsync_NoAvailableSlot_ThrowsSlotNotFoundException()
        {
            // Arrange
            var facilityId = Guid.NewGuid();

            // Calculate future Monday
            DateTime futureMonday = DateUtils.GetNextMonday(DateTime.UtcNow);

            var weeklyAvailability = new WeeklyAvailabilityResponse
            {
                Facility = new Facility { FacilityId = facilityId },
                Monday = new DayAvailability
                {
                    WorkPeriod = new WorkPeriod { StartHour = 9, EndHour = 12, LunchStartHour = 10, LunchEndHour = 11 },
                    BusySlots = new List<BusySlot> { new BusySlot {
                Start = new DateTime(futureMonday.Year, futureMonday.Month, futureMonday.Day, 11, 0 ,0, DateTimeKind.Utc),
                End = new DateTime(futureMonday.Year, futureMonday.Month, futureMonday.Day, 11, 30, 0, DateTimeKind.Utc) } }
                },
                SlotDurationMinutes = 30
            };

            var request = new BookSlotRequest
            {
                FacilityId = facilityId,
                Start = new DateTime(futureMonday.Year, futureMonday.Month, futureMonday.Day, 9, 0, 0, DateTimeKind.Utc),
                End = new DateTime(futureMonday.Year, futureMonday.Month, futureMonday.Day, 9, 30, 0, DateTimeKind.Utc),
                Comments = "Patient requires wheelchair access",
                Name = "John",
                SecondName = "Doe",
                Email = "john.doe@example.com",
                Phone = "1234567890"
            };

            // Mock mapper and slot service client
            var mappedRequest = new TakeSlotRequest
            {
                FacilityId = facilityId,
                Start = request.Start.ToString("yyyy-MM-ddTHH:mm:ss"),
                End = request.End.ToString("yyyy-MM-ddTHH:mm:ss"),
                Comments = "Patient requires wheelchair access",
                Patient = new Patient
                {
                    Name = "John",
                    SecondName = "Doe",
                    Email = "john.doe@example.com",
                    Phone = "1234567890"
                }
            };

            _mapperMock.Setup(mapper => mapper.Map<TakeSlotRequest>(request)).Returns(mappedRequest);
            _slotServiceClientMock.Setup(client => client.GetWeeklyAvailabilityAsync(DateUtils.GetFormattedDate(futureMonday))).ReturnsAsync(weeklyAvailability);

            // Ensure that TakeSlotAsync throws SlotNotFoundException
            _slotServiceClientMock.Setup(client => client.TakeSlotAsync(mappedRequest)).ThrowsAsync(new SlotNotFoundException("No available slot found for the requested time."));

            // Act and Assert
            await Assert.ThrowsAsync<SlotNotFoundException>(async () => await _appointmentService.TakeSlotAsync(request));

            // Verify that the mappings and client calls were made as expected
            _mapperMock.Verify(mapper => mapper.Map<TakeSlotRequest>(request), Times.Once());
            _slotServiceClientMock.Verify(client => client.TakeSlotAsync(mappedRequest), Times.Once);
        }
    }
}