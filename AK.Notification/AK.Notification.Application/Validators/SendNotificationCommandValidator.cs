using AK.Notification.Application.Commands;
using FluentValidation;

namespace AK.Notification.Application.Validators;

public sealed class SendNotificationCommandValidator : AbstractValidator<SendNotificationCommand>
{
    public SendNotificationCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty().WithMessage("UserId is required.");
        RuleFor(x => x.RecipientAddress).NotEmpty().WithMessage("RecipientAddress is required.");
    }
}
