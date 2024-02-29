using Newtonsoft.Json;

namespace PowerPlantCodingChallenge.Domain.DTOs;

public class ProductionPlanRequestDto
{
    [JsonProperty("load")] public decimal Load { get; set; }

    [JsonProperty("fuels")] public FuelDto Fuels { get; set; }

    [JsonProperty("powerplants")] public IEnumerable<PowerPlantDto> PowerPlants { get; set; }
}