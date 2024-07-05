using DocScheduler.Application;
using InvalidModelException = DocScheduler.Application.InvalidModelException;

namespace DocScheduler.Test
{
    public class ValidatorTests
    {
        [Fact]
        public async Task ValidateRequestAsync_ValidRequest_DoesNotThrowValidationException()
        {
            // Arrange
            var validRequest = new BookSlotRequest
            {
                FacilityId = Guid.NewGuid(),
                Start = DateTime.Now.AddDays(1),
                End = DateTime.Now.AddDays(2),
                Comments = "Test comments",
                Name = "John",
                SecondName = "Doe",
                Email = "john.doe@example.com",
                Phone = "1234567890"
            };

            // Act & Assert
            await Validator.ValidateRequestAsync<BookSlotRequest, BookSlotRequestValidator>(validRequest);

            // Validation should pass without throwing an exception
        }

        [Fact]
        public async Task ValidateRequestAsync_InvalidRequest_ThrowsValidationExceptionWithAllErrors()
        {
            // Arrange
            var invalidRequest = new BookSlotRequest
            {
                FacilityId = Guid.NewGuid(),
                Start = DateTime.Now.AddDays(-1),
                End = DateTime.Now.AddDays(-2),
                Comments = "Test comments",
                Name = "John",
                SecondName = "Doe",
                Email = "john.doe@example.com",
                Phone = "1234567890"
            };

            // Act
            async Task Act() => await Validator.ValidateRequestAsync<BookSlotRequest, BookSlotRequestValidator>(invalidRequest);

            // Assert
            var exception = await Assert.ThrowsAsync<InvalidModelException>(Act);

            // Assert individual error messages (could also check counts etc.)
            Assert.Contains("Start date must be in the future.", exception.ToString());
            Assert.Contains("End date must be greater than start date.", exception.ToString());
        }

        [Fact]
        public async Task ValidateAsync_ValidMondayDate_DoesNotThrowValidationException()
        {
            // Arrange
            var monday = new DateOnly(2024, 7, 8);

            var validRequest = new AvailableSlotRequest
            {
                MondayDate = monday
            };

            // Act & Assert
            await Validator.ValidateRequestAsync<AvailableSlotRequest, AvailableSlotRequestValidator>(validRequest);

            // Validation should pass without throwing a exception
        }

        [Fact]
        public async Task ValidateAsync_InvalidMondayDate_ThrowsValidationExceptionWithErrorMessage()
        {
            var tuesday = new DateOnly(2024, 7, 9);

            var invalidRequest = new AvailableSlotRequest
            {
                MondayDate = tuesday
            };

            // Act
            async Task Act() => await Validator.ValidateRequestAsync<AvailableSlotRequest, AvailableSlotRequestValidator>(invalidRequest);

            // Assert
            var exception = await Assert.ThrowsAsync<InvalidModelException>(Act);
            Assert.Contains("The date must be a Monday.", exception.ToString());
        }
    }
}