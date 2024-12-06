using Claims.Models;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Claims.Validators;

public class ClaimValidator : AbstractValidator<Claim>
{
    private readonly ClaimsContext _context;
    
    public ClaimValidator(ClaimsContext context)
    {
        _context = context;
        
        RuleFor(claim => claim.DamageCost)
            .NotNull()
            .NotEmpty()
            .LessThanOrEqualTo(100000)
            .WithMessage("Claims can not exceed 100000.");

        RuleFor(claim => claim.Created)
            .NotNull()
            .NotEmpty()
            .MustAsync(async (claim, value, cancellation) => await MustBeCreatedWithinCoverPeriod(claim))
            .WithMessage("Created date must be within cover period.");
    }
    
    private async Task<bool> MustBeCreatedWithinCoverPeriod(Claim claim)
    {
        var cover = await _context.Covers.FirstOrDefaultAsync(c => c.Id == claim.CoverId);
        if (cover is null)
        {
            return false;
        }
        
        return claim.Created >= cover.StartDate 
               && claim.Created <= cover.EndDate;
    }
}