namespace Archean.Core.Models.Networking;

public readonly struct FByte
{
    public const int Size = sizeof(byte);
    public const float MinValue = -4;
    public const float MaxValue = -MinValue - fractionValue;

    private const int fractionBits = 5;
    private const int fractionCount = 32;
    private const float fractionValue = 1F / fractionCount;

    private const byte signMask = 0b_1000_0000;

    private readonly byte value;

    public FByte(byte b)
    {
        value = b;
    }

    public FByte(float f)
    {
        float clamped = Math.Clamp(f, MinValue, MaxValue);
        float rounded = MathF.Round(clamped * fractionCount, MidpointRounding.AwayFromZero) / fractionCount;

        sbyte integers = (sbyte)rounded;

        byte fractions = (byte)(MathF.Abs(rounded - integers) * fractionCount);
        byte negative = f < 0 ? signMask : (byte)0b_0000_0000;

        value = (byte)(negative | integers << fractionBits | fractions);
    }

    public float ToFloat()
    {
        bool negative = value >= signMask;
        byte integers = (byte)((value & 0b_0110_0000) >> fractionBits);
        float integerValue = negative ? integers + MinValue : integers;

        int fractions = value & 0b_0001_1111;
        if (negative)
        {
            fractions = -fractions;
        }

        return integerValue + fractions * fractionValue;
    }

    public byte ToByte() => value;

    public override string ToString()
    {
        return ToFloat().ToString();
    }
}
