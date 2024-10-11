using FluentValidation;

namespace DocScheduler.Application
{
    public static class Validator
    {
        public static async Task ValidateRequestAsync<TRequest, TValidator>(TRequest request)
            where TValidator : AbstractValidator<TRequest>, new()
        {
            var validator = new TValidator();
            var results = await validator.ValidateAsync(request);

            if (!results.IsValid)
            {
                throw new InvalidModelException(results.Errors);
            }
        }
    }
}
