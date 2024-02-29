using AutoMapper;
using Newtonsoft.Json;
using PowerPlantCodingChallenge.Domain.DTOs;
using PowerPlantCodingChallenge.Services.Services;
using static PowerPlantCodingChallenge.Domain.AutoMapper.AutoMapperClasses;

namespace PowerPlantCodingChallenge.Tests;

public class PowerPlantsServicesTest
{
    private readonly IMapper _mapper;

    public PowerPlantsServicesTest()
    {
        _mapper = new MapperConfiguration(cfg => { cfg.AddProfile(new AutoMapperProfile()); }).CreateMapper();
    }

    [Theory]
    [InlineData("payload/payload1.json")]
    [InlineData("payload/payload2.json")]
    [InlineData("payload/payload3.json")]
    public async Task TestMeritOrderCalculator(string payloadFile)
    {
        // Setup
        var jsonContent = await File.ReadAllTextAsync(payloadFile);
        var payload = JsonConvert.DeserializeObject<ProductionPlanRequestDto>(jsonContent);
        var powerPlantsServices = new PowerPlantsServices(_mapper);

        // Act
        var calculated = await powerPlantsServices.CalculateProductionPlanAsync(payload, CancellationToken.None);

        // Assert
        Assert.True(calculated.Sum(x => x.P).Equals(payload.Load));
        foreach (var plan in calculated)
        {
            var _plan = payload.PowerPlants.FirstOrDefault(x => x.Name == plan.Name);
            Assert.True(plan.P <= _plan.PMax);
            Assert.True(plan.P >= _plan.PMin);
        }
    }
}