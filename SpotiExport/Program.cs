using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Web;

namespace SpotiExport;

class Program
{
    private static readonly string ClientId = "c5a091289be04a82be47d3757c9a23f7";
    private static readonly string CallbackUri = "http://localhost:8888/callback";
    private static readonly string CallbackListener = "http://localhost:8888/callback/";
    
    private static string GenerateSpotifyAuthUrl(string clientId, string redirectUri, string codeChallenge, string challengeMethod)
    {
        return   $"https://accounts.spotify.com/authorize?" +
                 $"client_id={clientId}" +
                 $"&response_type=code" +
                 $"&redirect_uri={HttpUtility.UrlEncode(redirectUri)}" +
                 $"&scope=playlist-read-private%20playlist-read-collaborative" +
                 $"&code_challenge_method={challengeMethod}" +
                 $"&code_challenge={codeChallenge}";
    }
    
    private static void OpenBrowserWithUrl(string url)
    {
        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
        {
            FileName = url,
            UseShellExecute = true
        });
    }
    
    public static async Task<Result<SpotifyToken>> ExchangeCodeForToken(string clientId, string codeVerifier, string code, string redirectUri)
    {
        using var client = new HttpClient();

        var requestBody = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("grant_type", "authorization_code"),
            new KeyValuePair<string, string>("code", code),
            new KeyValuePair<string, string>("redirect_uri", redirectUri),
            new KeyValuePair<string, string>("client_id", clientId),
            new KeyValuePair<string, string>("code_verifier", codeVerifier)
        });

        try
        {
            var response = await client.PostAsync("https://accounts.spotify.com/api/token", requestBody);
            var responseText = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var token = JsonSerializer.Deserialize<SpotifyToken>(responseText);
                return Result<SpotifyToken>.Ok(token);
            }
            else
            {
                return Result<SpotifyToken>.Fail("Error occured while exchanging spotify access token");
            }
        }
        catch (Exception ex)
        {
            return Result<SpotifyToken>.Fail(ex.Message);
        }
    }

    static async Task Main()
    {
        // get PKCE Challenge
        var pkceGenerator = new PkceChallengeGenerator();
        var codeVerifier = pkceGenerator.GenerateCodeVerifier();
        var codeChallenge = pkceGenerator.GenerateCodeChallenge(codeVerifier);
        
        // Get user code
        var authListener = new HttpAuthListener(CallbackListener);
        authListener.StartListening();
        
        Console.WriteLine("Starting browser for Spotify authentication...");
        OpenBrowserWithUrl(GenerateSpotifyAuthUrl(
            ClientId,
            CallbackUri,
            codeChallenge,
            pkceGenerator.ChallengeMethod
            ));
        
        Console.WriteLine("Waiting for Spotify callback...");
        var code = await authListener.ListenForParameterAsync("code");
        
        if (code.Failed)
        {
            Console.WriteLine($"Failed to obtain user code: {code.ErrorMessage}");
            return;
        }
        
        // Get token
        Console.WriteLine($"Access code: {code.ResultValue}");
        var token = await ExchangeCodeForToken(ClientId, codeVerifier, code.ResultValue, CallbackUri);
        if (token.Failed)
        {
            Console.WriteLine($"Failed to obtain user code: {token.ErrorMessage}");
            return;
        }

        Console.WriteLine($"Access token: {token.ResultValue}");

        //var userInfo = await GetUserInfoAsync(token.Data);
    }
}