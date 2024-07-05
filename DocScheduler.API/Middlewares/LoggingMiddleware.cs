namespace DocScheduler.API.Middlewares
{
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<LoggingMiddleware> _logger;

        public LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Log request information
            LogRequest(context);

            // Call the next delegate/middleware in the pipeline
            await _next(context);

            // Log response information
            LogResponse(context);
        }

        private void LogRequest(HttpContext context)
        {
            _logger.LogInformation("Request {Method} {Path} received", context.Request.Method, context.Request.Path);
        }

        private void LogResponse(HttpContext context)
        {
            if (context.Response.StatusCode == 500)
            {
                _logger.LogError("Response {StatusCode} returned", context.Response.StatusCode);
                return;
            }

            _logger.LogInformation("Response {StatusCode} returned", context.Response.StatusCode);
        }
    }
}
