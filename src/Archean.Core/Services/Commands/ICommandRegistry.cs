using Archean.Core.Models.Commands;
using System.Diagnostics.CodeAnalysis;

namespace Archean.Core.Services.Commands;

public interface ICommandRegistry
{
    bool RegisterCommand(Type commandType);

    bool RegisterCommandAlias(Type commandType, string alias);

    void RegisterCommandAliases(Type commandType);

    bool TryGetCommand(ReadOnlySpan<char> commandName, [NotNullWhen(true)] out ICommand? command);

    IEnumerable<ICommand> GetCommands();
}
