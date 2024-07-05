using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace DocScheduler.SlotService
{
    public class SlotServiceClient : ISlotServiceClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<SlotServiceClient> _logger;

        public SlotServiceClient(ILogger<SlotServiceClient> logger, HttpClient httpClient, SlotServiceClientOptions options)
        {
            _logger = logger;
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(options.BaseUrl);

            var authToken = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{options.Username}:{options.Password}"));
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authToken);
        }

        public async Task<WeeklyAvailabilityResponse> GetWeeklyAvailabilityAsync(string monday)
        {
            return await ExecuteRequestAsync<WeeklyAvailabilityResponse>(
                HttpMethod.Get,
                $"GetWeeklyAvailability/{monday}",
                "Error fetching weekly availability"
            );
        }

        public async Task<TakeSlotResponse> TakeSlotAsync(TakeSlotRequest request)
        {
            var response = await ExecuteRequestAsync<string>(
                HttpMethod.Post,
                "TakeSlot",
                "Error taking slot",
                request
            );

            if (string.IsNullOrEmpty(response))
            {
                return new TakeSlotResponse { Success = true };
            }

            return new TakeSlotResponse { Success = false, Message = response };
        }

        private async Task<T> ExecuteRequestAsync<T>(HttpMethod method, string endpoint, string errorMessage, object requestData = null)
        {
            var url = $"{_httpClient.BaseAddress}/{endpoint}";
            HttpResponseMessage response;

            if (method == HttpMethod.Get)
            {
                response = await _httpClient.GetAsync(url);
            }
            else if (method == HttpMethod.Post)
            {
                var jsonContent = JsonSerializer.Serialize(requestData);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                response = await _httpClient.PostAsync(url, content);
            }
            else
            {
                throw new ArgumentException("Unsupported HTTP method");
            }

            if (!response.IsSuccessStatusCode)
            {
                var errorDetails = await GetErrorDetails(response);
                _logger.LogError("{ErrorMessage}. Status code: {StatusCode}, Reason: {ReasonPhrase}, Error details: {ErrorDetails}",
                    errorMessage, response.StatusCode, response.ReasonPhrase, errorDetails);
                throw new HttpRequestException($"{errorMessage}. Status code: {response.StatusCode}, Reason: {response.ReasonPhrase}");
            }

            var responseContent = await response.Content.ReadAsStringAsync();

            if (string.IsNullOrEmpty(responseContent))
            {
                return default;
            }

            return JsonSerializer.Deserialize<T>(responseContent);
        }

        private async Task<string> GetErrorDetails(HttpResponseMessage response)
        {
            try
            {
                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to read error details from HttpResponseMessage");
                return "Failed to read error details";
            }
        }
    }
}