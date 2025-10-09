using OrderAccumulator.Domain.Aggregates.ExposureAggregate;

namespace OrderAccumulator.Domain.Interfaces;

public interface IExposureRepository
{
    Task<Exposure> GetBySymbolAsync(string symbol);
    Task UpdateAsync(Exposure exposure);
    Task<List<Exposure>> GetAllAsync();
}