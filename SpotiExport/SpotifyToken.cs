using System.Text.Json.Serialization;

namespace SpotiExport;

public class SpotifyToken
{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; }
    [JsonPropertyName("refresh_token")]
    public string RefreshToken { get; set; }
    [JsonPropertyName("expires_in")]
    public uint ExpiresIn { get; set; }
}