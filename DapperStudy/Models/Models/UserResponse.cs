namespace DapperStudy.Models.Models;

public class UserResponse
{
    public Guid Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public List<string> Roles { get; set; }
    public DateTime CreatedAt { get; set; }
}