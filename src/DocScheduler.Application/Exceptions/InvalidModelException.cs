using FluentValidation.Results;

namespace DocScheduler.Application
{
    public class InvalidModelException : Exception
    {
        public List<ValidationFailure> Errors { get; }

        public InvalidModelException(IEnumerable<ValidationFailure> failures)
            : base("One or more validation errors occurred.")
        {
            Errors = new List<ValidationFailure>(failures);
        }

        public override string ToString()
        {
            var errorMessages = new System.Text.StringBuilder();
            foreach (var failure in Errors)
            {
                errorMessages.AppendLine($"{failure.PropertyName}: {failure.ErrorMessage}");
            }
            return errorMessages.ToString();
        }
    }
}
