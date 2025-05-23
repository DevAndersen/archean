﻿using Archean.Application.Services.Networking;
using Archean.Core.Models.Networking.ClientPackets;

namespace Archean.Tests.Networking;

public class ClientPacketReaderTests
{
    private readonly ClientPacketReader _reader;

    public ClientPacketReaderTests()
    {
        PacketDataReader dataReader = new PacketDataReader();
        _reader = new ClientPacketReader(dataReader);
    }

    [Fact]
    public void ReadIdentificationPacket_BufferTooSmall_ThrowsException()
    {
        // Setup
        ReadOnlyMemory<byte> buffer = new byte[ClientIdentificationPacket.PacketSize - 1];

        // Assertion
        Assert.Throws<ArgumentOutOfRangeException>(() => _reader.ReadIdentificationPacket(buffer));
    }

    [Fact]
    public void ReadIdentificationPacket_BufferTooLarge_ThrowsException()
    {
        // Setup
        ReadOnlyMemory<byte> buffer = new byte[ClientIdentificationPacket.PacketSize + 1];

        // Assertion
        Assert.Throws<ArgumentOutOfRangeException>(() => _reader.ReadIdentificationPacket(buffer));
    }

    [Fact]
    public void ReadMessagePacket_BufferTooSmall_ThrowsException()
    {
        // Setup
        ReadOnlyMemory<byte> buffer = new byte[ClientMessagePacket.PacketSize - 1];

        // Assertion
        Assert.Throws<ArgumentOutOfRangeException>(() => _reader.ReadMessagePacket(buffer));
    }

    [Fact]
    public void ReadMessagePacket_BufferTooLarge_ThrowsException()
    {
        // Setup
        ReadOnlyMemory<byte> buffer = new byte[ClientMessagePacket.PacketSize + 1];

        // Assertion
        Assert.Throws<ArgumentOutOfRangeException>(() => _reader.ReadMessagePacket(buffer));
    }

    [Fact]
    public void ReadPositionAndOrientationPacket_BufferTooSmall_ThrowsException()
    {
        // Setup
        ReadOnlyMemory<byte> buffer = new byte[ClientPositionAndOrientationPacket.PacketSize - 1];

        // Assertion
        Assert.Throws<ArgumentOutOfRangeException>(() => _reader.ReadPositionAndOrientationPacket(buffer));
    }

    [Fact]
    public void ReadPositionAndOrientationPacket_BufferTooLarge_ThrowsException()
    {
        // Setup
        ReadOnlyMemory<byte> buffer = new byte[ClientPositionAndOrientationPacket.PacketSize + 1];

        // Assertion
        Assert.Throws<ArgumentOutOfRangeException>(() => _reader.ReadPositionAndOrientationPacket(buffer));
    }

    [Fact]
    public void ReadSetBlockPacket_BufferTooSmall_ThrowsException()
    {
        // Setup
        ReadOnlyMemory<byte> buffer = new byte[ClientSetBlockPacket.PacketSize - 1];

        // Assertion
        Assert.Throws<ArgumentOutOfRangeException>(() => _reader.ReadSetBlockPacket(buffer));
    }

    [Fact]
    public void ReadSetBlockPacket_BufferTooLarge_ThrowsException()
    {
        // Setup
        ReadOnlyMemory<byte> buffer = new byte[ClientSetBlockPacket.PacketSize + 1];

        // Assertion
        Assert.Throws<ArgumentOutOfRangeException>(() => _reader.ReadSetBlockPacket(buffer));
    }
}
