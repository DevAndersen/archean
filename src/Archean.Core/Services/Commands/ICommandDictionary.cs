using Archean.Core.Models.Commands;
using System.Diagnostics.CodeAnalysis;

namespace Archean.Core.Services.Commands;

public interface ICommandDictionary
{
    bool Register<TCommand>() where TCommand : ICommand;

    bool TryGetCommand(ReadOnlySpan<char> commandName, [NotNullWhen(true)] out ICommand? command);
}
