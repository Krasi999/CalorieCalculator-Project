public class SaveDailyCaloriesRequest
{
    public Guid UserId { get; set; }
    public DateTime Date { get; set; }
    public int CaloriesEaten { get; set; }
}

public class AddDailyCaloriesRequest
{
    public Guid UserId { get; set; }
    public int Calories { get; set; }
}