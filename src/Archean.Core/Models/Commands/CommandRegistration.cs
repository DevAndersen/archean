namespace Archean.Core.Models.Commands;

public record CommandRegistration(Type Type, CommandParameter[] Parameters, string Name);
