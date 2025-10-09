using OrderAccumulator.Application.CommandHandlers;

namespace OrderAccumulator.Presentation;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(CreateOrderCommandHandler).Assembly));
        return services;
    }
}