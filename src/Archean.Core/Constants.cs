using Archean.Core.Models;

namespace Archean.Core;

/// <summary>
/// Contains constant values.
/// </summary>
public static class Constants
{
    public static class Blocks
    {
        /// <summary>
        /// The highest valid Block ID to send to clients.
        /// </summary>
        public const Block HighestValidBlockId = Block.Obsidian;
    }

    public static class Worlds
    {
        /// <summary>
        /// The smallest size of the dimensions of a world.
        /// </summary>
        public const int MinWorldDimensionSize = 16;

        /// <summary>
        /// The largest size of the dimensions of a world.
        /// </summary>
        public const int MaxWorldDimensionSize = 1023;

        /// <summary>
        /// The dimensions of a world must be evenly divisible by this value.
        /// </summary>
        public const int WorldDimensionIncrement = 16;
    }

    public static class Networking
    {
        /// <summary>
        /// The maximum size of strings.
        /// </summary>
        public const int StringLength = 64;

        /// <summary>
        /// The maximum size of byte arrays.
        /// </summary>
        public const int ByteArrayLength = 1024;

        /// <summary>
        /// The player ID used to refer to the current player.
        /// </summary>
        public const sbyte PlayerSelfId = -1;

        /// <summary>
        /// The version of the networking protocol used by the server, and expected to be used by clients.
        /// </summary>
        public const byte ProtocolVersion = 7;
    }

    public static class Players
    {
        public const sbyte HighestPlayerId = sbyte.MaxValue;
    }
}
