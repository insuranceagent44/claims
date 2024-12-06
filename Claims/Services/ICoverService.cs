using Claims.Models;

namespace Claims.Services;

public interface ICoverService
{
    public Task<IEnumerable<Cover>> GetCoversAsync();
    public Task<Cover?> GetCoverAsync(string id);
    public Task AddCoverAsync(Cover cover);
    public Task DeleteCoverAsync(string id);
}