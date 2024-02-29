using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PowerPlantCodingChallenge.Domain.DTOs;
using PowerPlantCodingChallenge.Services.Services;

namespace PowerPlantCodingChallengeAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class PowerPlanController : ControllerBase
{
    private readonly IPowerPlantsServices _powerPlantsServices;

    public PowerPlanController(IPowerPlantsServices powerPlantsServices)
    {
        _powerPlantsServices = powerPlantsServices ?? throw new ArgumentNullException(nameof(powerPlantsServices));
    }

    /// <summary>
    ///     Calculate how much power each of a multitude of different powerplants need to produce.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> PostAsync([FromBody] ProductionPlanRequestDto request,
        CancellationToken cancellationToken)
    {
        var result = await _powerPlantsServices.CalculateProductionPlanAsync(request, cancellationToken);
        return Ok(result);
    }
}