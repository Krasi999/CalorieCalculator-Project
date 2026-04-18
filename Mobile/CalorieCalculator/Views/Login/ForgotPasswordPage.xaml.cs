using CalorieCalculator.ViewModels;

namespace CalorieCalculator.Views;

public partial class ForgotPasswordPage : ContentPage
{
    public ForgotPasswordPage(ForgotPasswordViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    private void OnDigitTextChanged(object? sender, TextChangedEventArgs e)
    {
        try
        {
            if (sender is not Entry currentEntry) return;

            // Намираме Entry-тата по x:Name динамично
            var entry1 = this.FindByName<Entry>("Entry1");
            var entry2 = this.FindByName<Entry>("Entry2");
            var entry3 = this.FindByName<Entry>("Entry3");
            var entry4 = this.FindByName<Entry>("Entry4");
            var entry5 = this.FindByName<Entry>("Entry5");
            var entry6 = this.FindByName<Entry>("Entry6");

            if (entry1 == null || entry2 == null || entry3 == null ||
                entry4 == null || entry5 == null || entry6 == null)
                return;

            var entries = new[] { entry1, entry2, entry3, entry4, entry5, entry6 };

            var index = Array.IndexOf(entries, currentEntry);
            if (index < 0) return;

            if (!string.IsNullOrEmpty(e.NewTextValue) && e.NewTextValue.Length == 1)
            {
                if (index < entries.Length - 1)
                {
                    entries[index + 1].Focus();
                }
                else
                {
                    currentEntry.Unfocus();
                }
            }
            else if (string.IsNullOrEmpty(e.NewTextValue) && !string.IsNullOrEmpty(e.OldTextValue))
            {
                if (index > 0)
                {
                    entries[index - 1].Focus();
                }
            }
        }
        catch
        {
            // Entry-тата все още не са заредени — игнорираме
        }
    }
}