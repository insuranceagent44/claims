using Claims.Models;
using Claims.PremiumCalculator;
using Microsoft.EntityFrameworkCore;

namespace Claims.Services;

public class CoverService : ICoverService
{
    private ClaimsContext _claimsContext;
    private IPremiumCalculator _premiumCalculator;

    public CoverService(ClaimsContext claimsContext, IPremiumCalculator premiumCalculator)
    {
        _claimsContext = claimsContext;
        _premiumCalculator = premiumCalculator;
    }

    public async Task<IEnumerable<Cover>> GetCoversAsync()
    {
        return await _claimsContext.Covers.ToListAsync();
    }

    public async Task<Cover?> GetCoverAsync(string id)
    {
        return await _claimsContext.Covers.SingleOrDefaultAsync(cover => cover.Id == id);
    }

    public async Task AddCoverAsync(Cover cover)
    {
        cover.Id = Guid.NewGuid().ToString();
        cover.Premium = _premiumCalculator.Compute(cover.StartDate, cover.EndDate, cover.Type);
        _claimsContext.Covers.Add(cover);
        await _claimsContext.SaveChangesAsync();
    }

    public async Task DeleteCoverAsync(string id)
    {
        var cover = await _claimsContext.Covers.SingleOrDefaultAsync(cover => cover.Id == id);
        if (cover is not null)
        {
            _claimsContext.Covers.Remove(cover);
            await _claimsContext.SaveChangesAsync();
        }
    }
}