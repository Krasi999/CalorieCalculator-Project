using CommunityToolkit.Mvvm.ComponentModel;

namespace CalorieCalculator.ViewModels;

public partial class CalendarDayItem : ObservableObject
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
            if (SetProperty(ref _isSelected, value))
            {
                OnPropertyChanged(nameof(TextColor));
                OnPropertyChanged(nameof(DayBackgroundColor));
                OnPropertyChanged(nameof(SelectionRingColor));
                OnPropertyChanged(nameof(SelectionRingThickness));
            }
        }
    }

    private int _caloriesEaten;
    public int CaloriesEaten
    {
        get => _caloriesEaten;
        set
        {
            if (SetProperty(ref _caloriesEaten, value))
            {
                OnPropertyChanged(nameof(HasCalories));
                OnPropertyChanged(nameof(ProgressForArc));
                OnPropertyChanged(nameof(CalorieRingColor));
            }
        }
    }

    private double _caloriesProgress;
    public double CaloriesProgress
    {
        get => _caloriesProgress;
        set
        {
            if (SetProperty(ref _caloriesProgress, value))
            {
                OnPropertyChanged(nameof(HasCalories));
                OnPropertyChanged(nameof(CalorieRingColor));
                OnPropertyChanged(nameof(ProgressForArc)); // уведомява конвертора
            }
        }
    }

    private bool _isOverTarget;
    public bool IsOverTarget
    {
        get => _isOverTarget;
        set
        {
            if (SetProperty(ref _isOverTarget, value))
            {
                OnPropertyChanged(nameof(CalorieRingColor));
                OnPropertyChanged(nameof(ProgressForArc));
            }
        }
    }

    public bool HasCalories => CaloriesProgress > 0;

    // Това свойство подаваме на конвертора – при надвишаване връща 1.0
    public double ProgressForArc => IsOverTarget ? 1.0 : CaloriesProgress;

    public Color DayBackgroundColor => IsToday
        ? Color.FromArgb("#3B82F6")
        : Colors.Transparent;

    public Color TextColor => IsToday
        ? Colors.White
        : IsCurrentMonth
            ? Color.FromArgb("#1E293B")
            : Color.FromArgb("#CBD5E1");

    public Color CalorieRingColor => IsOverTarget
        ? Color.FromArgb("#EF4444")
        : Color.FromArgb("#22C55E");

    public Color SelectionRingColor => IsSelected
        ? Color.FromArgb("#3B82F6")
        : Colors.Transparent;

    public double SelectionRingThickness => IsSelected ? 2 : 0;
}
