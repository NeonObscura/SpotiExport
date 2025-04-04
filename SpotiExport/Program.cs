using System.Net.Http.Headers;
using System.Text;

namespace SpotiExport;

class Program
{
    
    private static string GenerateSpotifyAuthUrl(string clientId, string redirectUri)
    {
        return $"https://accounts.spotify.com/authorize?client_id={clientId}&response_type=code&redirect_uri={redirectUri}";
    }
    
    private static void OpenBrowserWithUrl(string url)
    {
        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
        {
            FileName = url,
            UseShellExecute = true
        });
    }

    static async Task Main()
    {
        var cred = GetCredentials();
        if (cred.Failed)
        {
            Console.WriteLine($"Failed: {cred.ErrorMessage}");
            return;
        }
        
        Console.WriteLine($"Access token: {token.Data}");
        
        //var userInfo = await GetUserInfoAsync(token.Data);
    }
}