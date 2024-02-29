using AutoMapper;
using PowerPlantCodingChallenge.Domain.DTOs;
using PowerPlantCodingChallenge.Domain.Enum;
using PowerPlantCodingChallenge.Domain.Models;

namespace PowerPlantCodingChallenge.Services.Services;

public class PowerPlantsServices : IPowerPlantsServices
{
    private readonly IMapper _mapper;

    public PowerPlantsServices(IMapper mapper)
    {
        _mapper = mapper;
    }

    /// <summary>
    /// Calculates the production plan asynchronously based on the provided request
    /// </summary>
    /// <param name="productionPlanRequestDto"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<IEnumerable<PowerResult>> CalculateProductionPlanAsync(ProductionPlanRequestDto productionPlanRequestDto, CancellationToken cancellationToken)
    {
        // Create a list to store PowerPlant objects
        List<PowerPlant> plants = new List<PowerPlant>();

        // Map PowerPlantDto list from the request to a list of PowerPlant objects
        var powerPlants = _mapper.Map<List<PowerPlantDto>, List<PowerPlant>>(productionPlanRequestDto.PowerPlants.ToList());

        // Map FuelDto from the request to a Fuel object
        var fuels = _mapper.Map<FuelDto, Fuel>(productionPlanRequestDto.Fuels);

        // Calculate the cost for each PowerPlant based on its fuel type
        foreach (var plant in powerPlants)
        {
            plants.Add(GetCostByFuel(plant, fuels));
        }

        // Order the PowerPlants based on production cost, maximum power, and then minimum power
        powerPlants = powerPlants.OrderBy(d => d.ProdCost)
                                 .ThenByDescending(d => d.PMax)
                                 .ThenBy(d => d.PMin).ToList();

        // Calculate the production plan based on the ordered PowerPlants and the specified load
        return Task.FromResult<IEnumerable<PowerResult>>(Calculate(powerPlants, productionPlanRequestDto.Load));
    }

    /// <summary>
    /// Calculate the cost for PowerPlant based on its fuel type
    /// </summary>
    /// <param name="powerPlant"></param>
    /// <param name="fuel"></param>
    /// <returns></returns>
    private PowerPlant GetCostByFuel(PowerPlant powerPlant, Fuel fuel)
    {
        switch (powerPlant.Type)
        {
            case PowerPlantType.WindTurbine:
                powerPlant.FuelCost = 0;
                powerPlant.ProdCost = 0;
                powerPlant.PMin = powerPlant.PMin * fuel.Wind / 100;
                powerPlant.PMax = powerPlant.PMax * fuel.Wind / 100;
                break;
            case PowerPlantType.GasFired:
                powerPlant.ProdCost = fuel.Gas / powerPlant.Efficiency;
                powerPlant.FuelCost = fuel.Gas;
                //adding Co2 cost
                powerPlant.ProdCost += (powerPlant.ProdCost * fuel.Co);
                break;
            case PowerPlantType.TurboJet:
                powerPlant.ProdCost = fuel.Kerosine / powerPlant.Efficiency; ;
                powerPlant.FuelCost = fuel.Kerosine;
                //adding Co2 cost
                powerPlant.ProdCost += (powerPlant.ProdCost * fuel.Co);
                break;
            default:
                break;

        }
        return powerPlant;
    }

    /// <summary>
    /// Attempts to find a combination of PowerPlants that satisfies the required load
    /// </summary>
    /// <param name="combination"></param>
    /// <param name="requiredLoad"></param>
    /// <returns>Returns a PowerPlantCombination if successful, otherwise returns null</returns>
    private PowerPlantCombination? SolveCombination(IEnumerable<PowerPlant> combination, decimal requiredLoad)
    {
        // Order the combination of PowerPlants by production cost
        combination = combination.OrderBy(x => x.ProdCost);

        // Initialize variables to track current load, current cost, and selected power sources
        decimal currentLoad = 0;
        decimal currentCost = 0;
        List<PowerPlant> selectedSources = new();

        // Iterate through the ordered combination of PowerPlants
        for (var i = 0; i < combination.Count(); ++i)
        {
            // Get the current PowerPlant from the combination
            var source = (PowerPlant)combination.ElementAt(i);

            // Check if the current PowerPlant can contribute to the required load
            if (source.PMin < requiredLoad && source.PMax < requiredLoad - currentLoad &&
                i < combination.Count() - 1)
            {
                // If there is a next PowerPlant, calculate the provisioned load
                var nextSource = combination.ElementAt(i + 1);
                var provisionedLoad = Math.Min(requiredLoad - currentLoad - nextSource.PMin, source.PMax);

                // Update current load, cost, and mark the source as allocated
                currentLoad += provisionedLoad;
                currentCost += provisionedLoad * source.ProdCost;
                source.AllocatedProd = provisionedLoad;
                selectedSources.Add(source);
            }
            else if (currentLoad + source.PMin <= requiredLoad)
            {
                // If the current PowerPlant can satisfy the load, calculate the provisioned load
                var provisionedLoad = Math.Min(source.PMax, requiredLoad - currentLoad);

                // Update current load, cost, and mark the source as allocated
                currentLoad += provisionedLoad;
                currentCost += provisionedLoad * source.ProdCost;
                source.AllocatedProd = provisionedLoad;
                selectedSources.Add(source);
            }
        }

        // Check if the calculated load matches the required load
        return currentLoad == requiredLoad
            ? new PowerPlantCombination { Cost = currentCost, Sources = selectedSources, RequiredLoad = requiredLoad }
            : null;
    }

