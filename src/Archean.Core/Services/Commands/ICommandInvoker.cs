﻿using Archean.Core.Models;
using Archean.Core.Models.Commands;

namespace Archean.Core.Services.Commands;

/// <summary>
/// Handles invoking commands.
/// </summary>
public interface ICommandInvoker
{
    /// <summary>
    /// Attempts to find and invoke the <see cref="Command"/> corresponding to <paramref name="commandText"/>.
    /// </summary>
    /// <remarks>
    /// <paramref name="commandText"/> is expected to not include any command prefix characters, e.g. '<c>/</c>'.
    /// </remarks>
    /// <param name="commandText"></param>
    /// <param name="invokingPlayer"></param>
    /// <returns></returns>
    Task<bool> TryInvokeCommandAsync(ReadOnlyMemory<char> commandText, IPlayer? invokingPlayer);
}
