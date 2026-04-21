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
        }
    }

    private static void OnCurrentPageChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var navBar = (NavBar)bindable;
        navBar.OnPropertyChanged(nameof(ProfileColor));
        navBar.OnPropertyChanged(nameof(HomeColor));
        navBar.OnPropertyChanged(nameof(ProgressColor));
    }
}