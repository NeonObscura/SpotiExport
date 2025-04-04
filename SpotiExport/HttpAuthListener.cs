using System.Net;
using System.Web;

namespace SpotiExport;

internal class HttpAuthListener : IDisposable
{
    private readonly string _url;
    private HttpListener? _httpListener = null;

    public HttpAuthListener(string url)
    {
        _url = url ?? throw new ArgumentNullException(nameof(url));
    }

    public void StartListening()
    {
        if (_httpListener != null && _httpListener.IsListening)
            return;
        
        _httpListener = new HttpListener();
        _httpListener.Prefixes.Add(_url);
        _httpListener.Start();
    }

    public void StopListening()
    {
        _httpListener?.Stop();
    }

    public async Task<Result<string>> ListenForParameterAsync(string paramName)
    {
        try
        {
            var context = await _httpListener.GetContextAsync();
            var request = context.Request;
            var response = context.Response;

            string code = HttpUtility.ParseQueryString(request.Url.Query).Get(paramName);
            
            return Result<string>.Ok(code);
        }
        catch (Exception ex)
        {
            return Result<string>.Fail(ex.Message);
        }
    }

    public void Dispose()
    {
        ((IDisposable)_httpListener)?.Dispose();
    }
}