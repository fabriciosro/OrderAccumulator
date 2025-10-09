using OrderAccumulator.Domain.Aggregates.ExposureAggregate;
using OrderAccumulator.Domain.Interfaces;

namespace OrderAccumulator.Infrastructure.Repositories;

public class InMemoryExposureRepository : IExposureRepository
{
    private readonly Dictionary<string, Exposure> _exposures = new();
    private readonly object _lock = new();

    public Task<Exposure> GetBySymbolAsync(string symbol)
    {
        lock (_lock)
        {
            return Task.FromResult(_exposures.ContainsKey(symbol) ? _exposures[symbol] : null);
        }
    }

    public Task UpdateAsync(Exposure exposure)
    {
        lock (_lock)
        {
            _exposures[exposure.Symbol] = exposure;
            return Task.CompletedTask;
        }
    }

    public Task<List<Exposure>> GetAllAsync()
    {
        lock (_lock)
        {
            return Task.FromResult(_exposures.Values.ToList());
        }
    }
}