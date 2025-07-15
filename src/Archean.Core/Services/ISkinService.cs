namespace Archean.Core.Services;

public interface ISkinService
{
    bool DoesDefaultSkinFileExist();

    bool DoesSkinFileExist(string username);

    Task<bool> DownloadDefaultSkinAsync();

    Task<bool> DownloadSkinAsync(string username);

    string GetDefaultSkinFileName();

    string GetSkinFileName(string username);
}
