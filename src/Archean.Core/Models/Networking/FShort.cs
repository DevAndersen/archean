using System.Numerics;

namespace Archean.Core.Models.Networking;

public readonly struct FShort : IEqualityOperators<FShort, FShort, bool>
{
    public const int Size = sizeof(ushort);
    public const float MinValue = -1024;
    public const float MaxValue = -MinValue - FractionValue;

    private const int FractionBits = 5;
    private const int FractionCount = 32;
    private const float FractionValue = 1F / FractionCount;

    private const ushort SignMask = 0b_1000_0000_0000_0000;

    private readonly ushort _value;

    public FShort(ushort u)
    {
        _value = u;
    }

    public FShort(float f)
    {
        float clamped = Math.Clamp(f, MinValue, MaxValue);
        float rounded = MathF.Round(clamped * FractionCount, MidpointRounding.AwayFromZero) / FractionCount;

        short integers = (short)rounded;

        ushort fractions = (ushort)(MathF.Abs(rounded - integers) * FractionCount);
        ushort negative = f < 0 ? SignMask : (ushort)0b_0000_0000_0000_0000;

        _value = (ushort)(negative | integers << FractionBits | fractions);
    }

    public float ToFloat()
    {
        bool negative = _value >= SignMask;
        ushort integers = (ushort)((_value & 0b_0111_1111_1110_0000) >> FractionBits);
        float integerValue = negative ? integers + MinValue : integers;

        int fractions = _value & 0b_0000_0000_0001_1111;
        if (negative)
        {
            fractions = -fractions;
        }

        return integerValue + fractions * FractionValue;
    }

    public ushort ToUShort() => _value;

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
            && other._value == _value;
    }

    public override int GetHashCode()
    {
        return _value;
    }
}
