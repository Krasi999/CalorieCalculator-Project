namespace CalorieCalculator.Service;

public static class BiometricAuthenticator
{
    public static async Task<bool> AuthenticateAsync(string reason = "Потвърди самоличността си")
    {
        try
        {
            var isAvailable = await IsAvailableAsync();
            if (!isAvailable)
                return false;

            var request = new Plugin.Fingerprint.Abstractions.AuthenticationRequestConfiguration(
                reason, reason)
            {
                AllowAlternativeAuthentication = false,
                CancelTitle = "Отказ"
            };

            var result = await Plugin.Fingerprint.CrossFingerprint.Current
                .AuthenticateAsync(request);

            return result.Authenticated;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Biometric error: {ex.Message}");
            return false;
        }
    }

    public static async Task<bool> IsAvailableAsync()
    {
        try
        {
            return await Plugin.Fingerprint.CrossFingerprint.Current.IsAvailableAsync();
        }
        catch
        {
            return false;
        }
    }
}