using Archean.Core.Models.Commands;
using System.Diagnostics.CodeAnalysis;

namespace Archean.Core.Services.Commands;

public interface ICommandRegistry
{
    bool RegisterCommand(Type commandType, bool useDefaultNames, string[]? additionalNames = null);

    bool TryGetCommand(ReadOnlySpan<char> commandName, [NotNullWhen(true)] out ICommand? command, [NotNullWhen(true)] out CommandParameter[]? parameters);

    IEnumerable<CommandRegistration> GetCommands();
}
