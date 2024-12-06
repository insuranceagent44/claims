using Claims.Audit.Models;
using MassTransit;

namespace Claims.Audit.Consumers;

public class ClaimAuditConsumer(AuditContext auditContext) : IConsumer<ClaimAudit>
{
    public async Task Consume(ConsumeContext<ClaimAudit> context)
    {
        auditContext.Add(context.Message);
        await auditContext.SaveChangesAsync();
    }
}