using Archean.Core.Models.Commands;
using System.Diagnostics.CodeAnalysis;

namespace Archean.Core.Services.Commands;

/// <summary>
/// Handles registration and iteration of commands.
/// </summary>
public interface ICommandRegistry
{
    /// <summary>
    /// Register <paramref name="commandType"/> as a command.
    /// </summary>
    /// <param name="commandType"></param>
    /// <param name="useDefaultNames"></param>
    /// <param name="additionalNames"></param>
    /// <returns></returns>
    bool RegisterCommand(Type commandType, bool useDefaultNames, string[]? additionalNames = null);

    /// <summary>
    /// Attempts to find a command based on <paramref name="commandName"/>, and, if successful,
    /// returns a newly created <paramref name="command"/> with its corresponding <paramref name="parameters"/>.
    /// </summary>
    /// <param name="commandName"></param>
    /// <param name="command"></param>
    /// <param name="parameters"></param>
    /// <returns></returns>
    bool TryGetCommand(ReadOnlySpan<char> commandName, [NotNullWhen(true)] out Command? command, [NotNullWhen(true)] out CommandParameter[]? parameters);

    /// <summary>
    /// Retrieve a collection of all registered commands.
    /// </summary>
    /// <returns></returns>
    IEnumerable<CommandRegistration> GetCommands();
}
