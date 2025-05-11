namespace DapperStudy.Models;

public class Animal
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public int Age { get; set; }
    public double Weight { get; set; }
    public Guid AviaryId { get; set; }
}