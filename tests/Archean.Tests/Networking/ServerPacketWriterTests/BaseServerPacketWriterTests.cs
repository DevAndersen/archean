﻿using Archean.Application.Services.Networking;

namespace Archean.Tests.Networking.ServerPacketWriterTests;

public abstract class BaseServerPacketWriterTests
{
    protected readonly ServerPacketWriter writer;

    public BaseServerPacketWriterTests()
    {
        PacketDataWriter dataWriter = new PacketDataWriter();
        writer = new ServerPacketWriter(dataWriter);
    }
}
