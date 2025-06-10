namespace Archean.Networking.Helpers;

public static class FixedPointHelper
{
    private const float Fractions = 32;

    public const float FixedByteMinValue = -4;
    public const float FixedByteMaxValue = -FixedByteMinValue - (1F / Fractions);

    public const float FixedShortMinValue = -1024;
    public const float FixedShortMaxValue = -FixedShortMinValue - (1F / Fractions);

    public static sbyte WriteFixedSByte(float value)
    {
        float clamped = float.Clamp(value, FixedByteMinValue, FixedByteMaxValue) * Fractions;
        float rounded = float.Round(clamped, 0, MidpointRounding.AwayFromZero);
        return (sbyte)rounded;
    }

    public static float ReadFixedSByte(sbyte value)
    {
        return value / Fractions;
    }

    public static short WriteFixedShort(float value)
    {
        float clamped = float.Clamp(value, FixedShortMinValue, FixedShortMaxValue) * Fractions;
        float rounded = float.Round(clamped, 0, MidpointRounding.AwayFromZero);
        return (short)rounded;
    }

    public static float ReadFixedShort(short value)
    {
        return value / Fractions;
    }
}
