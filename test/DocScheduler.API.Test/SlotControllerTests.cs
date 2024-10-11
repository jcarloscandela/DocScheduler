using DocScheduler.Application;
using DocScheduler.Utils;
using Microsoft.AspNetCore.Mvc;

namespace DocScheduler.API.Test
{
    public class SlotsControllerTests
    {
        private readonly Mock<IAppointmentService> _mockAppointmentService;
        private readonly SlotsController _controller;

        public SlotsControllerTests()
        {
            _mockAppointmentService = new Mock<IAppointmentService>();
            _controller = new SlotsController(_mockAppointmentService.Object);
        }

        [Fact]
        public async Task GetWeekAvailableSlots_Returns_OkObjectResult()
        {
            // Arrange
            var expectedSlots = new List<AvailableSlotResponse>();
            _mockAppointmentService.Setup(x => x.GetAvailableSlotsForWeekAsync(It.IsAny<AvailableSlotRequest>()))
                                   .ReturnsAsync(expectedSlots);

            // Act
            var result = await _controller.GetWeekAvailableSlots(new AvailableSlotRequest());

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var model = Assert.IsAssignableFrom<IEnumerable<AvailableSlotResponse>>(okResult.Value);
            Assert.Equal(expectedSlots, model);
        }

        [Fact]
        public async Task ReserveSlot_Returns_OkObjectResult()
        {
            // Arrange
            var facilityId = Guid.NewGuid();

            // Calculate future Monday
            DateTime futureMonday = DateUtils.GetNextMonday(DateTime.UtcNow);

            var slotDetails = new BookSlotRequest
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

            _mockAppointmentService.Setup(x => x.TakeSlotAsync(It.IsAny<BookSlotRequest>()))
                                   .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.ReserveSlot(slotDetails);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Slot booked successfully.", okResult.Value);
        }
    }
}