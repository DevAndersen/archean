namespace Archean.Commands;

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
