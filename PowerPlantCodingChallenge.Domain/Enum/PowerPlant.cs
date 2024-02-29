using System.Runtime.Serialization;

namespace PowerPlantCodingChallenge.Domain.Enum;

public enum PowerPlantType
{
    [EnumMember(Value = "windturbine")] WindTurbine = 0,

    [EnumMember(Value = "gasfired")] GasFired = 1,

    [EnumMember(Value = "turbojet")] TurboJet = 2
}