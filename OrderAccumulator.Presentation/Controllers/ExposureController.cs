using Microsoft.AspNetCore.Mvc;
using OrderAccumulator.Domain.Interfaces;
using OrderAccumulator.Domain.Aggregates.ExposureAggregate;

namespace OrderAccumulator.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ExposureController : ControllerBase
{
    private readonly IExposureRepository _exposureRepository;
    private readonly ILogger<ExposureController> _logger;

    public ExposureController(IExposureRepository exposureRepository, ILogger<ExposureController> logger)
    {
        _exposureRepository = exposureRepository;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<List<Exposure>>> GetExposures()
    {
        try
        {
            var exposures = await _exposureRepository.GetAllAsync();
            return Ok(exposures);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting exposures");
            return StatusCode(500, new { error = "Internal server error" });
        }
    }

    [HttpPost]
    public async Task<IActionResult> ResetExposures()
    {
        try
        {
            var exposures = await _exposureRepository.GetAllAsync();
            foreach (var exposure in exposures)
            {
                exposure.Reset();
                await _exposureRepository.UpdateAsync(exposure);
            }

            _logger.LogInformation("All exposures reset to zero");
            return Ok(new { message = "Exposures reset successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error resetting exposures");
            return StatusCode(500, new { error = "Internal server error" });
        }
    }
}