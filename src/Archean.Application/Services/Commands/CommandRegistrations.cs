using Archean.Core.Models.Commands;

namespace Archean.Application.Services.Commands;

public class CommandRegistrations
{
    private readonly List<Type> commandTypes = [];

    public IReadOnlyList<Type> CommandTypes => commandTypes;

    public void RegisterCommand<TCommand>() where TCommand : ICommand
    {
        commandTypes.Add(typeof(TCommand));
    }
}
