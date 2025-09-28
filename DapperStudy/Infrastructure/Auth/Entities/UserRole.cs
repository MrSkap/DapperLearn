using System.ComponentModel.DataAnnotations.Schema;

namespace DapperStudy.Infrastructure.Auth.Entities;

public class UserRole
{
    public Guid UserId { get; set; }
    public Guid RoleId { get; set; }

    [ForeignKey("UserId")] public virtual UserEntity UserEntity { get; set; }

    [ForeignKey("RoleId")] public virtual Role Role { get; set; }

    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
}