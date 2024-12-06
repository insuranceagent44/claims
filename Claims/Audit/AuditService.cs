using Claims.Audit.Models;
using MassTransit;

namespace Claims.Audit;

public class AuditService : IAuditService
{
    private readonly IBus _bus;

    public AuditService(IBus bus)
    {
        _bus = bus;
    }

    public async Task AuditClaim(string id, string httpRequestType)
    {
        var claimAudit = new ClaimAudit
        {
            Created = DateTime.Now,
            HttpRequestType = httpRequestType,
            ClaimId = id
        };
        
        await _bus.Publish(claimAudit);
    }
        
    public async Task AuditCover(string id, string httpRequestType)
    {
        var coverAudit = new CoverAudit
        {
            Created = DateTime.Now,
            HttpRequestType = httpRequestType,
            CoverId = id
        };

        await _bus.Publish(coverAudit);
    }
}