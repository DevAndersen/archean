namespace Archean.Core.Models.Commands;

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
