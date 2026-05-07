using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Notch.Services;

public record SpotifyTrackInfo(string Title, string Artist, string AlbumArtUrl, bool IsPlaying);

public class SpotifyService
{
    private const string TokenFile = "spotify_refresh.token";
    private const int CallbackPort = 3000;

    private readonly string _clientId;
    private readonly string _clientSecret;
    private readonly HttpClient _http = new();

    private string? _accessToken;
    private string? _refreshToken;
    private DateTime _tokenExpiry = DateTime.MinValue;

    public SpotifyService()
    {
        _clientId = Environment.GetEnvironmentVariable("SPOTIFY_CLIENT_ID") ?? string.Empty;
        _clientSecret = Environment.GetEnvironmentVariable("SPOTIFY_CLIENT_SECRET") ?? string.Empty;

        if (File.Exists(TokenFile))
            _refreshToken = File.ReadAllText(TokenFile).Trim();
    }

    public bool IsConfigured => !string.IsNullOrEmpty(_clientId) && !string.IsNullOrEmpty(_clientSecret);

    public async Task<bool> AuthorizeAsync()
    {
        if (!IsConfigured) return false;

        if (_refreshToken is not null && await RefreshTokenAsync())
            return true;

        return await FullAuthFlowAsync();
    }

    private async Task<bool> FullAuthFlowAsync()
    {
        var state = Guid.NewGuid().ToString("N")[..8];
        var redirectUri = $"http://127.0.0.1:{CallbackPort}/callback";
        var scope = "user-read-currently-playing user-read-playback-state";
        var authUrl = "https://accounts.spotify.com/authorize"
            + $"?response_type=code&client_id={_clientId}"
            + $"&scope={Uri.EscapeDataString(scope)}"
            + $"&redirect_uri={Uri.EscapeDataString(redirectUri)}"
            + $"&state={state}";

        Process.Start(new ProcessStartInfo(authUrl) { UseShellExecute = true });

        var code = await ListenForCallbackAsync(state);
        if (code is null) return false;

        return await ExchangeCodeAsync(code, redirectUri);
    }

    private async Task<string?> ListenForCallbackAsync(string expectedState)
    {
        using var listener = new HttpListener();
        listener.Prefixes.Add($"http://127.0.0.1:{CallbackPort}/");
        listener.Start();

        using var cts = new CancellationTokenSource(TimeSpan.FromMinutes(2));

        while (!cts.Token.IsCancellationRequested)
        {
            HttpListenerContext context;
            try
            {
                context = await listener.GetContextAsync().WaitAsync(cts.Token);
            }
            catch (OperationCanceledException)
            {
                return null;
            }

            if (context.Request.Url?.AbsolutePath != "/callback")
            {
                context.Response.StatusCode = 404;
                context.Response.Close();
                continue;
            }

            var query = context.Request.QueryString;
            var responseHtml = Encoding.UTF8.GetBytes(
                "<html><body><h2>Autenticazione completata! Puoi chiudere questa finestra.</h2></body></html>");
            context.Response.ContentLength64 = responseHtml.Length;
            await context.Response.OutputStream.WriteAsync(responseHtml, cts.Token);
            context.Response.Close();

            if (query["state"] != expectedState) return null;
            return query["code"];
        }

        return null;
    }

    private async Task<bool> ExchangeCodeAsync(string code, string redirectUri)
    {
        var body = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["grant_type"] = "authorization_code",
            ["code"] = code,
            ["redirect_uri"] = redirectUri,
        });
        return await PostTokenRequestAsync(body);
    }

    private async Task<bool> RefreshTokenAsync()
    {
        if (_refreshToken is null) return false;

        var body = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["grant_type"] = "refresh_token",
            ["refresh_token"] = _refreshToken,
        });
        return await PostTokenRequestAsync(body);
    }

    private async Task<bool> PostTokenRequestAsync(FormUrlEncodedContent body)
    {
        var credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_clientId}:{_clientSecret}"));
        using var req = new HttpRequestMessage(HttpMethod.Post, "https://accounts.spotify.com/api/token");
        req.Headers.Authorization = new AuthenticationHeaderValue("Basic", credentials);
        req.Content = body;

        var res = await _http.SendAsync(req);
        if (!res.IsSuccessStatusCode) return false;

        using var json = JsonDocument.Parse(await res.Content.ReadAsStringAsync());
        var root = json.RootElement;

        _accessToken = root.GetProperty("access_token").GetString();
        _tokenExpiry = DateTime.UtcNow.AddSeconds(root.GetProperty("expires_in").GetInt32() - 30);

        if (root.TryGetProperty("refresh_token", out var rt) && rt.GetString() is { } newRefresh)
        {
            _refreshToken = newRefresh;
            File.WriteAllText(TokenFile, _refreshToken);
        }

        return _accessToken is not null;
    }

    private async Task EnsureValidTokenAsync()
    {
        if (DateTime.UtcNow >= _tokenExpiry)
            await RefreshTokenAsync();
    }

    public async Task<SpotifyTrackInfo?> GetCurrentlyPlayingAsync()
    {
        if (_accessToken is null) return null;

        await EnsureValidTokenAsync();

        using var req = new HttpRequestMessage(HttpMethod.Get, "https://api.spotify.com/v1/me/player/currently-playing");
        req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);

        var res = await _http.SendAsync(req);

        if (res.StatusCode == HttpStatusCode.NoContent) return null;
        if (!res.IsSuccessStatusCode) return null;

        using var json = JsonDocument.Parse(await res.Content.ReadAsStringAsync());
        var root = json.RootElement;

        if (!root.TryGetProperty("item", out var item)) return null;

        var title = item.GetProperty("name").GetString() ?? string.Empty;
        var artist = item.GetProperty("artists")[0].GetProperty("name").GetString() ?? string.Empty;
        var isPlaying = root.GetProperty("is_playing").GetBoolean();

        var albumArt = string.Empty;
        if (item.TryGetProperty("album", out var album) &&
            album.TryGetProperty("images", out var images) &&
            images.GetArrayLength() > 0)
        {
            albumArt = images[0].GetProperty("url").GetString() ?? string.Empty;
        }

        return new SpotifyTrackInfo(title, artist, albumArt, isPlaying);
    }
}
