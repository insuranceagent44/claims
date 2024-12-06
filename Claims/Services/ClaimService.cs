using Claims.Models;
using Microsoft.EntityFrameworkCore;

namespace Claims.Services;

public class ClaimService : IClaimService
{
    private readonly ClaimsContext _claimsContext;

    public ClaimService(ClaimsContext claimsContext)
    {
        _claimsContext = claimsContext;
    }
    
    public async Task<IEnumerable<Claim>> GetClaimsAsync()
    {
        return await _claimsContext.Claims.ToListAsync();
    }

    public async Task<Claim?> GetClaimAsync(string id)
    {
        return await _claimsContext.Claims.SingleOrDefaultAsync(claim => claim.Id == id);
    }

    public async Task AddClaimAsync(Claim item)
    {
        item.Id = Guid.NewGuid().ToString();
        _claimsContext.Claims.Add(item);
        await _claimsContext.SaveChangesAsync();
    }

    public async Task DeleteClaimAsync(string id)
    {
        var claim = await GetClaimAsync(id);
        if (claim is not null)
        {
            _claimsContext.Claims.Remove(claim);
            await _claimsContext.SaveChangesAsync();
        }
    }
}