namespace DapperStudy.Models;

public class Aviary
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public List<Animal>? Animals { get; set; }
    public AviarySettings Settings { get; set; } = null!;
}