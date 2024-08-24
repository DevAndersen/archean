namespace Archean.Core.Models.Networking;

public enum PlayerType : byte
{
    /// <summary>
    /// A normal player, cannot break bedrock.
    /// </summary>
    Normal = 0,

    /// <summary>
    /// An op, can break bedrock.
    /// </summary>
    Op = 100
}
