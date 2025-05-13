using System.Numerics;

namespace Archean.Core.Models.Networking;

public readonly struct FByte : IEqualityOperators<FByte, FByte, bool>
{
    public const int Size = sizeof(byte);
    public const float MinValue = -4;
    public const float MaxValue = -MinValue - FractionValue;

    private const int FractionBits = 5;
    private const int FractionCount = 32;
    private const float FractionValue = 1F / FractionCount;

    private const byte SignMask = 0b_1000_0000;

    private readonly byte _value;

    public FByte(byte b)
    {
        _value = b;
    }

    public FByte(float f)
    {
        float clamped = Math.Clamp(f, MinValue, MaxValue);
        float rounded = MathF.Round(clamped * FractionCount, MidpointRounding.AwayFromZero) / FractionCount;

        sbyte integers = (sbyte)rounded;

        byte fractions = (byte)(MathF.Abs(rounded - integers) * FractionCount);
        byte negative = f < 0 ? SignMask : (byte)0b_0000_0000;

        _value = (byte)(negative | integers << FractionBits | fractions);
    }

    public float ToFloat()
    {
        bool negative = _value >= SignMask;
        byte integers = (byte)((_value & 0b_0110_0000) >> FractionBits);
        float integerValue = negative ? integers + MinValue : integers;

        int fractions = _value & 0b_0001_1111;
        if (negative)
        {
            fractions = -fractions;
        }

        return integerValue + fractions * FractionValue;
    }

    public byte ToByte() => _value;

    public override string ToString()
    {
        return ToFloat().ToString();
    }

    public static bool operator ==(FByte left, FByte right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(FByte left, FByte right)
    {
        return !left.Equals(right);
    }

    public override bool Equals(object? obj)
    {
        return obj is FByte other
            && other._value == _value;
    }

    public override int GetHashCode()
    {
        return _value;
    }
}
