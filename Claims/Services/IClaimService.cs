using Claims.Models;

namespace Claims.Services;

public interface IClaimService
{
    public Task<IEnumerable<Claim>> GetClaimsAsync();
    public Task<Claim?> GetClaimAsync(string id);
    public Task AddClaimAsync(Claim item);
    public Task DeleteClaimAsync(string id);
}