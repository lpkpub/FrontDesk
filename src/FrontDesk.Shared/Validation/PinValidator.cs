namespace FrontDesk.Shared.Validation;

public static class PinValidator
{
    public static bool IsValid(string? pin) =>
       pin is not null &&
       pin.Length == 6 &&
       pin.All(char.IsDigit);
}