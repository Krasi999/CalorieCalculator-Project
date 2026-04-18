namespace CalorieCalculator
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            var window = new Window(new AppShell());

#if WINDOWS
            window.Width = 400;
            window.Height = 800;
            window.MinimumWidth = 400;
            window.MinimumHeight = 700;
#endif

            return window;
        }
    }
}