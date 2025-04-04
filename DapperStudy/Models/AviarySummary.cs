namespace DapperStudy.Models;

public record AviarySummary
{
    public Guid Id { get; set; }
    public string? Name { get; init; }
    public int? AnimalsCount { get; set; }
    public string? Type { get; init; }
    public int Width { get; init; }
    public int Height { get; init; }
    public int Depth { get; init; }
}