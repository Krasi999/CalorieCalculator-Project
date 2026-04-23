using System.Windows.Input;

namespace CalorieCalculator.Views.MainView.Components;

public partial class NavBar : ContentView
{
    public static readonly BindableProperty CurrentPageProperty =
        BindableProperty.Create(nameof(CurrentPage), typeof(string), typeof(NavBar),
            "home", propertyChanged: OnCurrentPageChanged);

    public string CurrentPage
    {
        get => (string)GetValue(CurrentPageProperty);
        set => SetValue(CurrentPageProperty, value);
    }

    private Color ActiveColor => Color.FromArgb("#4DB6AC");
    private Color InactiveColor => Color.FromArgb("#BDBDBD");

    public Color ProfileColor => CurrentPage == "profile" ? ActiveColor : InactiveColor;
    public Color HomeColor => CurrentPage == "home" ? ActiveColor : InactiveColor;
    public Color ProgressColor => CurrentPage == "progress" ? ActiveColor : InactiveColor;

    public Color CalendarColor => CurrentPage == "calendar" ? ActiveColor : InactiveColor;

    public string ProfileIcon => CurrentPage == "profile" ? "person_teal.png" : "person_gray.png";
    public string HomeIcon => CurrentPage == "home" ? "home_teal.png" : "home_gray.png";
    public string ProgressIcon => CurrentPage == "progress" ? "chart_teal.png" : "chart_gray.png";
    public string CalendarIcon => CurrentPage == "calendar" ? "calendar_teal.png" : "calendar_gray.png";

    public ICommand NavigateCommand { get; }

    public NavBar()
    {
        NavigateCommand = new Command<string>(NavigateToPage);
        InitializeComponent();
        BindingContext = this;
    }

    private async void NavigateToPage(string page)
    {
        if (page == CurrentPage) return;

        switch (page)
        {
            case "profile":
                await Shell.Current.GoToAsync("//ProfilePage");
                break;
            case "home":
                await Shell.Current.GoToAsync("//MainPage");
                break;
            case "progress":
                await Shell.Current.GoToAsync("//ProgressPage");
                break;
            case "calendar":
                await Shell.Current.GoToAsync("//CalendarPage");
                break;
        }
    }

    private static void OnCurrentPageChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var navBar = (NavBar)bindable;
        navBar.OnPropertyChanged(nameof(ProfileColor));
        navBar.OnPropertyChanged(nameof(HomeColor));
        navBar.OnPropertyChanged(nameof(ProgressColor));
        navBar.OnPropertyChanged(nameof(CalendarColor));
        navBar.OnPropertyChanged(nameof(ProfileIcon));
        navBar.OnPropertyChanged(nameof(HomeIcon));
        navBar.OnPropertyChanged(nameof(ProgressIcon));
        navBar.OnPropertyChanged(nameof(CalendarIcon));
    }
}