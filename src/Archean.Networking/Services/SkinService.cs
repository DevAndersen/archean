using Archean.Core.Services;
using System.Net.Http.Json;
using System.Text.Json;

namespace Archean.Networking.Services;

public class SkinService : ISkinService
{
    private readonly HttpClient _httpClient = new HttpClient();

    private readonly IOptions<SkinSettings> _skinSettings;
    private readonly ILogger<ISkinService> _logger;

    public SkinService(IOptions<SkinSettings> skinSettings, ILogger<ISkinService> logger)
    {
        _skinSettings = skinSettings;
        _logger = logger;
    }

    public bool DoesDefaultSkinFileExist()
    {
        return File.Exists(GetAbsoluteSkinPath(GetDefaultSkinFileName()));
    }

    public bool DoesSkinFileExist(string username)
    {
        return File.Exists(GetAbsoluteSkinPath(username));
    }

    public async Task<bool> DownloadDefaultSkinAsync()
    {
        string? skinUrl = "https://assets.mojang.com/SkinTemplates/steve.png";
        return await DownloadSkinFromUrlAsync(GetDefaultSkinFileName(), skinUrl);
    }

    public async Task<bool> DownloadSkinAsync(string username)
    {
        string? skinUrl = await GetSkinUrlAsync(username);
        return await DownloadSkinFromUrlAsync(username, skinUrl);
    }

    private async Task<bool> DownloadSkinFromUrlAsync(string username, string? skinUrl)
    {
        if (skinUrl == null)
        {
            return false;
        }

        try
        {
            using Stream stream = await _httpClient.GetStreamAsync(skinUrl);
            using FileStream fileStream = File.Create(GetAbsoluteSkinPath(username));
            stream.CopyTo(fileStream);

            return true;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Unexpected error during skin download for user {username}",
                username);

            return false;
        }
    }

    public string GetDefaultSkinFileName()
    {
        return "_";
    }

    public string GetSkinFileName(string username)
    {
        return username + ".png";
    }

    private string GetAbsoluteSkinPath(string username)
    {
        return Path.Join(_skinSettings.Value.SkinsDirectory, GetSkinFileName(username));
    }

    private async Task<string?> GetSkinUrlAsync(string username)
    {
        try
        {
            PlayerUuidResponse? uuidResponse = await _httpClient.GetFromJsonAsync<PlayerUuidResponse>($"https://api.mojang.com/users/profiles/minecraft/{username}");
            if (uuidResponse == null)
            {
                _logger.LogError("Error during skin download for user {username}, failed to retrieve user UUID",
                    username);

                return null;
            }

            PlayerSkinResponse? skinResponse = await _httpClient.GetFromJsonAsync<PlayerSkinResponse>($"https://sessionserver.mojang.com/session/minecraft/profile/{uuidResponse.Id}");
            if (skinResponse == null)
            {
                _logger.LogError("Error during skin download for user {username}, failed to retrieve skin",
                    username);

                return null;
            }

            PlayerSkinPropertyResponse? base64TexturesData = skinResponse.Properties.FirstOrDefault(x => x.Name == "textures");
            if (base64TexturesData == null)
            {
                _logger.LogError("Error during skin download for user {username}, textures data not found",
                    username);

                return null;
            }

            byte[] json = Convert.FromBase64String(base64TexturesData.Value);
            PlayerSkinDataModel? skinDataModel = JsonSerializer.Deserialize<PlayerSkinDataModel>(json, JsonSerializerOptions.Web);
            if (string.IsNullOrWhiteSpace(skinDataModel?.Textures?.Skin?.Url))
            {
                _logger.LogError("Error during skin download for user {username}, unable to deserialize base-64 response",
                    username);

                return null;
            }

            return skinDataModel.Textures.Skin.Url;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Unexpected error during skin url download for user {username}",
                username);

            return null;
        }
    }

    private sealed record PlayerUuidResponse(string Id, string Name);

    private sealed record PlayerSkinResponse(string Id, string Name, PlayerSkinPropertyResponse[] Properties);

    private sealed record PlayerSkinPropertyResponse(string Name, string Value);

    private sealed record PlayerSkinDataModel(long Timestamp, string ProfileId, string ProfileName, TexturesModel Textures);

    private sealed record TexturesModel(SkinTextureModel Skin);

    private sealed record SkinTextureModel(string Url);
}
