using DocScheduler.Application;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;

namespace DocScheduler.API.Test
{
    public class ExceptionMiddlewareTests
    {
        [Fact]
        public async Task InvokeAsync_WithInvalidModelException_ReturnsBadRequest()
        {
            // Arrange
            var context = new DefaultHttpContext();
            var mockNext = new Mock<RequestDelegate>();
            var validationFailures = new List<ValidationFailure>
            {
                new ValidationFailure("property", "property is not correct")
            };
            mockNext.Setup(next => next(context)).ThrowsAsync(new InvalidModelException(validationFailures));
            var middleware = new ExceptionMiddleware(mockNext.Object);

            // Act
            await middleware.InvokeAsync(context);

            // Assert
            Assert.Equal(StatusCodes.Status400BadRequest, context.Response.StatusCode);
            Assert.Equal("application/json", context.Response.ContentType);
        }

        [Fact]
        public async Task InvokeAsync_WithSlotNotFoundException_ReturnsNotFound()
        {
            // Arrange
            var context = new DefaultHttpContext();
            var mockNext = new Mock<RequestDelegate>();
            mockNext.Setup(next => next(context)).ThrowsAsync(new SlotNotFoundException("Slot not found"));
            var middleware = new ExceptionMiddleware(mockNext.Object);

            // Act
            await middleware.InvokeAsync(context);

            // Assert
            Assert.Equal(StatusCodes.Status404NotFound, context.Response.StatusCode);
            Assert.Equal("application/json", context.Response.ContentType);
        }

        [Fact]
        public async Task InvokeAsync_WithGeneralException_ReturnsInternalServerError()
        {
            // Arrange
            var context = new DefaultHttpContext();
            var mockNext = new Mock<RequestDelegate>();
            mockNext.Setup(next => next(context)).ThrowsAsync(new Exception("Some error occurred"));
            var middleware = new ExceptionMiddleware(mockNext.Object);

            // Act
            await middleware.InvokeAsync(context);

            // Assert
            Assert.Equal(StatusCodes.Status500InternalServerError, context.Response.StatusCode);
            Assert.Equal("application/json", context.Response.ContentType);
        }
    }
}