using Archean.Core.Helpers;
using Archean.Core.Models;
using Archean.Core.Models.Worlds;
using Microsoft.Extensions.Logging;
using System.Buffers;
using System.Buffers.Binary;
using System.Runtime.InteropServices;

namespace Archean.Worlds.Services.Persistence;

public  class BlockMapPersistenceHandler
{
    private readonly ILogger<BlockMapPersistenceHandler> _logger;

    public BlockMapPersistenceHandler(ILogger<BlockMapPersistenceHandler> logger)
    {
        _logger = logger;
    }

    public async Task<BlockMap?> LoadBlockMapAsync(string fileName)
    {
        try
        {
            if (!File.Exists(fileName))
            {
                _logger.LogError("Unable to load blockmap from non-existing file {fileName}",
                    fileName);

                return null;
            }

            // Read the bytes from the file, and decompress them.
            byte[] compressedBytes = await File.ReadAllBytesAsync(fileName);
            byte[] bytes = GZipHelper.Decompress(compressedBytes);
            Span<byte> buffer = bytes.AsSpan();

            // Read the data from the uncompressed bytes.
            short width = ReadShort(ref buffer);
            short height = ReadShort(ref buffer);
            short depth = ReadShort(ref buffer);
            int length = ReadInt(ref buffer);
            Span<Block> blocks = MemoryMarshal.Cast<byte, Block>(buffer);

            if (length != width * height * depth)
            {
                throw new NotImplementedException("Blockmap dimensions did not match expected buffer length"); // Todo: Throw appropriate exception.
            }

            if (blocks.Length != length)
            {
                throw new NotImplementedException("Blockmap expected buffer length did not match actual buffer length"); // Todo: Throw appropriate exception.
            }

            return new BlockMap(width, height, depth, blocks);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred while attempting to load blockmap from file {fileName}",
                fileName);

            return null;
        }
    }

    public async Task<bool> SaveBlockMapAsync(string fileName, BlockMap blockMap)
    {
        byte[]? bytes = null;
        try
        {
            // Determine the size of the uncompressed byte buffer.
            ReadOnlyMemory<byte> blockMapMemory = blockMap.AsMemory();
            int dimensionBufferSize = sizeof(short) * 3;
            int bufferSize = blockMapMemory.Length + dimensionBufferSize;

            // Rent a byte array, and create a sliced span that fits the needed size.
            bytes = ArrayPool<byte>.Shared.Rent(bufferSize);
            Span<byte> buffer = bytes.AsSpan()[..bufferSize];

            // Write data to the buffer.
            Span<byte> writeBuffer = buffer;
            WriteShort(ref writeBuffer, blockMap.Width);
            WriteShort(ref writeBuffer, blockMap.Height);
            WriteShort(ref writeBuffer, blockMap.Depth);
            blockMapMemory.Span.CopyTo(writeBuffer);

            // Compress the buffer and save it to the specified file.
            byte[] compressedBytes = GZipHelper.Compress(buffer);
            using FileStream fileStream = File.Create(fileName);
            await fileStream.WriteAsync(compressedBytes);

            return true;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred while attempting to save blockmap to file {fileName}",
                fileName);

            return false;
        }
        finally
        {
            if (bytes != null)
            {
                ArrayPool<byte>.Shared.Return(bytes);
            }
        }
    }

    private static short ReadShort(ref Span<byte> buffer)
    {
        short value = BinaryPrimitives.ReadInt16BigEndian(buffer);
        buffer = buffer[sizeof(short)..];
        return value;
    }

    private static int ReadInt(ref Span<byte> buffer)
    {
        int value = BinaryPrimitives.ReadInt32BigEndian(buffer);
        buffer = buffer[sizeof(int)..];
        return value;
    }

    private static void WriteShort(ref Span<byte> buffer, short value)
    {
        BinaryPrimitives.WriteInt16BigEndian(buffer, value);
        buffer = buffer[sizeof(short)..];
    }
}
