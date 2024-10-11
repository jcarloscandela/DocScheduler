using DocScheduler.Application;
using System.Text.Json;

namespace DocScheduler.API
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (InvalidModelException ex)
            {
                await HandleValidationExceptionAsync(context, ex);
            }
            catch (SlotNotFoundException ex)
            {
                await HandleSlotNotFoundExceptionAsync(context, ex);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleValidationExceptionAsync(HttpContext context, InvalidModelException exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = StatusCodes.Status400BadRequest;

            var errors = exception.Errors.Select(e => new
            {
                e.PropertyName,
                e.ErrorMessage
            });

            var result = JsonSerializer.Serialize(new { Errors = errors });
            return context.Response.WriteAsync(result);
        }

        private static Task HandleSlotNotFoundExceptionAsync(HttpContext context, SlotNotFoundException exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = StatusCodes.Status404NotFound;

            var result = JsonSerializer.Serialize(new { Error = exception.Message });
            return context.Response.WriteAsync(result);
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;

            var result = JsonSerializer.Serialize(new { Error = exception.Message });
            return context.Response.WriteAsync(result);
        }
    }
}