using FrontDesk.Shared.Validation;
using Xunit;

namespace FrontDesk.UnitTests;

public class PinGeneratorTests
{
    [Fact]
    public async Task GenerateUniqueAsync_RetriesUntilAvailable()
    {
        int calls = 0;
        Task<bool> mockIsPinTaken(string _)
        {
            calls++;
            return Task.FromResult(calls < 3); // first 2 are "taken", 3rd is free
        }

        var pin = await PinGenerator.GenerateUniqueAsync(mockIsPinTaken);

        Assert.False(string.IsNullOrWhiteSpace(pin));
        Assert.Equal(3, calls);
    }

    [Fact]
    public async Task GenerateUniqueAsync_ThrowsAfterMaxAttempts()
    {
        static Task<bool> mockIsPinTaken(string _) => Task.FromResult(true);
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            PinGenerator.GenerateUniqueAsync(mockIsPinTaken));
    }
}
