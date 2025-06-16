namespace Archean.Core.Models.Commands;

public interface ICommand
{
    Task InvokeAsync();
}
