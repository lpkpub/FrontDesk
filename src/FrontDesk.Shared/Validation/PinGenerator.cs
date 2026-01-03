namespace FrontDesk.Shared.Validation;

public static class PinGenerator
{
    /// <summary>
    /// Generates a random 6-digit pin that is checked against <paramref name="isPinTakenAsync"/> for uniqueness.
    /// </summary>
    /// <param name="isPinTakenAsync">Should return true if the pin is already in use</param>
    /// <param name="maxAttempts">Max retry attempts</param>
    /// <returns>A unique 6-digit Pin</returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static async Task<string> GenerateUniqueAsync(
        Func<string, Task<bool>> isPinTakenAsync,
        int maxAttempts = 10)
    {
        for (var attempt = 0; attempt < maxAttempts; attempt++)
        {
            var pin = NextPin();
            if (!await isPinTakenAsync(pin))
                return pin;
        }

        throw new InvalidOperationException("Unable to generate a unique PIN.");
    }

    private static string NextPin() =>
       Random.Shared.Next(0, 1_000_000).ToString("D6");
}
