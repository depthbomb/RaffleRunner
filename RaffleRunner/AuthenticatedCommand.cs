using System.IO;
using AngleSharp.Html.Parser;
using System.Text.RegularExpressions;

namespace RaffleRunner;

/// The AuthenticatedCommand base class is used for commands that require making HTTP requests to the Scrap.TF website
/// while authenticated with a user cookie.
///
/// It also provides simple helper methods to make these requests easier to deal with.
[PublicAPI]
public class AuthenticatedCommand : RootCommand
{
    #region Options
    [Option("-c|--cookie", CommandOptionType.SingleValue, Description = "Your scr_session cookie value, omit to use your saved cookie instead")]
    public string Cookie { get; private set; }
    #endregion

    /// <summary>
    /// The CSRF token of the authenticated user.
    /// </summary>
    public string CsrfToken { get; private set; }

    /// <summary>
    /// The underlying <see cref="HttpClient"/> preconfigured with the user's <c>scr_session</c> cookie.
    /// </summary>
    public HttpClient HttpClient => _httpClient;

    /// <summary>
    /// The <see cref="HtmlParser"/> instance used for DOM parsing.
    /// </summary>
    public HtmlParser HtmlParser => _htmlParser;
    
    private          HttpClient _httpClient;
    private readonly Logger     _logger           = LogManager.GetCurrentClassLogger();
    private readonly string     _userAgent        = GlobalShared.MimicUserAgent;
    private readonly HtmlParser _htmlParser       = new();
    private readonly Regex      _csrfTokenPattern = new(@"value=""([a-f\d]{64})""");
    
    public override async Task OnExecuteAsync()
    {
        await Task.WhenAll(
            GetCookie(),
            CreateHttpClientAsync(),
            GetCsrfTokenAsync(),
            base.OnExecuteAsync()
        );
    }

    protected async Task<string> GetStringAsync(string path = "/")
    {
        _logger.Debug("Sending GET request to {Path}", path);
        
        var res = await _httpClient.GetAsync(path);
        if (res.StatusCode == HttpStatusCode.OK)
        {
            return await res.Content.ReadAsStringAsync();
        }
        
        throw new HttpRequestException($"Unable to get string: {res.ReasonPhrase}");
    }

    private async Task GetCookie()
    {
        if (Cookie == null)
        {
            if (!File.Exists(GlobalShared.CookieFilePath))
            {
                throw new FileNotFoundException("Unable to find cookie file");
            }
            
            Cookie = await File.ReadAllTextAsync(GlobalShared.CookieFilePath);
        }
    }

    private Task CreateHttpClientAsync()
    {
        var handler = new HttpClientHandler
        {
            UseCookies = true
        };
        var client = new HttpClient(handler);
        client.BaseAddress = new Uri("https://scrap.tf");
        client.DefaultRequestHeaders.Add("user-agent", _userAgent);
        client.DefaultRequestHeaders.Add("cookie", $"scr_session={Cookie}");

        _httpClient = client;
        
        _logger.Debug("Created HTTP client");
        
        return Task.CompletedTask;
    }

    private async Task GetCsrfTokenAsync()
    {
        _logger.Debug("Retrieving CSRF token");
        
        string html  = await GetStringAsync();
        var    match = _csrfTokenPattern.Match(html);
        if (match.Success)
        {
            _logger.Debug("Retrieved CSRF token");
            CsrfToken = match.Groups[1].Value;
        }
        else
        {
            throw new Exception("Unable to find CSRF token");
        }
    }
}