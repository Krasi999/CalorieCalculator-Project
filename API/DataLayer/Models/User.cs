using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataLayer.Models;

[Table("Users")]
public class User
{
    [Key]
    public Guid Id { get; private set; } = Guid.NewGuid();

    public string Email { get; private set; } = string.Empty;

    public string PasswordHash { get; private set; } = string.Empty;

    public DateTime ActivationDate { get; private set; } = DateTime.UtcNow;

    public DateTime? LastPasswordLogin { get; private set; }

    public bool IsBiometricEnabled { get; private set; }

    public bool IsActive { get; private set; } = true;

    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; private set; }

    // ──────────────────────────────────────────────
    // Методи за мутация (същия pattern като FoodProduct.Update())
    // ──────────────────────────────────────────────

    public void Create(string email, string passwordHash)
    {
        Email = email.ToLowerInvariant().Trim();
        PasswordHash = passwordHash;
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

    // 72-часова проверка — изчислява се в модела, не в бизнес логиката
    public bool RequiresPasswordReauth()
    {
        if (LastPasswordLogin == null) return true;
        return (DateTime.UtcNow - LastPasswordLogin.Value).TotalHours >= 72;
    }
}