using CalorieCalculator.Views;
using CalorieCalculator.Views.MainView.Food;

namespace CalorieCalculator;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute("Login", typeof(LoginPage));
        Routing.RegisterRoute("Register", typeof(RegisterPage));
        Routing.RegisterRoute("ForgotPassword", typeof(ForgotPasswordPage));
        Routing.RegisterRoute("ProfileSetup", typeof(ProfileSetupPage));

        Routing.RegisterRoute("food/categories", typeof(FoodCategoryPage));
        Routing.RegisterRoute("food/products", typeof(FoodProductPage));
        Routing.RegisterRoute("food/details", typeof(FoodDetailsPage));
        Routing.RegisterRoute("food/create", typeof(CreateProductPage));
    }
}