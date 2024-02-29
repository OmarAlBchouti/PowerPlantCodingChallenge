using AutoMapper;
using PowerPlantCodingChallenge.Domain.DTOs;
using PowerPlantCodingChallenge.Domain.Models;

namespace PowerPlantCodingChallenge.Domain.AutoMapper;

public static class AutoMapperClasses
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<PowerPlantDto, PowerPlant>().ReverseMap();
            CreateMap<FuelDto, Fuel>().ReverseMap();
        }
    }
}