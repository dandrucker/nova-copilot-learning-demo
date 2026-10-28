using FluentValidation;
using MeterReadings.Api.Models;

namespace MeterReadings.Api.Validation;

public class MeterReadingRequestValidator : AbstractValidator<MeterReadingRequest>
{
    private static readonly string[] AllowedUnits = ["MCF", "CCF", "M3"];

    public MeterReadingRequestValidator(TimeProvider timeProvider)
    {
        RuleFor(r => r.MeterId)
            .NotEmpty()
            .MaximumLength(20);

        RuleFor(r => r.Volume)
            .GreaterThanOrEqualTo(0);

        RuleFor(r => r.Unit)
            .NotEmpty()
            .Must(unit => AllowedUnits.Contains(unit))
            .WithMessage("Unit must be one of: MCF, CCF, M3.");

        RuleFor(r => r.ReadAt)
            .Must(readAt => readAt <= timeProvider.GetUtcNow())
            .WithMessage("ReadAt cannot be in the future.");
    }
}
