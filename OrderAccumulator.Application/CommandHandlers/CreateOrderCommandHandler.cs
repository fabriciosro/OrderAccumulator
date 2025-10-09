using MediatR;
using Microsoft.Extensions.Logging;
using OrderAccumulator.Application.Commands;
using OrderAccumulator.Domain.Aggregates.ExposureAggregate;
using OrderAccumulator.Domain.Interfaces;

namespace OrderAccumulator.Application.CommandHandlers;

public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, OrderResult>
{
    private readonly IExposureRepository _exposureRepository;
    private readonly ILogger<CreateOrderCommandHandler> _logger;

    public CreateOrderCommandHandler(IExposureRepository exposureRepository, ILogger<CreateOrderCommandHandler> logger)
    {
        _exposureRepository = exposureRepository;
        _logger = logger;
    }

    public async Task<OrderResult> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Validação básica dos dados
            ValidateOrderRequest(request);

            var orderValue = request.Quantity * request.Price;

            var exposure = await _exposureRepository.GetBySymbolAsync(request.Symbol)
                ?? new Exposure(request.Symbol);

            var exposureResult = exposure.CalculateNewExposure(request.Side, orderValue);

            if (exposureResult.IsAccepted)
            {
                await _exposureRepository.UpdateAsync(exposure);

                _logger.LogInformation("Order {ClOrdID} ACCEPTED - Symbol: {Symbol}, Side: {Side}, Value: {Value}, New Exposure: {Exposure}",
                    request.ClOrdID, request.Symbol, request.Side, orderValue, exposureResult.NewExposure);

                return OrderResult.Accepted(request.ClOrdID, request.Symbol, exposureResult.NewExposure, orderValue);
            }
            else
            {
                _logger.LogWarning("Order {ClOrdID} REJECTED - Symbol: {Symbol}, Side: {Side}, Value: {Value}, Reason: {Reason}",
                    request.ClOrdID, request.Symbol, request.Side, orderValue, exposureResult.RejectionReason);

                return OrderResult.Rejected(request.ClOrdID, request.Symbol, exposureResult.RejectionReason, orderValue);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing order {ClOrdID}", request.ClOrdID);
            return OrderResult.Rejected(request.ClOrdID, request.Symbol, $"Erro interno: {ex.Message}", 0);
        }
    }

    private void ValidateOrderRequest(CreateOrderCommand request)
    {
        if (string.IsNullOrWhiteSpace(request.ClOrdID))
            throw new ArgumentException("ClOrdID é obrigatório");

        if (string.IsNullOrWhiteSpace(request.Symbol))
            throw new ArgumentException("Symbol é obrigatório");

        var validSymbols = new[] { "PETR4", "VALE3", "VIIA4" };
        if (!validSymbols.Contains(request.Symbol))
            throw new ArgumentException($"Símbolo inválido: {request.Symbol}");

        if (request.Quantity <= 0 || request.Quantity >= 100000)
            throw new ArgumentException("Quantidade deve ser positiva e menor que 100.000");

        if (request.Price <= 0 || request.Price >= 1000)
            throw new ArgumentException("Preço deve ser positivo e menor que 1.000");

        if (request.Price * 100 % 1 != 0)
            throw new ArgumentException("Preço deve ser múltiplo de 0.01");
    }
}