    /// <summary>
    /// Calculates the optimal combination of PowerPlants to meet the required load
    /// </summary>
    /// <param name="sources"></param>
    /// <param name="requiredLoad"></param>
    /// <returns>Returns a list of PowerResults representing the allocated production of each selected PowerPlant</returns>
    private List<PowerResult> Calculate(List<PowerPlant> sources, decimal requiredLoad)
    {
        // Initialize variables to track the best combination and the number of iterations
        PowerPlantCombination bestCombination = null;
        int iterations = 0;

        // Iterate through all possible combinations of PowerPlants
        foreach (var combination in GetPowerPlantCombinations(sources))
        {
            iterations++;

            // Check if the current combination is valid for the required load
            if (IsPowerPlantCombinationValid(combination, requiredLoad))
            {
                // Solve the current combination and get the result
                var currentCombination = SolveCombination(combination, requiredLoad);

                // Update the best combination if the current one is better
                if (currentCombination != null &&
                    (bestCombination is null || currentCombination.Cost < bestCombination.Cost))
                {
                    bestCombination = currentCombination;
                    bestCombination.Iterations = iterations;
                }
            }
        }

        // Update the iterations for the best combination
        if (bestCombination != null)
            bestCombination.Iterations = iterations;

        // Prepare the final result list of PowerResults
        var result = new List<PowerResult>();
        if (bestCombination != null)
        {
            // Convert the selected PowerPlants in the best combination to PowerResults
            foreach (var powerPlant in bestCombination.Sources)
            {
                result.Add(new PowerResult()
                {
                    Name = powerPlant.Name,
                    P = powerPlant.AllocatedProd,
                });
            }
        }

        // Return the list of PowerResults representing the allocated production
        return result;
    }

    /// <summary>
    /// This function checks whether a combination of power plants is valid for a given required load.
    /// It takes a collection of PowerPlant objects (powerPlantsCombination) and a required load value.
    /// </summary>
    private bool IsPowerPlantCombinationValid(IEnumerable<PowerPlant> powerPlantsCombination, decimal requiredLoad)
    {
        var sumPMin = powerPlantsCombination.Sum(x => x.PMin);
        var sumPMax = powerPlantsCombination.Sum(x => x.PMax);

        if (requiredLoad < sumPMin || requiredLoad > sumPMax)
            return false;

        return true;
    }

    /// <summary>
    /// Generates all possible combinations of PowerPlants from a given list of sources.
    /// </summary>
    /// <param name="powerPlants">The list of PowerPlants to generate combinations from.</param>
    /// <returns>
    /// An enumerable of enumerables representing all possible combinations of PowerPlants.
    /// Each inner enumerable represents a combination of PowerPlants.
    /// </returns>
    private IEnumerable<IEnumerable<PowerPlant>> GetPowerPlantCombinations(List<PowerPlant> powerPlants)
    {
        if (powerPlants is null)
            throw new ArgumentNullException(nameof(powerPlants));

        var powerPlantCount = powerPlants.Count;

        var result = Enumerable.Range(0, 1 << powerPlantCount)
            .Select(index => powerPlants.Where((_, i) => (index & (1 << i)) != 0));


        return result.OrderBy(combination => combination.Sum(powerPlant => powerPlant.ProdCost));
    }

    private record PowerPlantCombination
    {
        public int Iterations { get; set; }
        public decimal RequiredLoad { get; set; }
        public decimal Cost { get; set; }
        public List<PowerPlant> Sources { get; set; }
    }
}

