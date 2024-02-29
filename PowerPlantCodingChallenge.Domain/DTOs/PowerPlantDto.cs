using System.Text.Json.Serialization;
using Newtonsoft.Json;
using PowerPlantCodingChallenge.Domain.Enum;

namespace PowerPlantCodingChallenge.Domain.DTOs;

public class PowerPlantDto
{
    [JsonProperty("name")] public string Name { get; set; }

    [JsonProperty("type")]
    [System.Text.Json.Serialization.JsonConverter(typeof(JsonStringEnumConverter))]
    public PowerPlantType Type { get; set; }

    [JsonProperty("efficiency")] public decimal Efficiency { get; set; }

    [JsonProperty("pmin")] public decimal PMin { get; set; }

    [JsonProperty("pmax")] public decimal PMax { get; set; }
}