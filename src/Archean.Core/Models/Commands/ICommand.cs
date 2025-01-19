namespace Archean.Core.Models.Commands;

public interface ICommand
{
    public static abstract string CommandName { get; }

    public static virtual string[]? CommandAliases { get; }

    public Task InvokeAsync();
}
