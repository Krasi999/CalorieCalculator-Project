namespace CalorieCalculator.Models;

public class DailyProgramDTO
{
    public int ProgramID { get; set; }

    public int CaloriesPerDay { get; set; }

    public int CarbsPerDay { get; set; }

    public int ProteinPerDay { get; set; }

    public int FatsPerDay { get; set; }

    public List<MealDTO> Meals { get; set; } = new();
}
