using Archean.Core.Models.Networking;

namespace Archean.Tests.Networking;

public class FShortTests
{
    [Theory]
    [InlineData(0F, 0F)]
    [InlineData(0.01F, 0F)]
    [InlineData(0.04F, 0.03125F)]
    [InlineData(1F, 1F)]
    [InlineData(200.5F, 200.5F)]
    [InlineData(MathF.PI, 3.15625F)]
    [InlineData(-1F, -1F)]
    [InlineData(-200.5F, -200.5F)]
    [InlineData(-MathF.PI, -3.15625F)]
    public void ConstructFromFloat_WithinRange_ReturnsExpected(float input, float expected)
    {
        // Setup
        FShort fshort = new FShort(input);

        // Action
        float actual = fshort.ToFloat();

        // Assertion
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(2000, FShort.MaxValue)]
    [InlineData(float.MaxValue, FShort.MaxValue)]
    [InlineData(float.PositiveInfinity, FShort.MaxValue)]
    public void ConstructFromFloat_Positive_ReturnsClamped(float input, float expected)
    {
        // Setup
        FShort fshort = new FShort(input);

        // Action
        float actual = fshort.ToFloat();

        // Assertion
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(-2000, FShort.MinValue)]
    [InlineData(float.MinValue, FShort.MinValue)]
    [InlineData(float.NegativeInfinity, FShort.MinValue)]
    public void ConstructFromFloat_Negative_ReturnsClamped(float input, float expected)
    {
        // Setup
        FShort fshort = new FShort(input);

        // Action
        float actual = fshort.ToFloat();

        // Assertion
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(float.NaN, 0F)]
    public void ConstructFromFloat_NaN_ReturnsZero(float input, float expected)
    {
        // Setup
        FShort fshort = new FShort(input);

        // Action
        float actual = fshort.ToFloat();

        // Assertion
        Assert.Equal(expected, actual);
    }
}
