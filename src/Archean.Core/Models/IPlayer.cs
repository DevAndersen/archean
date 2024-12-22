using Archean.Core.Models.Networking;

namespace Archean.Core.Models;

public interface IPlayer
{
    public sbyte Id { get; set; }

    public IConnection Connection { get; }

    public string DisplayName { get; }

    public string Username { get; }

    public float PosX { get; }

    public float PosY { get; }

    public float PosZ { get; }

    public byte Pitch { get; } // Todo

    public byte Yaw { get; } // Todo

    /// <summary>
    /// Updates the position and rotation of the player.
    /// </summary>
    /// <param name="posX"></param>
    /// <param name="posY"></param>
    /// <param name="posZ"></param>
    /// <param name="pitch"></param>
    /// <param name="yaw"></param>
    public void UpdatePositionAndRotation(float posX, float posY, float posZ, byte pitch, byte yaw);

    /// <summary>
    /// Change the position of the player.
    /// </summary>
    /// <param name="posX"></param>
    /// <param name="posY"></param>
    /// <param name="posZ"></param>
    /// <returns></returns>
    public Task SetPositionAsync(float posX, float posY, float posZ);

    /// <summary>
    /// Change the rotation of the player.
    /// </summary>
    /// <param name="pitch"></param>
    /// <param name="yaw"></param>
    /// <returns></returns>
    public Task SetRotationAsync(byte pitch, byte yaw);

    /// <summary>
    /// Change the position and rotation of the player.
    /// </summary>
    /// <param name="posX"></param>
    /// <param name="posY"></param>
    /// <param name="posZ"></param>
    /// <param name="pitch"></param>
    /// <param name="yaw"></param>
    /// <returns></returns>
    public Task SetPositionAndRotationAsync(float posX, float posY, float posZ, byte pitch, byte yaw);
}
