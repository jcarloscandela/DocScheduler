namespace DocScheduler.API
{
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<LoggingMiddleware> _logger;

        public LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Log request information
            LogRequest(context);

            // Call the next delegate/middleware in the pipeline
            await _next(context);

            // Log response information
            await LogResponse(context);
        }

        private void LogRequest(HttpContext context)
        {
            _logger.LogInformation("Request {Method} {Path} received", context.Request.Method, context.Request.Path);
        }

        private async Task LogResponse(HttpContext context)
        {
            var statusCode = context.Response.StatusCode;

            if (statusCode == 500)
            {
                // Read the response body asynchronously
                var responseBody = await ReadResponseBody(context.Response);

                _logger.LogError("Response {StatusCode} returned. Error: {ResponseBody}", statusCode, responseBody);
            }
            else
            {
                _logger.LogInformation("Response {StatusCode} returned", statusCode);
            }
        }

        private async Task<string> ReadResponseBody(HttpResponse response)
        {
            response.Body.Seek(0, SeekOrigin.Begin);
            var responseBody = await new StreamReader(response.Body).ReadToEndAsync();
            response.Body.Seek(0, SeekOrigin.Begin);
            return responseBody;
        }
    }
}