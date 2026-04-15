using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DataLayer.Enums;

namespace DataLayer.Models.Users;

[Table("UserDetails")]
public class UserDetails
{
    public UserDetails() { }

    public UserDetails(Guid userID)
    {
        UserID = userID;
    }

    [Key]
    public Guid ID { get; private set; } = Guid.NewGuid();

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

        // Ръст
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

        // Тегло
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

        // Желано тегло
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

    // Изчислява възрастта за BMR формулата
    public int GetAge() =>
        DateTime.UtcNow.Year - DateOfBirth.Year -
        (DateTime.UtcNow.DayOfYear < DateOfBirth.DayOfYear ? 1 : 0);

    // BMR по Mifflin-St Jeor
    public decimal? CalculateBMR()
    {
        if (!WeightKg.HasValue || !HeightCm.HasValue) return null;

        return Gender switch
        {
            Gender.Male => 10 * WeightKg.Value + 6.25m * HeightCm.Value - 5 * GetAge() + 5,
            Gender.Female => 10 * WeightKg.Value + 6.25m * HeightCm.Value - 5 * GetAge() - 161,
            _ => null
        };
    }

    // TDEE = BMR × activity multiplier
    public decimal? CalculateTDEE()
    {
        var bmr = CalculateBMR();
        if (!bmr.HasValue) return null;

        var multiplier = ActivityLevel switch
        {
            ActivityLevel.Sedentary => 1.2m,
            ActivityLevel.LightlyActive => 1.375m,
            ActivityLevel.ModerateActive => 1.55m,
            ActivityLevel.VeryActive => 1.725m,
            ActivityLevel.ExtraActive => 1.9m,
            _ => 1.2m
        };

        return Math.Round(bmr.Value * multiplier, 2);
    }
}

