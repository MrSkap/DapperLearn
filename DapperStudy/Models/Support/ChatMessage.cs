namespace DapperStudy.Models.Support;

public class ChatMessage
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public required string UserId { get; set; }
    public required string UserName { get; set; }
    public required string Message { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public bool IsSupportAgent { get; set; }
    public string ChatRoom { get; set; } = "general";
}