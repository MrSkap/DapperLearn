namespace DapperStudy.Models;

public class Worker
{
    public Guid? UserId { get; set; }
    public string Name { get; set; }
    public List<Aviary> Aviaries { get; set; }
    public List<Animal> Animals { get; set; }
}