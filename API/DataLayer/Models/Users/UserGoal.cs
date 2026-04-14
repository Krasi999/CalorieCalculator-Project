using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DataLayer.Enums;

namespace DataLayer.Models.Users;

[Table("UserGoals")]
public class UserGoal
{
    [Key]
    public Guid Id { get; private set; } = Guid.NewGuid();

    public Guid UserId { get; private set; }

    public GoalType GoalType { get; private set; }

    public decimal? TargetWeightKg { get; private set; }
    public decimal? TargetWeightLbs { get; private set; }

    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }

    public bool IsActive { get; private set; }

    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    [ForeignKey(nameof(UserId))]
    public User User { get; private set; } = null!;

    // ──────────────────────────────────────────────
    // Методи за мутация
    // ──────────────────────────────────────────────

    public void Create(
        Guid userId,
        GoalType goalType,
        decimal? targetWeightKg,
        decimal? targetWeightLbs,
        DateTime startDate,
        DateTime endDate)
    {
        if (endDate <= startDate)
            throw new ArgumentException(
                "Крайната дата трябва да е след началната.");

        UserId = userId;
        GoalType = goalType;
        StartDate = startDate;
        EndDate = endDate;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;

        if (targetWeightKg.HasValue)
        {
            TargetWeightKg = targetWeightKg;
            TargetWeightLbs = Math.Round(targetWeightKg.Value * 2.20462m, 2);
        }
        else if (targetWeightLbs.HasValue)
        {
            TargetWeightLbs = targetWeightLbs;
            TargetWeightKg = Math.Round(targetWeightLbs.Value / 2.20462m, 2);
        }
    }

    public void Deactivate() => IsActive = false;

    public void Update(
        GoalType goalType,
        decimal? targetWeightKg,
        decimal? targetWeightLbs,
        DateTime startDate,
        DateTime endDate)
    {
        if (endDate <= startDate)
            throw new ArgumentException(
                "Крайната дата трябва да е след началната.");

        GoalType = goalType;
        StartDate = startDate;
        EndDate = endDate;

        if (targetWeightKg.HasValue)
        {
            TargetWeightKg = targetWeightKg;
            TargetWeightLbs = Math.Round(targetWeightKg.Value * 2.20462m, 2);
        }
        else if (targetWeightLbs.HasValue)
        {
            TargetWeightLbs = targetWeightLbs;
            TargetWeightKg = Math.Round(targetWeightLbs.Value / 2.20462m, 2);
        }
    }

    // Препоръчителен калориен прием спрямо цел
    public decimal? GetCalorieTarget(decimal? tdee)
    {
        if (!tdee.HasValue) return null;

        return GoalType switch
        {
            GoalType.WeightLoss => tdee.Value - 500,
            GoalType.Maintenance => tdee.Value,
            GoalType.WeightGain => tdee.Value + 300,
            GoalType.MuscleGain => tdee.Value + 200,
            _ => tdee.Value
        };
    }
}