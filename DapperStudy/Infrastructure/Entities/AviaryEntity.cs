using DapperStudy.Models;

namespace DapperStudy.Infrastructure.Entities;

public record AviaryEntity(
    Guid Id,
    string Name,
    Guid SettingsId)
{
    public Aviary MapToModel()
    {
        return new Aviary
        {
            Id = Id,
            Name = Name
        };
    }
}