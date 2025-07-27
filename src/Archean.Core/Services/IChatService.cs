using Archean.Core.Models.Events;

namespace Archean.Core.Services;

public interface IChatService
{
    string FormatMessageEvent(MessageEvent messageEvent, out bool wasMessageTruncated, out int untruncatedLength);
}
