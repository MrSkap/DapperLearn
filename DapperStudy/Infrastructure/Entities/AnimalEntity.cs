using DapperStudy.Models;

namespace DapperStudy.Infrastructure.Entities;

public record AnimalEntity(
    Guid Id,
    string Name,
    int Age,
    double Weight,
    Guid AviaryId)
{
    public Animal MapToModel()
    {
        return new Animal
        {
            Id = Id,
            Age = Age,
            AviaryId = AviaryId,
            Name = Name,
            Weight = Weight
        };
    }
}