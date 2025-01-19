using Archean.Core.Models.Commands;
using System.Diagnostics.CodeAnalysis;

namespace Archean.Core.Services.Commands;

public interface ICommandDictionary
{
    public bool Register<TCommand>() where TCommand : ICommand;

    public bool TryGetCommand(string commandName, [NotNullWhen(true)] out ICommand? command);
}
