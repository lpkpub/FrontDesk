using FrontDesk.Shared.Validation;
using Xunit;

namespace FrontDesk.UnitTests;

public class PinValidatorTests
{
    [Theory]
    [InlineData(null, false)]
    [InlineData("", false)]
    [InlineData("12345", false)]
    [InlineData("1234567", false)]
    [InlineData("12a456", false)]
    [InlineData("000000", true)]
    [InlineData("123456", true)]
    [InlineData("1@2345", false)]
    [InlineData("1 2345", false)]
    public void IsValid_ReturnsExpected(string? pin, bool expected)
    {
        Assert.Equal(expected, PinValidator.IsValid(pin));
    }
}