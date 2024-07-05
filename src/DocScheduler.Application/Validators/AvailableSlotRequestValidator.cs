using FluentValidation;

namespace DocScheduler.Application
{
    public class AvailableSlotRequestValidator : AbstractValidator<AvailableSlotRequest>
    {
        public AvailableSlotRequestValidator()
        {
            RuleFor(x => x.MondayDate)
                .Must(BeMonday).WithMessage("The date must be a Monday.");
        }

        private bool BeMonday(DateOnly date)
        {
            return date.DayOfWeek == DayOfWeek.Monday;
        }
    }
}
