#region License
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
// Last edited on 07/26/2022 @ 19:18
#endregion

using System.Text.Json;
using RaffleRunner.Models;
using System.Text.RegularExpressions;

namespace RaffleRunner.Commands;

[Command(Name = "join-raffles", Description = "Scans and joins all available raffles")]
public class JoinRafflesCommand : AuthenticatedCommand
{
    #region Options
    [Option("-r|--repeat", Description = "How many times to scan and join available raffles. Omit or use 0 to repeat indefinitely.")]
    public int RepeatAmount { get; }
    
    [Option("-e|--ending", Description = "Sort raffles by time left rather than when they were created.")]
    public bool SortByTimeLeft { get; }
    
    [Option("-i|--increment-scan-delay", Description = "Increment the delay between scans if the previous scan returned no available raffles.")]
    public bool IncrementScanDelay { get; }

    [Option("-p|--paranoid", Description = "Enable paranoid mode which will skip raffles if they could potentially be traps. May cause false positive.")]
    public bool Paranoid { get; }
    #endregion

    private int _scanDelay = 5000;
    
    private const int    JoinDelay               = 4000;
    private const int    PaginateDelay           = 500;
    private const string RafflesIndexUrl         = "/raffles";
    private const string RaffleEnterEndpoint     = "/ajax/viewraffle/EnterRaffle";
    private const string RafflesPaginateEndpoint = "/ajax/raffles/Paginate";
    private const string RafflePanelSelector     = ".panel-raffle:not(.raffle-entered)";
    
    private readonly Logger       _logger        = LogManager.GetCurrentClassLogger();
    private readonly List<string> _queue         = new();
    private readonly List<string> _joinedRaffles = new();
    private readonly Regex        _entryPattern  = new(@"ScrapTF\.Raffles\.RedirectToRaffle\('(?<RaffleId>[A-Z0-9]{6,})'\)", RegexOptions.Compiled);
    private readonly Regex        _hashPattern   = new(@"EnterRaffle\('(?<RaffleId>[A-Z0-9]{6,})', '(?<RaffleHash>[a-f0-9]{64})'", RegexOptions.Compiled);
    private readonly Regex        _limitPattern  = new(@"total=""(?<Entered>\d+)"" data-max=""(?<Max>\d+)", RegexOptions.Compiled);

    public override async Task ExecuteAsync()
    {
        _logger.Debug("Starting loop with {Iteractions} iterations", RepeatAmount);
        
        bool repeatForever = RepeatAmount < 1;
        for (int i = 0; repeatForever || i < RepeatAmount; i++)
        {
            _logger.Info("Scanning raffles");
            
            await ScanRafflesAsync();
            
            if (_queue.Count > 0)
            {
                await JoinRafflesAsync();
            }
            else
            {
                if (!repeatForever && i == RepeatAmount)
                {
                    _logger.Debug("Arrived at end of loop, breaking");
                    break;
                }
                
                _logger.Debug("All raffles have been entered, scanning again after {Delay} seconds", _scanDelay / 1000);
                
                await Task.Delay(_scanDelay);

                if (IncrementScanDelay)
                {
                    _scanDelay += 1000;
                }
            }
        }
        
        _logger.Debug("Loop ended");
    }

    private async Task ScanRafflesAsync()
    {
        _queue.Clear();
        
        bool   doneScanning = false;
        string html         = await GetStringAsync(RafflesIndexUrl);
        string lastId       = string.Empty;

        while (!doneScanning)
        {
            string json =  await PaginateAsync(lastId);
            if (json == null)
            {
                _logger.Error("Pagination didn't return a valid JSON response, trying again in 10 seconds");
                await Task.Delay(10_000);
                continue;
            }

            try
            {
                var paginateResponse = JsonSerializer.Deserialize<PaginateResponse>(json);
                if (paginateResponse is { Success: true })
                {
                    html   += paginateResponse.Html;
                    lastId =  paginateResponse.LastId;

                    if (!paginateResponse.Done)
                    {
                        await Task.Delay(PaginateDelay);
                    }

                    doneScanning = true;

                    var document       = HtmlParser.ParseDocument(html);
                    var raffleElements = document.QuerySelectorAll(RafflePanelSelector);
                    foreach (var raffleElement in raffleElements)
                    {
                        string elementHtml   = raffleElement.InnerHtml;
                        var    raffleIdMatch = _entryPattern.Match(elementHtml);
                        if (raffleIdMatch.Success)
                        {
                            string raffleId = raffleIdMatch.Groups["RaffleId"].Value;
                            if (!_queue.Contains(raffleId) && !_joinedRaffles.Contains(raffleId))
                            {
                                _queue.Add(raffleId);
                            }
                        }
                    }
                }
                else
                {
                    if (paginateResponse is { Message: { } })
                    {
                        if (paginateResponse.Message.Contains("active site ban"))
                        {
                            _logger.Fatal("Account is banned");
                        }
                                
                        _logger.Error("Encountered an error while paginating: {Message} - Waiting 10 seconds", paginateResponse.Message);

                        await Task.Delay(10_000);
                    }
                    else
                    {
                        _logger.Error("Paginate response for apex {Apex} was unsuccessful", string.IsNullOrEmpty(lastId) ? "<empty>" : lastId);
                    }
                }
            }
            catch (JsonException ex)
            {
                _logger.Error(ex, "Failed to read pagination data");
            }
        }
    }

