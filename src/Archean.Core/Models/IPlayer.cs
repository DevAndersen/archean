using Archean.Core.Models.Networking;

namespace Archean.Core.Models;

public interface IPlayer
{
    sbyte Id { get; set; }

    IConnection Connection { get; }

    string DisplayName { get; }

    string Username { get; }

    float PosX { get; }

    float PosY { get; }

    float PosZ { get; }

    byte Pitch { get; } // Todo

    byte Yaw { get; } // Todo

    /// <summary>
    /// Updates the position and rotation of the player.
    /// </summary>
    /// <param name="posX"></param>
    /// <param name="posY"></param>
    /// <param name="posZ"></param>
    /// <param name="pitch"></param>
    /// <param name="yaw"></param>
    void UpdatePositionAndRotation(float posX, float posY, float posZ, byte pitch, byte yaw);

    /// <summary>
    /// Change the position of the player.
    /// </summary>
    /// <param name="posX"></param>
    /// <param name="posY"></param>
    /// <param name="posZ"></param>
    /// <returns></returns>
    Task SetPositionAsync(float posX, float posY, float posZ);

    /// <summary>
    /// Change the rotation of the player.
    /// </summary>
    /// <param name="pitch"></param>
    /// <param name="yaw"></param>
    /// <returns></returns>
    Task SetRotationAsync(byte pitch, byte yaw);

    /// <summary>
    /// Change the position and rotation of the player.
    /// </summary>
    /// <param name="posX"></param>
    /// <param name="posY"></param>
    /// <param name="posZ"></param>
    /// <param name="pitch"></param>
    /// <param name="yaw"></param>
    /// <returns></returns>
    Task SetPositionAndRotationAsync(float posX, float posY, float posZ, byte pitch, byte yaw);
}
