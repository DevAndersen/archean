namespace Archean.Core.Exceptions;

public class UnexpectedPacketTypeException : Exception
{
    public Type PacketType { get; }

    public UnexpectedPacketTypeException(Type packetType) : base($"Unknown packet type {packetType.FullName}")
    {
        PacketType = packetType;
    }
}
