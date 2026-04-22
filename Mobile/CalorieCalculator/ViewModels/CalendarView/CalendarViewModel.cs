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
            selectedDayInfo = $"На {day.Date:dd.MM.yyyy} все още няма записани данни.";
        }
        else if (day.CaloriesEaten > 0)
        {
            var status = day.IsOverTarget ? "⚠️ Надвишен лимит!" : "✅ В рамките на целта";
            selectedDayInfo = $"На {day.Date:dd.MM.yyyy} си изял {day.CaloriesEaten} от {(int)_targetCalories} калории. {status}";
        }
        else
        {
            selectedDayInfo = $"На {day.Date:dd.MM.yyyy} няма записани калории.";
        }

        OnPropertyChanged(nameof(SelectedDayInfo));
        isDayInfoVisible = true;
        OnPropertyChanged(nameof(IsDayInfoVisible));
    }

    private void BuildCalendar()
    {
        var bgCulture = new System.Globalization.CultureInfo("bg-BG");
        var monthName = _currentMonth.ToString("MMMM yyyy", bgCulture);
        monthYearText = char.ToUpper(monthName[0]) + monthName[1..];
        OnPropertyChanged(nameof(MonthYearText));

        Days.Clear();

        var firstDay = _currentMonth;
        var firstDayOfWeek = (int)firstDay.DayOfWeek;
        var mondayOffset = firstDayOfWeek == 0 ? 6 : firstDayOfWeek - 1;

        // Дни от предишния месец
        var prevMonth = _currentMonth.AddMonths(-1);
        var daysInPrevMonth = DateTime.DaysInMonth(prevMonth.Year, prevMonth.Month);
        for (int i = mondayOffset - 1; i >= 0; i--)
        {
            var date = new DateTime(prevMonth.Year, prevMonth.Month, daysInPrevMonth - i);
            Days.Add(CreateDayItem(date, false));
        }

        // Дни от текущия месец
        var daysInMonth = DateTime.DaysInMonth(_currentMonth.Year, _currentMonth.Month);
        for (int d = 1; d <= daysInMonth; d++)
        {
            var date = new DateTime(_currentMonth.Year, _currentMonth.Month, d);
            Days.Add(CreateDayItem(date, true));
        }

        // Попълваме до 42 клетки
        var remaining = 42 - Days.Count;
        var nextMonth = _currentMonth.AddMonths(1);
        for (int i = 1; i <= remaining; i++)
        {
            var date = new DateTime(nextMonth.Year, nextMonth.Month, i);
            Days.Add(CreateDayItem(date, false));
        }

        isDayInfoVisible = false;
        OnPropertyChanged(nameof(IsDayInfoVisible));

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

            // Зареждаме целевите калории от API
            var response = await _api.GetSingleAsync<JsonElement>(
                $"api/UserDetails/{userId}/monthly-calories/{_currentMonth.Year}/{_currentMonth.Month}");

            if (response.ValueKind != JsonValueKind.Undefined)
            {
                _targetCalories = response.GetProperty("targetCalories").GetDecimal();

                // Ако има реални дневни калории от базата
                if (response.TryGetProperty("dailyCalories", out var dailyProp))
                {
                    foreach (var prop in dailyProp.EnumerateObject())
                    {
                        if (DateTime.TryParse(prop.Name, out var date))
                        {
                            var eaten = prop.Value.GetInt32();
                            var day = Days.FirstOrDefault(d => d.Date == date.Date);
                            if (day != null)
                            {
                                day.CaloriesEaten = eaten;
                                day.CaloriesProgress = Math.Min(eaten / (double)_targetCalories, 1.0);
                                day.IsOverTarget = eaten > (int)_targetCalories;
                            }
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Calendar load error: {ex.Message}");
        }
    }
}

public class CalendarDayItem : ObservableObject
{
    public DateTime Date { get; set; }
    public string DayNumber { get; set; } = string.Empty;
    public bool IsCurrentMonth { get; set; }

    private bool _isToday;
    public bool IsToday
    {
        get => _isToday;
        set => SetProperty(ref _isToday, value);
    }

    private bool _isSelected;
    public bool IsSelected
    {
        get => _isSelected;
        set
        {
            SetProperty(ref _isSelected, value);
            OnPropertyChanged(nameof(TextColor));
            OnPropertyChanged(nameof(DayBackgroundColor));
            OnPropertyChanged(nameof(BorderColor));
            OnPropertyChanged(nameof(BorderThickness));
        }
    }

    public int CaloriesEaten { get; set; }

    private double _caloriesProgress;
    public double CaloriesProgress
    {
        get => _caloriesProgress;
        set
        {
            SetProperty(ref _caloriesProgress, value);
            OnPropertyChanged(nameof(HasCalories));
            OnPropertyChanged(nameof(RingColor));
        }
    }

    private bool _isOverTarget;
    public bool IsOverTarget
    {
        get => _isOverTarget;
        set
        {
            SetProperty(ref _isOverTarget, value);
            OnPropertyChanged(nameof(RingColor));
        }
    }

    public bool HasCalories => CaloriesProgress > 0;

    // Цвят на кръгчето
    public Color RingColor => IsOverTarget ? Color.FromArgb("#EF4444") : Color.FromArgb("#22C55E");

    // Цвят на текста
    public Color TextColor => IsToday ? Colors.White
        : IsCurrentMonth ? Color.FromArgb("#1E293B")
        : Color.FromArgb("#CBD5E1");

    // Фон на деня
    public Color DayBackgroundColor => IsToday ? Color.FromArgb("#22C55E") : Colors.Transparent;

    // Рамка при селекция (зелен контур, без запълване)
    public Color BorderColor => IsSelected ? Color.FromArgb("#22C55E") : Colors.Transparent;
    public double BorderThickness => IsSelected ? 2 : 0;
}