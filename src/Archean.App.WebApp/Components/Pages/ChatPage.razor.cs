using System.Text.RegularExpressions;

namespace Archean.App.WebApp.Components.Pages;

public partial class ChatPage
{
    [GeneratedRegex("(&[0-9a-f]|^)?(.+?)(?=&|$)", RegexOptions.IgnoreCase)]
    private static partial Regex ChatColorRegex();
}
