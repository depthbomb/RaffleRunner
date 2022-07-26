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
// Created on     07/25/2022 @ 19:46
// Last edited on 07/26/2022 @ 01:59
#endregion

using System.Text.Json;
using RaffleRunner.Models;

namespace RaffleRunner.Commands;

[Command(Name = "check-updates", Description = "Checks for updates")]
public class CheckUpdatesCommand : RootCommand
{
    [Option("-o|--open", Description = "Whether to open your the latest release GitHub page if a new release is available.")]
    public bool OpenPage { get; private set; }
    
    private const string Url = "https://api.github.com/repos/depthbomb/RaffleRunner/releases/latest";

    private readonly ILogger    _logger = Log.ForContext<CheckUpdatesCommand>();
    private readonly HttpClient _http;

    public CheckUpdatesCommand()
    {
        _http = new HttpClient();
        _http.DefaultRequestHeaders.Add("user-agent", "RaffleRunner - depthbomb/RaffleRunner");
    }

    public override async Task ExecuteAsync()
    {
        try
        {
            await AnsiConsole.Status().StartAsync("Checking for updates", async _ =>
            {
                var currentVersion = GlobalShared.Version;
                var json           = await _http.GetStreamAsync(Url);
                var release        = await JsonSerializer.DeserializeAsync<LatestRelease>(json);
                if (release != null)
                {
                    var remoteVersion = new Version(release.TagName);
                    if (currentVersion.CompareTo(remoteVersion) < 0)
                    {
                        AnsiConsole.MarkupLineInterpolated($"RaffleRunner version [bold black on #06b6d4]{remoteVersion}[/] is available to download.\n");

                        foreach (string bodyLine in release.Body.Split("\n"))
                        {
                            Console.WriteLine(bodyLine);
                        }

                        if (OpenPage)
                        {
                            Utils.OpenUrl(release.Url);
                        }
                    }
                    else
                    {
                        AnsiConsole.MarkupLine("You are using the latest version of RaffleRunner.");
                    }
                }
            });
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Unable to check for updates");
        }
    }
}