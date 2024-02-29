using PowerPlantCodingChallenge.Domain.Enum;

namespace PowerPlantCodingChallenge.Domain.Models;

public class PowerPlant
{
    public string Name { get; set; }

    public PowerPlantType Type { get; set; }

    public decimal Efficiency { get; set; }

    public decimal PMin { get; set; }

    public decimal PMax { get; set; }

    public decimal FuelCost { get; set; }

    public decimal ProdCost { get; set; }

    public decimal AllocatedProd { get; set; }
}