using Claims.Audit.Models;
using MassTransit;

namespace Claims.Audit.Consumers;

public class CoverAuditConsumer(AuditContext auditContext) : IConsumer<CoverAudit>
{
    public async Task Consume(ConsumeContext<CoverAudit> context)
    {
        auditContext.Add(context.Message);
        await auditContext.SaveChangesAsync();
    }
}