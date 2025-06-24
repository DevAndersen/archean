using Archean.Networking.Helpers;

namespace Archean.Tests.Networking;

public class FixedSByteTests
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
        // Arrange
        sbyte fbyte = FixedPointHelper.WriteFixedSByte(input);

        // Act
        float actual = FixedPointHelper.ReadFixedSByte(fbyte);

        // Assertion
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(4, FixedPointHelper.FixedByteMaxValue)]
    [InlineData(float.MaxValue, FixedPointHelper.FixedByteMaxValue)]
    [InlineData(float.PositiveInfinity, FixedPointHelper.FixedByteMaxValue)]
    public void ConstructFromFloat_Positive_ReturnsClamped(float input, float expected)
    {
        // Arrange
        sbyte fbyte = FixedPointHelper.WriteFixedSByte(input);

        // Act
        float actual = FixedPointHelper.ReadFixedSByte(fbyte);

        // Assertion
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(-4, FixedPointHelper.FixedByteMinValue)]
    [InlineData(float.MinValue, FixedPointHelper.FixedByteMinValue)]
    [InlineData(float.NegativeInfinity, FixedPointHelper.FixedByteMinValue)]
    public void ConstructFromFloat_Negative_ReturnsClamped(float input, float expected)
    {
        // Arrange
        sbyte fbyte = FixedPointHelper.WriteFixedSByte(input);

        // Act
        float actual = FixedPointHelper.ReadFixedSByte(fbyte);

        // Assertion
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(float.NaN, 0F)]
    public void ConstructFromFloat_NaN_ReturnsZero(float input, float expected)
    {
        // Arrange
        sbyte fbyte = FixedPointHelper.WriteFixedSByte(input);

        // Act
        float actual = FixedPointHelper.ReadFixedSByte(fbyte);

        // Assertion
        Assert.Equal(expected, actual);
    }
}
