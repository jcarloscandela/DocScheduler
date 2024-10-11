using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using System.Net;
using System.Text;
using System.Text.Json;

namespace DocScheduler.SlotService.Test
{
    public class SlotServiceClientTests
    {
        private readonly Mock<ILogger<SlotServiceClient>> _mockLogger;
        private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
        private readonly HttpClient _httpClient;
        private readonly SlotServiceClientOptions _options;
        private readonly SlotServiceClient _client;

        public SlotServiceClientTests()
        {
            _mockLogger = new Mock<ILogger<SlotServiceClient>>();
            _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            _httpClient = new HttpClient(_mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri("https://example.com")
            };
            _options = new SlotServiceClientOptions
            {
                BaseUrl = "https://example.com",
                Username = "user",
                Password = "password"
            };
            _client = new SlotServiceClient(_mockLogger.Object, _httpClient, _options);
        }

        [Fact]
        public async Task TakeSlotAsync_Successful_ReturnsSuccessResponse()
        {
            // Arrange
            var slotServiceClient = CreateClient();
            var takeSlotRequest = CreateTakeSlotRequest();
            SetupHttpResponse(HttpMethod.Post, "/TakeSlot", HttpStatusCode.OK, string.Empty);

            // Act
            var response = await slotServiceClient.TakeSlotAsync(takeSlotRequest);

            // Assert
            Assert.True(response.Success);
            Assert.Null(response.Message);
        }

        [Fact]
        public async Task TakeSlotAsync_Failed_ReturnsErrorResponse()
        {
            // Arrange
            var slotServiceClient = CreateClient();
            var takeSlotRequest = CreateTakeSlotRequest();
            var errorMessage = "Slot is already taken.";
            SetupHttpResponse(HttpMethod.Post, "/TakeSlot", HttpStatusCode.BadRequest, errorMessage);

            // Act
            HttpRequestException exception = await Assert.ThrowsAsync<HttpRequestException>(() => slotServiceClient.TakeSlotAsync(takeSlotRequest));

            // Assert
            Assert.Contains("Error taking slot", exception.Message);
            Assert.Contains("BadRequest", exception.Message);
        }

        [Fact]
        public async Task GetWeeklyAvailabilityAsync_ReturnsExpectedResponse()
        {
            // Arrange
            var facilityId = Guid.NewGuid();
            var expectedResponse = new WeeklyAvailabilityResponse
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

            var jsonResponse = JsonSerializer.Serialize(expectedResponse);
            SetupHttpResponse(HttpMethod.Get, "/GetWeeklyAvailability/monday", HttpStatusCode.OK, jsonResponse);

            // Act
            var actualResponse = await _client.GetWeeklyAvailabilityAsync("monday");

            // Assert
            Assert.Equal(expectedResponse.Monday.BusySlots.Count, actualResponse.Monday.BusySlots.Count);
        }

        private void SetupHttpResponse(HttpMethod method, string endpoint, HttpStatusCode statusCode, string content)
        {
            _mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req => req.Method == method && req.RequestUri.ToString().EndsWith(endpoint)),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = statusCode,
                    Content = new StringContent(content, Encoding.UTF8, "application/json")
                });
        }

        private SlotServiceClient CreateClient()
        {
            return new SlotServiceClient(_mockLogger.Object, _httpClient, new SlotServiceClientOptions
            {
                BaseUrl = "https://example.com",
                Username = "testuser",
                Password = "testpassword"
            });
        }

        private TakeSlotRequest CreateTakeSlotRequest()
        {
            return new TakeSlotRequest
            {
                FacilityId = Guid.NewGuid(),
                Start = DateTime.UtcNow.AddDays(1).ToString("yyyy-MM-ddTHH:mm:ss"),
                End = DateTime.UtcNow.AddDays(1).AddMinutes(30).ToString("yyyy-MM-ddTHH:mm:ss"),
                Comments = "Patient requires wheelchair access",
                Patient = new Patient
                {
                    Name = "John",
                    SecondName = "Doe",
                    Email = "john.doe@example.com",
                    Phone = "1234567890"
                }
            };
        }
    }
}