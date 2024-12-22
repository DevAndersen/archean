using System.Numerics;

namespace Archean.Core.Models.Networking;

public readonly struct FShort : IEqualityOperators<FShort, FShort, bool>
{
    public const int Size = sizeof(ushort);
    public const float MinValue = -1024;
    public const float MaxValue = -MinValue - fractionValue;

    private const int fractionBits = 5;
    private const int fractionCount = 32;
    private const float fractionValue = 1F / fractionCount;

    private const ushort signMask = 0b_1000_0000_0000_0000;

    private readonly ushort value;

    public FShort(ushort u)
    {
        value = u;
    }

    public FShort(float f)
    {
        float clamped = Math.Clamp(f, MinValue, MaxValue);
        float rounded = MathF.Round(clamped * fractionCount, MidpointRounding.AwayFromZero) / fractionCount;

        short integers = (short)rounded;

        ushort fractions = (ushort)(MathF.Abs(rounded - integers) * fractionCount);
        ushort negative = f < 0 ? signMask : (ushort)0b_0000_0000_0000_0000;

        value = (ushort)(negative | integers << fractionBits | fractions);
    }

    public float ToFloat()
    {
        bool negative = value >= signMask;
        ushort integers = (ushort)((value & 0b_0111_1111_1110_0000) >> fractionBits);
        float integerValue = negative ? integers + MinValue : integers;

        int fractions = value & 0b_0000_0000_0001_1111;
        if (negative)
        {
            fractions = -fractions;
        }

        return integerValue + fractions * fractionValue;
    }

    public ushort ToUShort() => value;

    public override string ToString()
    {
        return ToFloat().ToString();
    }

    public static bool operator ==(FShort left, FShort right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(FShort left, FShort right)
    {
        return !left.Equals(right);
    }

    public override bool Equals(object? obj)
    {
        return obj is FShort other
            && other.value == value;
    }

    public override int GetHashCode()
    {
        return value;
    }
}
