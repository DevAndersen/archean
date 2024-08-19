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
    }
}
