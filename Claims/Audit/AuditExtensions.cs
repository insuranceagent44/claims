using MassTransit;

namespace Claims.Audit;

public static class AuditExtensions
{
    public static IServiceCollection AddAudit(this IServiceCollection services)
    {
        services = services.AddMassTransit(conf =>
        {
            conf.SetKebabCaseEndpointNameFormatter();
            conf.AddConsumers(typeof(Program).Assembly);
            conf.UsingInMemory((context, config) =>
            {
                config.ConfigureEndpoints(context);
            });
        });

        return services;
    }
}