using DocScheduler.SlotService;

namespace DocScheduler.API
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Configure settings
            var configuration = builder.Configuration;

            IConfigurationSection slotServiceSection = configuration.GetSection("SlotService");

            var slotServiceOptions = new SlotServiceClientOptions();
            slotServiceSection.Bind(slotServiceOptions);

            // Add services to the container.
            builder.Services.Configure<SlotServiceClientOptions>(slotServiceSection);
            builder.Services.AddApplicationServices(slotServiceOptions);
            builder.Services.AddControllers();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            app.UseMiddleware<LoggingMiddleware>();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.MapControllers();

            app.UseMiddleware<ExceptionMiddleware>();

            app.Run();
        }
    }
}