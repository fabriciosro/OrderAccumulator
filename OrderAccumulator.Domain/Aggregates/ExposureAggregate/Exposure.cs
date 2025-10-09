using OrderAccumulator.Domain.Aggregates.OrderAggregate;
using OrderAccumulator.Domain.Common;

namespace OrderAccumulator.Domain.Aggregates.ExposureAggregate;

public class Exposure : Entity, IAggregateRoot
{
    public string Symbol { get; private set; }
    public decimal CurrentExposure { get; private set; }
    private const decimal LIMIT = 100000000m; // R$ 100.000.000

    private Exposure() { }

    public Exposure(string symbol)
    {
        Symbol = symbol;
        CurrentExposure = 0m;
    }

    public ExposureResult CalculateNewExposure(OrderSide side, decimal orderValue)
    {
        var newExposure = side == OrderSide.Buy
            ? CurrentExposure + orderValue
            : CurrentExposure - orderValue;

        if (Math.Abs(newExposure) > LIMIT)
        {
            return ExposureResult.CreateRejected($"Limite de exposição excedido. Limite: {LIMIT:N2}, Tentativa: {Math.Abs(newExposure):N2}");
        }

        CurrentExposure = newExposure;
        return ExposureResult.CreateAccepted(CurrentExposure);
    }

    public void Reset()
    {
        CurrentExposure = 0m;
    }
}

public class ExposureResult
{
    public bool IsAccepted { get; }
    public decimal NewExposure { get; }
    public string RejectionReason { get; }

    private ExposureResult(bool isAccepted, decimal newExposure, string rejectionReason)
    {
        IsAccepted = isAccepted;
        NewExposure = newExposure;
        RejectionReason = rejectionReason;
    }

    public static ExposureResult CreateAccepted(decimal newExposure)
    {
        return new ExposureResult(true, newExposure, null);
    }

    public static ExposureResult CreateRejected(string reason)
    {
        return new ExposureResult(false, 0, reason);
    }
}