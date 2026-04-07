namespace CalorieCalculator.Service;

public static class BiometricAuthenticator
{
    public static async Task<bool> AuthenticateAsync(string reason = "Потвърди самоличността си")
    {
        try
        {
            // Plugin.Fingerprint — ще го добавим в NuGet
            var result = await Plugin.Fingerprint.CrossFingerprint.Current
                .AuthenticateAsync(new Plugin.Fingerprint.Abstractions
                    .AuthenticationRequestConfiguration(reason, reason));

            return result.Authenticated;
        }
        catch
        {
            return false;
        }
    }

    public static async Task<bool> IsAvailableAsync()
    {
        return await Plugin.Fingerprint.CrossFingerprint.Current.IsAvailableAsync();
    }
}