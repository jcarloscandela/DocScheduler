using DocScheduler.SlotService;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

namespace DocScheduler.Application
{
    public static class ServiceExtensions
    {
        public static void AddApplicationServices(this IServiceCollection services, SlotServiceClientOptions options)
        {
            services.AddAutoMapper(typeof(ServiceExtensions));

            services.TryAddScoped<ISlotServiceClient>(provider =>
            {
                var logger = provider.GetRequiredService<ILogger<SlotServiceClient>>();
                return new SlotServiceClient(logger, options);
            });

            services.TryAddScoped<IAppointmentService, AppointmentService>();
        }
    }
}
