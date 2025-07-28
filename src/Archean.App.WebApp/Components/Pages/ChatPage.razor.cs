using System.Text.RegularExpressions;

namespace Archean.App.WebApp.Components.Pages;

public partial class ChatPage
{
    /// <summary>
    /// Regex pattern for grouping text according to coloring, defined as an ampersand (&amp;) followed by a single hexadecimal character.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    /// <item>Match group 1: The ampersand and hexadecimal character, indicating which color to be used. May be undefined.</item>
    /// <item>Match group 2: The text that should use the color defined in match group 1.</item>
    /// </list>
    /// </remarks>
    /// <returns></returns>
    [GeneratedRegex("(&[0-9a-f]|^)?(.+?)(?=&|$)", RegexOptions.IgnoreCase)]
    private static partial Regex ChatColorRegex();
}