    private async Task<string> PaginateAsync(string apex = null)
    {
        string sort = SortByTimeLeft ? "0" : "1"; // They should really use integers for this
        var postData = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("start", apex),
            new KeyValuePair<string, string>("sort", sort),
            new KeyValuePair<string, string>("puzzle", "0"),
            new KeyValuePair<string, string>("csrf", CsrfToken)
        });

        var response = await HttpClient.PostAsync(RafflesPaginateEndpoint, postData);
        if (response.StatusCode == HttpStatusCode.OK)
        {
            return await response.Content.ReadAsStringAsync();
        }

        return null;
    }

    private async Task JoinRafflesAsync()
    {
        int joined = 0;
        int total  = _queue.Count;
        var queue  = _queue.Where(r => !_joinedRaffles.Contains(r)).ToList();
        
        _logger.Info("Joining {Count} {Pluralization:l}", queue.Count, queue.Count != 1 ? "raffles" : "raffle");
        
        foreach (string raffle in queue)
        {
            _logger.Debug("Joining raffle {Id}", raffle);
            
            string html        = await GetStringAsync($"/raffles/{raffle}");
            var    hashMatch   = _hashPattern.Match(html);
            var    limitsMatch = _limitPattern.Match(html);
            bool   raffleEnded = html.Contains(@"data-time=""Raffle Ended""");
            
            // TODO honeypot checking

            if (raffleEnded)
            {
                _logger.Info("Raffle {Id} has ended", raffle);
                
                total--;
                _joinedRaffles.Add(raffle);
                continue;
            }

            if (limitsMatch.Success)
            {
                int num = int.Parse(limitsMatch.Groups["Entered"].Value);
                int max = int.Parse(limitsMatch.Groups["Max"].Value);
                if (Paranoid && num < 2)
                {
                    _logger.Info("Raffle {Id} has too few entries (from Paranoid option)", raffle);
                    
                    total--;
                    continue;
                }

                if (num >= max)
                {
                    _logger.Info("Raffle {Id} is full ({Num}/{Max})", raffle, num, max);
                    
                    total--;
                    _joinedRaffles.Add(raffle);
                    continue;
                }
            }

            if (hashMatch.Success)
            {
                var hash = hashMatch.Groups["RaffleHash"].Value;
                var postData = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("raffle", raffle),
                    new KeyValuePair<string, string>("captcha", ""),
                    new KeyValuePair<string, string>("hash", hash),
                    new KeyValuePair<string, string>("flag", ""),
                    new KeyValuePair<string, string>("csrf", CsrfToken),
                });
                var enterRequest = new HttpRequestMessage(HttpMethod.Post, RaffleEnterEndpoint);
                enterRequest.Content          = postData;
                enterRequest.Headers.Referrer = new Uri($"https://scrap.tf/raffles/{raffle}");
                var enterResponse = await HttpClient.SendAsync(enterRequest);
                
                // TODO handle request errors

                string json                = await enterResponse.Content.ReadAsStringAsync();
                var    enterRaffleResponse = JsonSerializer.Deserialize<EnterRaffleResponse>(json);
                if (enterRaffleResponse is { Success: true })
                {
                    joined++;
                    
                    _logger.Info("Joined raffle {Id} ({Joined} of {Total})", raffle, joined, total);
                    
                    _joinedRaffles.Add(raffle);
                }
                else
                {
                    _logger.Error("Unable to join raffle {Id}: {Message}", raffle, enterRaffleResponse?.Message ?? "Unknown Reason");
                }

                await Task.Delay(JoinDelay);
            }
            else
            {
                _joinedRaffles.Add(raffle);
                
                _logger.Error("Could not obtain hash from raffle {Id}", raffle);
            }
        }

        if (joined > 0)
        {
            _logger.Info("Finished raffle queue");
        }
    }
}