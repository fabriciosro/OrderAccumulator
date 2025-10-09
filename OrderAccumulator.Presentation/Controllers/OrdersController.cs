using Microsoft.AspNetCore.Mvc;
using MediatR;
using OrderAccumulator.Application.Commands;

namespace OrderAccumulator.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<OrdersController> _logger;

    public OrdersController(IMediator mediator, ILogger<OrdersController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpPost]
    public async Task<ActionResult<OrderResult>> CreateOrder([FromBody] CreateOrderCommand command)
    {
        try
        {
            _logger.LogInformation("Received order via API - ClOrdID: {ClOrdID}, Symbol: {Symbol}, Side: {Side}",
                command.ClOrdID, command.Symbol, command.Side);

            var result = await _mediator.Send(command);

            if (result.IsAccepted)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing order via API");
            return StatusCode(500, new { error = ex.Message });
        }
    }
}