namespace Archean.Core.Models.Commands;

/// <summary>
/// Indicates that the decorated class is a command, as well as specifying related metadata.
/// </summary>
/// <remarks>
/// This is only exepcted to be used on the properties of classes inheriting from <see cref="ICommand"/>.
/// </remarks>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class CommandAttribute : Attribute
{
    public string Name { get; }

    public string[]? Aliases { get; init; }

    public CommandAttribute(string name)
    {
        Name = name;
    }
}
