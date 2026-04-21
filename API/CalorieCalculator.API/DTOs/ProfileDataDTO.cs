public class ProfileDataRequest
{
    public Guid? UserID { get; set; }

    public string Nickname { get; set; }

    public int? Gender { get; set; }

    public DateTime? DateOfBirth { get; set; }

    public decimal? HeightCm { get; set; }

    public decimal? HeightFt { get; set; }

    public decimal? WeightKg { get; set; }

    public decimal? WeightLbs { get; set; }

    public int? ActivityLevel { get; set; }

    public int? CurrentGoal {  get; set; }

    public decimal? TargetWeightKg { get; set; }

    public decimal? TargetWeightLbs { get; set; }
}

public class ProfileDataResponse
{
    public bool Success { get; set; }

    public Guid? UserID { get; set; }
}

public class UpdateNicknameRequest
{
    public Guid UserID { get; set; }
    public string Nickname { get; set; }
}