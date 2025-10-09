using QuickFix;
using QuickFix.Fields;
using QuickFix.FIX44;
using MediatR;
using OrderAccumulator.Application.Commands;
using OrderAccumulator.Domain.Aggregates.OrderAggregate;
using Microsoft.Extensions.Logging;
using Message = QuickFix.Message;

namespace OrderAccumulator.Infrastructure.Fix;

public class FixApplication : MessageCracker, IApplication
{
    private readonly IMediator _mediator;
    private readonly ILogger<FixApplication> _logger;

    public FixApplication(IMediator mediator, ILogger<FixApplication> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    #region IApplication Implementation
    public void FromAdmin(Message message, SessionID sessionID)
    {
        _logger.LogDebug("Admin message received: {MessageType}", message.GetType().Name);
    }

    public void ToAdmin(Message message, SessionID sessionID)
    {
        _logger.LogDebug("Sending admin message: {MessageType}", message.GetType().Name);
    }

    public void FromApp(Message message, SessionID sessionID)
    {
        Crack(message, sessionID);
    }

    public void ToApp(Message message, SessionID sessionID)
    {
        _logger.LogDebug("Sending app message: {MessageType}", message.GetType().Name);
    }

    public void OnCreate(SessionID sessionID)
    {
        _logger.LogInformation("FIX Session created: {SessionID}", sessionID);
    }

    public void OnLogout(SessionID sessionID)
    {
        _logger.LogInformation("FIX Session logout: {SessionID}", sessionID);
    }

    public void OnLogon(SessionID sessionID)
    {
        _logger.LogInformation("FIX Session logon: {SessionID}", sessionID);
    }
    #endregion

    #region MessageCracker Methods
    public void OnMessage(NewOrderSingle message, SessionID sessionID)
    {
        _ = ProcessNewOrderSingleAsync(message, sessionID);
    }

    private async Task ProcessNewOrderSingleAsync(NewOrderSingle message, SessionID sessionID)
    {
        try
        {
            var sideChar = message.Side.getValue();

            OrderSide side = sideChar == '1' ? OrderSide.Buy : OrderSide.Sell;

            var command = new CreateOrderCommand
            {
                ClOrdID = message.ClOrdID.getValue(),
                Symbol = message.Symbol.getValue(),
                Side = side,
                Quantity = message.OrderQty.getValue(),
                Price = message.Price.getValue(),
                TransactTime = message.TransactTime.getValue()
            };

            _logger.LogInformation("Processing order: {ClOrdID}", command.ClOrdID);

            var result = await _mediator.Send(command);

            var executionReport = CreateExecutionReport(message, result);
            Session.SendToTarget(executionReport, sessionID);

            _logger.LogInformation("Order {ClOrdID} processed: {Status}",
                command.ClOrdID, result.IsAccepted ? "Accepted" : "Rejected");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing order");
        }
    }
    #endregion

    #region Private Methods
    private ExecutionReport CreateExecutionReport(NewOrderSingle message, OrderResult result)
    {
        var executionReport = new ExecutionReport(
            new OrderID(result.ClOrdID),
            new ExecID(Guid.NewGuid().ToString()),
            new ExecType(result.IsAccepted ? ExecType.NEW : ExecType.REJECTED),
            new OrdStatus(result.IsAccepted ? OrdStatus.NEW : OrdStatus.REJECTED),
            new Symbol(result.Symbol),
            new Side(message.Side.getValue()),
            new LeavesQty(message.OrderQty.getValue()),
            new CumQty(0),
            new AvgPx(0)
        );

        executionReport.Set(new ClOrdID(result.ClOrdID));
        executionReport.Set(new Symbol(message.Symbol.getValue()));
        executionReport.Set(new OrderQty(message.OrderQty.getValue()));
        executionReport.Set(new Price(message.Price.getValue()));

        if (!result.IsAccepted)
        {
            executionReport.Set(new Text(result.RejectionReason));
        }

        return executionReport;
    }
    #endregion
}