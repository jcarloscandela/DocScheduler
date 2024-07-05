using DocScheduler.Application;
using DocScheduler.SlotService;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;
using System.Text;

public static class ServiceExtensions
{
    public static void AddApplicationServices(this IServiceCollection services, SlotServiceClientOptions options)
    {
        services.AddAutoMapper(typeof(ServiceExtensions));

        // Register SlotServiceClientOptions
        services.AddSingleton(options);

        // Configure HttpClient for SlotServiceClient
        services.AddHttpClient<ISlotServiceClient, SlotServiceClient>((provider, httpClient) =>
        {
            var logger = provider.GetRequiredService<ILogger<SlotServiceClient>>();
            httpClient.BaseAddress = new Uri(options.BaseUrl);
            var authToken = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{options.Username}:{options.Password}"));
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authToken);
        });

        services.TryAddScoped<IAppointmentService, AppointmentService>();
    }
}