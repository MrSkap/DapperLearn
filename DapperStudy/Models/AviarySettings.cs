namespace DapperStudy.Models;

public class AviarySettings
{
    public Guid Id { get; set; }
    public string Type { get; set; } = null!;
    public int Width { get; set; }
    public int Height { get; set; }
    public int Depth { get; set; }
}