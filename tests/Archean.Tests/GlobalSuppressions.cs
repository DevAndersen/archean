using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage(
    "Performance",
    "CA1859:Use concrete types when possible for improved performance",
    Justification = "Replicating expected behavior is more important than improving performance for a test project.")]

[assembly: SuppressMessage(
    "Style",
    "IDE0039:Use local function",
    Justification = "It makes certain tests simpler to write.")]
