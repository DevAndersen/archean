namespace Archean.Core.Models.Commands;

public interface ICommandInvoker
{
    /// <summary>
    /// Attempts to find and invoke the <see cref="ICommand"/> corresponding to <paramref name="commandText"/>.
    /// </summary>
    /// <remarks>
    /// <paramref name="commandText"/> is expected to not include any command prefix characters, e.g. '<c>/</c>'.
    /// </remarks>
    /// <param name="commandText"></param>
    /// <returns></returns>
    Task<bool> TryInvokeCommandAsync(ReadOnlyMemory<char> commandText);
}
