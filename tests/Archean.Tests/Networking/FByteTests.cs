using Archean.Core.Models.Networking;

namespace Archean.Tests.Networking;

public class FByteTests
{
    [Theory]
    [InlineData(0F, 0F)]
    [InlineData(0.01F, 0F)]
    [InlineData(0.04F, 0.03125F)]
    [InlineData(1F, 1F)]
    [InlineData(2.5F, 2.5F)]
    [InlineData(MathF.PI, 3.15625F)]
    [InlineData(-1F, -1F)]
    [InlineData(-2.5F, -2.5F)]
    [InlineData(-MathF.PI, -3.15625F)]
    public void ConstructFromFloat_WithinRange_ReturnsExpected(float input, float expected)
    {
        // Setup
        FByte fbyte = new FByte(input);

        // Action
        float actual = fbyte.ToFloat();

        // Assertion
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(4, FByte.MaxValue)]
    [InlineData(float.MaxValue, FByte.MaxValue)]
    [InlineData(float.PositiveInfinity, FByte.MaxValue)]
    public void ConstructFromFloat_Positive_ReturnsClamped(float input, float expected)
    {
        // Setup
        FByte fbyte = new FByte(input);

        // Action
        float actual = fbyte.ToFloat();

        // Assertion
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(-4, FByte.MinValue)]
    [InlineData(float.MinValue, FByte.MinValue)]
    [InlineData(float.NegativeInfinity, FByte.MinValue)]
    public void ConstructFromFloat_Negative_ReturnsClamped(float input, float expected)
    {
        // Setup
        FByte fbyte = new FByte(input);

        // Action
        float actual = fbyte.ToFloat();

        // Assertion
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(float.NaN, 0F)]
    public void ConstructFromFloat_NaN_ReturnsZero(float input, float expected)
    {
        // Setup
        FByte fbyte = new FByte(input);

        // Action
        float actual = fbyte.ToFloat();

        // Assertion
        Assert.Equal(expected, actual);
    }
}
