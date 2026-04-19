using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataLayer.Models;

[Table("PasswordResetCodes")]
public class PasswordResetCode
{
    public PasswordResetCode() { }

    public PasswordResetCode(Guid userId, string code, DateTime expiresAt)
    {
        UserId = userId;
        Code = code;
        ExpiresAt = DateTime.SpecifyKind(expiresAt, DateTimeKind.Utc);
    }

    [Key]
    [Column("Id")]
    public Guid ID { get; private set; } = Guid.NewGuid();

    [Column("UserId")]
    public Guid UserId { get; private set; }

    public string Code { get; private set; } = string.Empty;

    public DateTime ExpiresAt { get; private set; }

    public bool IsUsed { get; private set; }

    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    [ForeignKey(nameof(UserId))]
    public User User { get; private set; } = null!;

    public void MarkAsUsed()
    {
        IsUsed = true;
    }

    public bool IsValid()
    {
        return !IsUsed && DateTime.UtcNow < ExpiresAt;
    }
}