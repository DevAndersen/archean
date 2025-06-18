using System.Reflection;

namespace Archean.Core.Models.Commands;

public record CommandParameter(PropertyInfo Property, CommandParameterAttribute Attribute);
