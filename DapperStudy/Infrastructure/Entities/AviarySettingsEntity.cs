using DapperStudy.Models;

namespace DapperStudy.Infrastructure.Entities;

public record AviarySettingsEntity(
    Guid Id,
    string Type,
    int Width,
    int Height,
    int Depth)
{
    public AviarySettings MapToModel()
    {
        return new AviarySettings
        {
            Id = Id,
            Type = Type,
            Width = Width,
            Height = Height,
            Depth = Depth
        };
    }
}