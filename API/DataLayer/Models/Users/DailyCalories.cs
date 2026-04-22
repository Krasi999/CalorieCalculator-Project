using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataLayer.Models.Users;

[Table("DailyCalories")]
public class DailyCalories
{
    public DailyCalories() { }

    public DailyCalories(Guid userId, DateTime date, int caloriesEaten)
    {
        UserId = userId;
        Date = DateTime.SpecifyKind(date.Date, DateTimeKind.Utc);
        CaloriesEaten = caloriesEaten;
    }

    [Key]
    [Column("Id")]
    public Guid ID { get; private set; } = Guid.NewGuid();

    [Column("UserId")]
    public Guid UserId { get; private set; }

    public DateTime Date { get; private set; }

    public int CaloriesEaten { get; private set; }

    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; private set; }

    [ForeignKey(nameof(UserId))]
    public User User { get; private set; } = null!;

    public void AddCalories(int calories)
    {
        CaloriesEaten += calories;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetCalories(int calories)
    {
        CaloriesEaten = calories;
        UpdatedAt = DateTime.UtcNow;
    }
}