using DapperStudy.Models.Support;
using Microsoft.AspNetCore.Mvc;

namespace DapperStudy.Api;

public class SupportController: ControllerBase
{
    private static readonly List<ChatMessage> _messageHistory = new List<ChatMessage>();

    [HttpPost("send")]
    public IActionResult SendMessage([FromBody] ChatMessage message)
    {
        _messageHistory.Add(message);
        return Ok();
    }

    [HttpGet("history")]
    public IActionResult GetMessageHistory(string room = "general")
    {
        var messages = _messageHistory
            .Where(m => m.ChatRoom == room)
            .OrderBy(m => m.Timestamp)
            .Take(50)
            .ToList();
        return Ok(messages);
    }

    [HttpGet("users")]
    public IActionResult GetConnectedUsers()
    {
        // В реальном приложении здесь была бы база данных
        return Ok(new { users = new List<object>() });
    }
}