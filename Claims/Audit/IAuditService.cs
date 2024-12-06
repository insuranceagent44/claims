namespace Claims.Audit;

public interface IAuditService
{
    public Task AuditClaim(string id, string httpRequestType);
    public Task AuditCover(string id, string httpRequestType);
}