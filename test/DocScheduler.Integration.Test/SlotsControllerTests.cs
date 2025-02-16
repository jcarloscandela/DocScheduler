using System.Net;
using System.Net.Http.Json;

namespace DocScheduler.Integration.Test
{
    public class SlotsControllerIntegrationTests
    {
        private readonly HttpClient _client;

        public SlotsControllerIntegrationTests()
        {
            _client = new HttpClient
            {
                BaseAddress = new Uri("http://localhost:5038")
            };
        }

        [Fact]
        public async Task GetAndReserveSlot_ShouldBookSlotAndVerifyAvailability()
        {
            // Arrange
            var mondayDate = "2025-02-17"; // Example Monday date

            // Step 1: Get available slots
            var getResponse = await _client.GetAsync($"/api/slots/available?MondayDate={mondayDate}");
            Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

            var slots = await getResponse.Content.ReadFromJsonAsync<IEnumerable<AvailableSlotResponse>>();

            Assert.NotNull(slots);

            if (slots.Any())
            {
                // Step 2: Book the first available slot
                var firstSlot = slots.First();
                var bookRequest = new BookSlotRequest
                {
                    FacilityId = firstSlot.FacilityId,
                    Start = firstSlot.Start,
                    End = firstSlot.End,
                    Name = "John",
                    SecondName = "Doe",
                    Email = "test@mail.com",
                    Phone = "666999666",
                    Comments = "Test booking"
                };

                var postResponse = await _client.PostAsJsonAsync("/api/slots/book", bookRequest);
                Assert.Equal(HttpStatusCode.OK, postResponse.StatusCode);

                var responseMessage = await postResponse.Content.ReadAsStringAsync();
                responseMessage = responseMessage.Trim('"');
                Assert.Equal("Slot booked successfully.", responseMessage);

                // Step 3: Verify the slot is no longer available
                var getResponseAfterBooking = await _client.GetAsync($"/api/slots/available?MondayDate={mondayDate}");
                Assert.Equal(HttpStatusCode.OK, getResponseAfterBooking.StatusCode);

                var updatedSlots = await getResponseAfterBooking.Content.ReadFromJsonAsync<IEnumerable<AvailableSlotResponse>>();
                Assert.NotNull(updatedSlots);
                Assert.DoesNotContain(updatedSlots, s => s.FacilityId == firstSlot.FacilityId && s.Start == firstSlot.Start && s.End == firstSlot.End);
            }
        }
    }

    public class AvailableSlotResponse
    {
        public string FacilityId { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
    }

    public class BookSlotRequest
    {
        public string FacilityId { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string Name { get; set; }
        public string SecondName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Comments { get; set; }
    }
}