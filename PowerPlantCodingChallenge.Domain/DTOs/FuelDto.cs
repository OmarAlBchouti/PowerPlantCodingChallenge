using System.Text.Json.Serialization;

namespace PowerPlantCodingChallenge.Domain.DTOs;

public class FuelDto
{
    [JsonPropertyName("gas(euro/MWh)")] public double Gas { get; set; }

    [JsonPropertyName("kerosine(euro/MWh)")]
    public double Kerosine { get; set; }

    [JsonPropertyName("co2(euro/ton)")] public double Co { get; set; }

    [JsonPropertyName("wind(%)")] public double Wind { get; set; }
}