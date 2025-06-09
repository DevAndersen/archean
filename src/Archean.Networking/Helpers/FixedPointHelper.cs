namespace Archean.Networking.Helpers;

public static class FixedPointHelper
{
    private const float Fractions = 32;

    private const float FixedByteMinValue = -4;
    private const float FixedByteMaxValue = -FixedByteMinValue - (1F / Fractions);

    private const float FixedShortMinValue = -1024;
    private const float FixedShortMaxValue = -FixedShortMinValue - (1F / Fractions);

    public static sbyte WriteFixedSByte(float value)
    {
        return (sbyte)(float.Clamp(value, FixedByteMinValue, FixedByteMaxValue) * Fractions);
    }

    public static float ReadFixedSByte(sbyte value)
    {
        return value / Fractions;
    }

    public static short WriteFixedShort(float value)
    {
        return (short)(float.Clamp(value, FixedShortMinValue, FixedShortMaxValue) * Fractions);
    }

    public static float ReadFixedShort(short value)
    {
        return value / Fractions;
    }
}
