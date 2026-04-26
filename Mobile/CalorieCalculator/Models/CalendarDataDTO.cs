namespace CalorieCalculator.Models;

public class CalendarDataDTO
{
    public List<CalendarDayDataDto> Days { get; set; } = new();
}

public class CalendarDayDataDto
{
    public DateTime Date { get; set; }
    public int CaloriesGoal { get; set; }
    public int CaloriesEaten { get; set; }
    public bool HasProgram { get; set; }
}
