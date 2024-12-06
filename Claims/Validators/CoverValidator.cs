using Claims.Models;
using FluentValidation;

namespace Claims.Validators;

public class CoverValidator : AbstractValidator<Cover>
{
    public CoverValidator()
    {
        RuleFor(cover => cover.StartDate)
            .NotNull()
            .NotEmpty()
            .GreaterThanOrEqualTo(DateTime.Now)
            .WithMessage("Start date cannot be in the past.");

        RuleFor(cover => cover.EndDate)
            .NotNull()
            .NotEmpty()
            .Must((cover, value, cancellation) => cover.EndDate <= cover.StartDate.AddYears(1))
            .WithMessage("Total insurance period cannot exceed 1 year.");
    }
}