using CalorieCalculator.Models;
using CalorieCalculator.Service;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Text.Json;

namespace CalorieCalculator.ViewModels;

public partial class CalendarViewModel : ObservableObject
{
    private readonly ApiService _api;

    [ObservableProperty] private string monthYearText = string.Empty;
    [ObservableProperty] private string selectedDayInfo = string.Empty;
    [ObservableProperty] private bool isDayInfoVisible;

    private DateTime _currentMonth;
    private DateTime _today;
    private decimal _targetCalories = 2000;

    public ObservableCollection<CalendarDayItem> Days { get; } = new();

    public CalendarViewModel(ApiService api)
    {
        _api = api;
        _today = DateTime.Now.Date;
        _currentMonth = new DateTime(_today.Year, _today.Month, 1);
        BuildCalendar();
    }

    [RelayCommand]
    private void PreviousMonth()
    {
        _currentMonth = _currentMonth.AddMonths(-1);
        BuildCalendar();
    }

    [RelayCommand]
    private void NextMonth()
    {
        _currentMonth = _currentMonth.AddMonths(1);
        BuildCalendar();
    }

    [RelayCommand]
    private void SelectDay(CalendarDayItem day)
    {
        if (day == null || !day.IsCurrentMonth) return;

        foreach (var d in Days)
        {
            d.IsSelected = d.Date == day.Date && d.Date != _today;
        }

        if (day.Date > _today)
        {
            SelectedDayInfo = $"На {day.Date:dd.MM.yyyy} все още няма записани данни!";
        }
        else if (day.CaloriesEaten > 0)
        {
            var goal = day.CaloriesProgress > 0
                ? (int)Math.Round(day.CaloriesEaten / day.CaloriesProgress)
                : (int)_targetCalories;

            var status = day.IsOverTarget
                ? "Внимавай, превишил си дневния си лимит!"
                : "Поздравления, спазил си дневния си лимит!";
            SelectedDayInfo = $"На {day.Date:dd.MM.yyyy} си изял {day.CaloriesEaten} от {goal} калории! {status}";
        }
        else
        {
            SelectedDayInfo = $"На {day.Date:dd.MM.yyyy} няма записани калории!";
        }

        IsDayInfoVisible = true;
    }

    private void BuildCalendar()
    {
        var bgCulture = new System.Globalization.CultureInfo("bg-BG");
        var monthName = _currentMonth.ToString("MMMM yyyy", bgCulture);
        MonthYearText = char.ToUpper(monthName[0]) + monthName[1..];

        Days.Clear();

        var firstDay = _currentMonth;
        var firstDayOfWeek = (int)firstDay.DayOfWeek;
        var mondayOffset = firstDayOfWeek == 0 ? 6 : firstDayOfWeek - 1;

        var prevMonth = _currentMonth.AddMonths(-1);
        var daysInPrevMonth = DateTime.DaysInMonth(prevMonth.Year, prevMonth.Month);
        for (int i = mondayOffset - 1; i >= 0; i--)
        {
            var date = new DateTime(prevMonth.Year, prevMonth.Month, daysInPrevMonth - i);
            Days.Add(CreateDayItem(date, false));
        }

        var daysInMonth = DateTime.DaysInMonth(_currentMonth.Year, _currentMonth.Month);
        for (int d = 1; d <= daysInMonth; d++)
        {
            var date = new DateTime(_currentMonth.Year, _currentMonth.Month, d);
            Days.Add(CreateDayItem(date, true));
        }

        var remaining = 42 - Days.Count;
        var nextMonth = _currentMonth.AddMonths(1);
        for (int i = 1; i <= remaining; i++)
        {
            var date = new DateTime(nextMonth.Year, nextMonth.Month, i);
            Days.Add(CreateDayItem(date, false));
        }

        IsDayInfoVisible = false;

        _ = LoadMonthCaloriesAsync();
    }

    private CalendarDayItem CreateDayItem(DateTime date, bool isCurrentMonth)
    {
        return new CalendarDayItem
        {
            Date = date,
            DayNumber = date.Day.ToString(),
            IsCurrentMonth = isCurrentMonth,
            IsToday = date == _today,
            IsSelected = false,
            CaloriesEaten = 0,
            CaloriesProgress = 0,
            IsOverTarget = false
        };
    }

    private async Task LoadMonthCaloriesAsync()
    {
        try
        {
            var userId = Preferences.Get("user_id", string.Empty);
            if (string.IsNullOrEmpty(userId)) return;

            var response = await _api.GetAsyncT<CalendarDataDTO>(
                $"api/calendar/{userId}?year={_currentMonth.Year}&month={_currentMonth.Month}");

            if (response == null) return;

            foreach (var apiDay in response.Days)
            {
                var day = Days.FirstOrDefault(d => d.Date.Date == apiDay.Date.Date);
                if (day != null)
                {
                    _targetCalories = apiDay.CaloriesGoal > 0 ? apiDay.CaloriesGoal : _targetCalories;
                    day.CaloriesEaten = apiDay.CaloriesEaten;
                    day.CaloriesProgress = apiDay.CaloriesGoal > 0? Math.Min(apiDay.CaloriesEaten / (double)apiDay.CaloriesGoal, 1.0): 0;
                    day.IsOverTarget = apiDay.CaloriesGoal > 0 && apiDay.CaloriesEaten > apiDay.CaloriesGoal;
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Calendar load error: {ex.Message}");
        }
    }
}