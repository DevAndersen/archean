namespace Archean.Core.Services;

public interface ISkinService
{
    bool DoesSkinFileExist(string username);

    Task<bool> DownloadSkinAsync(string username);

    string GetSkinFileName(string username);
}
