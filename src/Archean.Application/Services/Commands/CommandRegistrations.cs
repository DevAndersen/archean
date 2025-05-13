using Archean.Core.Models.Commands;

namespace Archean.Application.Services.Commands;

public class CommandRegistrations
{
    private readonly List<Type> _commandTypes = [];

    public IReadOnlyList<Type> CommandTypes => _commandTypes;

    public void RegisterCommand<TCommand>() where TCommand : ICommand
    {
        _commandTypes.Add(typeof(TCommand));
    }
}
