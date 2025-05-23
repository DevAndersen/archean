using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage(
    "Performance",
    "CA1859:Use concrete types when possible for improved performance",
    Justification = "Replicating expected behavior is more important than performance for a test project.")]
