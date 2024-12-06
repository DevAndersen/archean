﻿namespace Archean.Core.Models.Networking;

public interface IConnection : IDisposable
{
    public bool IsConnected { get; }

    public Task SendAsync(ReadOnlyMemory<byte> data);

    public Task<ReadOnlyMemory<byte>> ReadAsync(CancellationToken cancellationToken);

    public void Disconnect();
}
