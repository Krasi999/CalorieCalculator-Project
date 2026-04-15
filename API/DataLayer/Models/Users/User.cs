using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataLayer.Models.Users;

[Table("Users")]
public class User
{
    [Key]
    public Guid ID { get; private set; } = Guid.NewGuid();

    public string Email { get; private set; } = string.Empty;

    public string PasswordHash { get; private set; } = string.Empty;

    public string Salt { get; private set; }

    public DateTime ActivationDate { get; private set; } = DateTime.UtcNow;

    public DateTime? LastPasswordLogin { get; private set; }

    public bool IsBiometricEnabled { get; private set; }

    public bool IsActive { get; private set; } = true;

    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; private set; }

    public void Update(string email, string passwordHash, string salt)
    {
        Email = email.ToLowerInvariant().Trim();
        PasswordHash = passwordHash;
        Salt = salt;
        ActivationDate = DateTime.UtcNow;
        LastPasswordLogin = DateTime.UtcNow;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
    }

    public void UpdateLastPasswordLogin()
    {
        LastPasswordLogin = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetBiometric(bool enabled)
    {
        IsBiometricEnabled = enabled;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public bool RequiresPasswordReauth()
    {
        if (LastPasswordLogin == null) return true;
        return (DateTime.UtcNow - LastPasswordLogin.Value).TotalHours >= 72;
    }
}