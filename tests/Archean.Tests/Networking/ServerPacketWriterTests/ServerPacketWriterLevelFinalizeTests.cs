﻿using Archean.Core.Models.Networking.ServerPackets;

namespace Archean.Tests.Networking.ServerPacketWriterTests;

public class ServerPacketWriterLevelFinalizeTests : BaseServerPacketWriterTests
{
    private static readonly ServerLevelFinalizePacket ValidPacket = new()
    {
        XSize = default,
        YSize = default,
        ZSize = default
    };

    [Fact]
    public void WritePacket_ValidPacket_ExpectedBufferSize()
    {
        // Action
        ReadOnlyMemory<byte> buffer = _writer.WriteLevelFinalizePacket(ValidPacket);

        // Assert
        Assert.Equal(ServerLevelFinalizePacket.PacketSize, buffer.Length);
    }
}
