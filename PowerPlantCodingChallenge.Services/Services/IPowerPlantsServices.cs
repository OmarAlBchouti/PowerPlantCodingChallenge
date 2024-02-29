using PowerPlantCodingChallenge.Domain.DTOs;
using PowerPlantCodingChallenge.Domain.Models;

namespace PowerPlantCodingChallenge.Services.Services;

public interface IPowerPlantsServices
{
    Task<IEnumerable<PowerResult>> CalculateProductionPlanAsync(ProductionPlanRequestDto productionPlanRequestDto,
        CancellationToken cancellationToken);
}