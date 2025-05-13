namespace Archean.Core.Models.Commands;

public interface ICommand
{
    static abstract string CommandName { get; }

    static virtual string[]? CommandAliases { get; }

    Task InvokeAsync();
}
