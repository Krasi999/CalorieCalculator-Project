using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DataLayer.Enums;

namespace DataLayer.Models;

[Table("UserDetails")]
public class UserDetails
{
    public UserDetails() { }

    public UserDetails(Guid userID)
    {
        UserID = userID;
    }

    [Key]
    [Column("Id")]
    public Guid ID { get; private set; } = Guid.NewGuid();

    [Column("UserId")]
    public Guid UserID { get; private set; }

    public string Nickname { get; private set; } = string.Empty;

    public Gender Gender { get; private set; }

    public DateTime DateOfBirth { get; private set; }

    public decimal? HeightCm { get; private set; }

    public decimal? HeightFt { get; private set; }

    public decimal? WeightKg { get; private set; }

    public decimal? WeightLbs { get; private set; }
    
    public GoalType CurrentGoal { get; private set; }

    public decimal? TargetWeightKg { get; private set; }

    public decimal? TargetWeightLbs { get; private set; }

    public ActivityLevel ActivityLevel { get; private set; }

    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; private set; }

    [ForeignKey(nameof(UserID))]
    public User User { get; private set; } = null!;

    public void Update(
    string nickname,
    Gender gender,
    DateTime dateOfBirth,
    decimal? heightCm,
    decimal? heightFt,
    decimal? weightKg,
    decimal? weightLbs,
    ActivityLevel activityLevel,
    GoalType currentGoal,
    decimal? targetWeightKg,
    decimal? targetWeightLbs)
    {
        Nickname = nickname.Trim();
        Gender = gender;
        DateOfBirth = dateOfBirth;
        ActivityLevel = activityLevel;
        CurrentGoal = currentGoal;
        UpdatedAt = DateTime.UtcNow;

        if (heightCm.HasValue)
        {
            HeightCm = heightCm;
            HeightFt = Math.Round(heightCm.Value / 30.48m, 2);
        }
        else if (heightFt.HasValue)
        {
            HeightFt = heightFt;
            HeightCm = Math.Round(heightFt.Value * 30.48m, 2);
        }

        if (weightKg.HasValue)
        {
            WeightKg = weightKg;
            WeightLbs = Math.Round(weightKg.Value * 2.20462m, 2);
        }
        else if (weightLbs.HasValue)
        {
            WeightLbs = weightLbs;
            WeightKg = Math.Round(weightLbs.Value / 2.20462m, 2);
        }

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

    public int GetAge() =>
        DateTime.UtcNow.Year - DateOfBirth.Year -
        (DateTime.UtcNow.DayOfYear < DateOfBirth.DayOfYear ? 1 : 0);

    public void UpdateNickname(string newNickname)
    {
        Nickname = newNickname.Trim();
        UpdatedAt = DateTime.UtcNow;
    }
}

