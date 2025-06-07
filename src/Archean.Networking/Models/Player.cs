using Archean.Core.Models;
using Archean.Core.Models.Events;
using Archean.Core.Models.Networking;
using Archean.Core.Services.Events;

namespace Archean.Networking.Models;

public class Player : IPlayer
{
    private readonly IGlobalEventBus _globalEventBus;

    public sbyte Id { get; set; }

    public IConnection Connection { get; }

    public string Username { get; }

    public string DisplayName => Username;

    public float PosX { get; private set; }

    public float PosY { get; private set; }

    public float PosZ { get; private set; }

    public byte Pitch { get; private set; } // Todo

    public byte Yaw { get; private set; } // Todo

    public Player(
        IConnection connection,
        string username,
        IGlobalEventBus globalEventBus)
    {
        Connection = connection;
        Username = username;
        _globalEventBus = globalEventBus;
    }

    public void UpdatePositionAndRotation(float posX, float posY, float posZ, byte pitch, byte yaw)
    {
        PosX = posX;
        PosY = posY;
        PosZ = posZ;
        Pitch = pitch;
        Yaw = yaw;
    }

    public async Task SetPositionAsync(float posX, float posY, float posZ)
    {
        PosX = posX;
        PosY = posY;
        PosZ = posZ;

        await _globalEventBus.InvokeEventAsync(new PositionAndOrientationEvent
        {
            Player = this,
            X = posX,
            Y = posY,
            Z = posZ,
            Pitch = Pitch,
            Yaw = Yaw
        });
    }

    public async Task SetRotationAsync(byte pitch, byte yaw)
    {
        Pitch = pitch;
        Yaw = yaw;

        await _globalEventBus.InvokeEventAsync(new PositionAndOrientationEvent
        {
            Player = this,
            X = PosX,
            Y = PosY,
            Z = PosZ,
            Pitch = pitch,
            Yaw = yaw
        });
    }

    public async Task SetPositionAndRotationAsync(float posX, float posY, float posZ, byte pitch, byte yaw)
    {
        PosX = posX;
        PosY = posY;
        PosZ = posZ;
        Pitch = pitch;
        Yaw = yaw;

        await _globalEventBus.InvokeEventAsync(new PositionAndOrientationEvent
        {
            Player = this,
            X = posX,
            Y = posY,
            Z = posZ,
            Pitch = pitch,
            Yaw = yaw
        });
    }
}
