using FluentValidation;

namespace DocScheduler.Application
{
    public class BookSlotRequestValidator : AbstractValidator<BookSlotRequest>
    {
        public BookSlotRequestValidator()
        {
            RuleFor(x => x.FacilityId).NotEmpty().WithMessage("FacilityId cannot be empty.");
            RuleFor(x => x.Start)
                .NotEmpty().WithMessage("Start date cannot be empty.")
                .GreaterThan(DateTime.Now).WithMessage("Start date must be in the future.");
            RuleFor(x => x.End)
                .NotEmpty().WithMessage("End date cannot be empty.")
                .GreaterThan(x => x.Start).WithMessage("End date must be greater than start date.");
            RuleFor(x => x.Comments).NotEmpty().WithMessage("Comments cannot be empty.");
            RuleFor(x => x.Name).NotEmpty().WithMessage("Name cannot be empty.");
            RuleFor(x => x.SecondName).NotEmpty().WithMessage("SecondName cannot be empty.");
            RuleFor(x => x.Email).NotEmpty().WithMessage("Email cannot be empty.")
                                 .EmailAddress().WithMessage("Email is not a valid email address.");
            RuleFor(x => x.Phone).NotEmpty().WithMessage("Phone cannot be empty.");
        }
    }
}
