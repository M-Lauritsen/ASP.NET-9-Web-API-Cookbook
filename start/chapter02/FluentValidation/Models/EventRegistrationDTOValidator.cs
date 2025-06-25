using FluentValidation;

namespace FluentExample.Models;

public class EventRegistrationDTOValidator : AbstractValidator<EventRegistrationDTO>
{
    public EventRegistrationDTOValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Full name is required.");
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.");
        RuleFor(x => x.EventName)
            .NotEmpty().WithMessage("Event name is required.");
        RuleFor(x => x.EventDate)
            .NotEmpty().WithMessage("Event date is required.");
        RuleFor(x => x.ConfirmEmail)
            .NotEmpty().WithMessage("Confirm email is required.");
        RuleFor(x => x.DaysAttending)
            .NotEmpty().WithMessage("Days attending is required.");
    }
}
