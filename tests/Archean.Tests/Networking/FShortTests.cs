using Archean.Networking.Helpers;

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
        short fshort = FixedPointHelper.WriteFixedShort(input);

        // Action
        float actual = FixedPointHelper.ReadFixedShort(fshort);

        // Assertion
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(2000, FixedPointHelper.FixedShortMaxValue)]
    [InlineData(float.MaxValue, FixedPointHelper.FixedShortMaxValue)]
    [InlineData(float.PositiveInfinity, FixedPointHelper.FixedShortMaxValue)]
    public void ConstructFromFloat_Positive_ReturnsClamped(float input, float expected)
    {
        // Setup
        short fshort = FixedPointHelper.WriteFixedShort(input);

        // Action
        float actual = FixedPointHelper.ReadFixedShort(fshort);

        // Assertion
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(-2000, FixedPointHelper.FixedShortMinValue)]
    [InlineData(float.MinValue, FixedPointHelper.FixedShortMinValue)]
    [InlineData(float.NegativeInfinity, FixedPointHelper.FixedShortMinValue)]
    public void ConstructFromFloat_Negative_ReturnsClamped(float input, float expected)
    {
        // Setup
        short fshort = FixedPointHelper.WriteFixedShort(input);

        // Action
        float actual = FixedPointHelper.ReadFixedShort(fshort);

        // Assertion
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(float.NaN, 0F)]
    public void ConstructFromFloat_NaN_ReturnsZero(float input, float expected)
    {
        // Setup
        short fshort = FixedPointHelper.WriteFixedShort(input);

        // Action
        float actual = FixedPointHelper.ReadFixedShort(fshort);

        // Assertion
        Assert.Equal(expected, actual);
    }
}
