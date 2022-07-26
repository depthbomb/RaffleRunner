﻿#region License
// RaffleRunner
// Copyright(C) 2022 Caprine Logic
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <https://www.gnu.org/licenses/>.
// 
// Created on     07/25/2022 @ 18:44
// Last edited on 07/25/2022 @ 19:11
#endregion

using RaffleRunner.Attributes;
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
    [Cookie]
    [Required]
    [Option("-c|--cookie", CommandOptionType.SingleValue, Description = "Your scr_session cookie value")]
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
    private readonly string     _userAgent        = GlobalShared.MimicUserAgent;
    private readonly HtmlParser _htmlParser       = new();
    private readonly Regex      _csrfTokenPattern = new(@"value=""([a-f\d]{64})""");
    
    public override async Task OnExecuteAsync()
    {
        await Task.WhenAll(
            CreateHttpClientAsync(),
            GetCsrfTokenAsync(),
            base.OnExecuteAsync()
        );
    }

    protected async Task<string> GetStringAsync(string path = "/")
    {
        Logger.Debug("Sending GET request to {0}", path);
        
        var res = await _httpClient.GetAsync(path);
        if (res.StatusCode == HttpStatusCode.OK)
        {
            return await res.Content.ReadAsStringAsync();
        }
        
        throw new HttpRequestException($"Unable to get string: {res.ReasonPhrase}");
    }

    private async Task CreateHttpClientAsync()
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
        
        Logger.Debug("Created HTTP client");
    }

    private async Task GetCsrfTokenAsync()
    {
        Logger.Debug("Retrieving CSRF token");
        
        string html  = await GetStringAsync();
        var    match = _csrfTokenPattern.Match(html);
        if (match.Success)
        {
            Logger.Debug("Retrieved CSRF token");
            CsrfToken = match.Groups[1].Value;
        }
        else
        {
            throw new Exception("Unable to find CSRF token");
        }
    }
}