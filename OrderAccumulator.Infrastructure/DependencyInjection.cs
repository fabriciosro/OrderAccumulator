using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using OrderAccumulator.Domain.Interfaces;
using OrderAccumulator.Infrastructure.Repositories;
using OrderAccumulator.Infrastructure.Fix;
using QuickFix;
using QuickFix.Logger;
using QuickFix.Store;

namespace OrderAccumulator.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Repositories
        services.AddSingleton<IExposureRepository, InMemoryExposureRepository>();

        // FIX Application
        services.AddSingleton<FixApplication>();
        services.AddSingleton<IApplication>(provider => provider.GetRequiredService<FixApplication>());

        // FIX Acceptor - Versão simplificada
        services.AddSingleton(provider =>
        {
            var settings = new SessionSettings("fix-server.cfg");
            var storeFactory = new FileStoreFactory(settings);
            var logFactory = new FileLogFactory(settings);
            var application = provider.GetRequiredService<IApplication>();

            return new ThreadedSocketAcceptor(application, storeFactory, settings, logFactory);
        });

        services.AddSingleton<IAcceptor>(provider => provider.GetRequiredService<ThreadedSocketAcceptor>());
        services.AddHostedService<FixAcceptorService>();

        return services;
    }
}