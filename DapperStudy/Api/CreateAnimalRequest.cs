namespace DapperStudy.Api;

public record CreateAnimalRequest
{
    public required string Name { get; init; }
    public int Age { get; init; }
    public int Weight { get; init; }
}