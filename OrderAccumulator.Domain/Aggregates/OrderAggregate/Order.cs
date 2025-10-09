using OrderAccumulator.Domain.Common;

namespace OrderAccumulator.Domain.Aggregates.OrderAggregate;

public class Order : Entity
{
    public string ClOrdID { get; private set; }
    public string Symbol { get; private set; }
    public OrderSide Side { get; private set; }
    public decimal Quantity { get; private set; }
    public decimal Price { get; private set; }
    public DateTime TransactTime { get; private set; }
    public OrderStatus Status { get; private set; }

    private Order() { }

    public Order(string clOrdId, string symbol, OrderSide side, decimal quantity, decimal price, DateTime transactTime)
    {
        ClOrdID = clOrdId;
        Symbol = symbol;
        Side = side;
        Quantity = quantity;
        Price = price;
        TransactTime = transactTime;
        Status = OrderStatus.Pending;

        Validate();
    }

    private void Validate()
    {
        if (string.IsNullOrWhiteSpace(ClOrdID))
            throw new DomainException("ClOrdID é obrigatório");

        if (string.IsNullOrWhiteSpace(Symbol))
            throw new DomainException("Symbol é obrigatório");

        if (Quantity <= 0)
            throw new DomainException("Quantity deve ser positiva");

        if (Price <= 0)
            throw new DomainException("Price deve ser positivo");
    }

    public void MarkAsAccepted()
    {
        Status = OrderStatus.Accepted;
    }

    public void MarkAsRejected(string reason = null)
    {
        Status = OrderStatus.Rejected;
    }

    public decimal CalculateValue()
    {
        return Price * Quantity;
    }
}

public enum OrderSide
{
    Buy = 1,
    Sell = 2
}

public enum OrderStatus
{
    Pending,
    Accepted,
    Rejected
}