using CalorieCalculator.Views;

namespace CalorieCalculator;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        // Регистриране на допълнителни маршрути за навигация
        Routing.RegisterRoute("Login", typeof(LoginPage));
        Routing.RegisterRoute("Register", typeof(RegisterPage));
        Routing.RegisterRoute("ForgotPassword", typeof(ForgotPasswordPage));
        Routing.RegisterRoute("ProfileSetup", typeof(ProfileSetupPage));
        Routing.RegisterRoute("MainPage", typeof(MainPage));
        Routing.RegisterRoute("ProfilePage", typeof(ProfilePage));
        Routing.RegisterRoute("ProgressPage", typeof(ProgressPage));
    }
}