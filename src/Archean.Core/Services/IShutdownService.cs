namespace Archean.Core.Services;

public interface IShutdownService
{
    Task OnShutdownAsync();
}
