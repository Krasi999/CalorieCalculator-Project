using MediatR;

namespace Services.Queries;

public class CalendarDataQuery : IRequest<CalendarDataResponse>
{
    public Guid UserID { get; set; }

    public int Year { get; set; }

    public int Month { get; set; }
}

public class CalendarDataResponse
{
    public List<CalendarDayResponse> Days { get; set; } = new();
}

public class CalendarDayResponse
{
    public DateTime Date { get; set; }

    public int CaloriesGoal { get; set; }

    public int CaloriesEaten { get; set; }

    public bool HasProgram { get; set; }
}
