using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using QuickFix;

namespace OrderAccumulator.Infrastructure.Fix;

public class FixAcceptorService : IHostedService
{
    private readonly IAcceptor _acceptor;
    private readonly ILogger<FixAcceptorService> _logger;
    private bool _isStarted = false;

    public FixAcceptorService(IAcceptor acceptor, ILogger<FixAcceptorService> logger)
    {
        _acceptor = acceptor;
        _logger = logger;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            if (!_isStarted)
            {
                _logger.LogInformation("🚀 Starting FIX Acceptor...");
                _acceptor.Start();
                _isStarted = true;
                _logger.LogInformation("✅ FIX Acceptor started successfully on port 9810");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Failed to start FIX Acceptor");
            throw;
        }

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        try
        {
            if (_isStarted)
            {
                _logger.LogInformation("🛑 Stopping FIX Acceptor...");
                _acceptor.Stop();
                _isStarted = false;
                _logger.LogInformation("✅ FIX Acceptor stopped successfully");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error stopping FIX Acceptor");
        }

        return Task.CompletedTask;
    }
}