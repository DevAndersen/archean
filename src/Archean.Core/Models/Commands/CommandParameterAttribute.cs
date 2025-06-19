namespace Archean.Core.Models.Commands;

/// <summary>
/// Indicates that the decorated property should be considered a parameter for a command.
/// </summary>
/// <remarks>
/// This is only exepcted to be used on the properties of classes inheriting from <see cref="ICommand"/>.
/// </remarks>
[AttributeUsage(AttributeTargets.Property, Inherited = true)]
public class CommandParameterAttribute : Attribute
{
    public int Order { get; }

    public bool Required { get; set; }

    public CommandParameterAttribute(int order)
    {
        Order = order;
    }
}
