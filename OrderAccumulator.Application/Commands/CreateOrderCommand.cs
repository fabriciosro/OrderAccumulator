using MediatR;
using OrderAccumulator.Domain.Aggregates.OrderAggregate;

namespace OrderAccumulator.Application.Commands;

public class CreateOrderCommand : IRequest<OrderResult>
{
    public string ClOrdID { get; set; }
    public string Symbol { get; set; }
    public OrderSide Side { get; set; }
    public decimal Quantity { get; set; }
    public decimal Price { get; set; }
    public DateTime TransactTime { get; set; }
}

public class OrderResult
{
    public bool IsAccepted { get; set; }
    public string ClOrdID { get; set; }
    public string Symbol { get; set; }
    public decimal CurrentExposure { get; set; }
    public string RejectionReason { get; set; }
    public decimal OrderValue { get; set; }

    public static OrderResult Accepted(string clOrdId, string symbol, decimal currentExposure, decimal orderValue)
    {
        return new OrderResult
        {
            IsAccepted = true,
            ClOrdID = clOrdId,
            Symbol = symbol,
            CurrentExposure = currentExposure,
            OrderValue = orderValue
        };
    }

    public static OrderResult Rejected(string clOrdId, string symbol, string reason, decimal orderValue)
    {
        return new OrderResult
        {
            IsAccepted = false,
            ClOrdID = clOrdId,
            Symbol = symbol,
            RejectionReason = reason,
            OrderValue = orderValue
        };
    }
}