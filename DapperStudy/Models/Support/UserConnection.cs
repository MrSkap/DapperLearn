namespace DapperStudy.Models.Support;

public class UserConnection
{
    public required string UserId { get; set; }
    public required string UserName { get; set; }
    public required string ConnectionId { get; set; }
    public bool IsSupportAgent { get; set; }
    public DateTime ConnectedAt { get; set; } = DateTime.UtcNow;
}