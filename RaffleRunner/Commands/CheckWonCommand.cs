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
// Last edited on 07/26/2022 @ 01:27
#endregion

using System.Text.RegularExpressions;

namespace RaffleRunner.Commands;

[Command(Name = "check-won", Description = "Checks to see if you have any raffles that need withdrawing")]
public class CheckWonCommand : AuthenticatedCommand
{
    [Option("-o|--open", Description = "Whether to open your default browser to the raffle won page if you've won any raffles.")]
    public bool OpenPage { get; private set; }
    
    private readonly ILogger _logger            = Log.ForContext<CheckWonCommand>();
    private readonly Regex   _wonRafflesPattern = new(@"You've won (?<Amount>\d) raffles? that must be withdrawn");
    
    public override async Task ExecuteAsync()
    {
        string outcome    = "You have no raffles that need to be withdrawn.";
        string html       = await GetStringAsync();
        var    match      = _wonRafflesPattern.Match(html);
        int    wonRaffles = int.Parse(match.Groups["Amount"].Value);
        if (wonRaffles != 0)
        {
            string raffles = wonRaffles == 1 ? "raffle" : "raffles";
            outcome = $"You've won [rapidblink #06b6d4]{wonRaffles} {raffles}[/] that may be withdrawn.";
        }

        Console.WriteLine();
        AnsiConsole.MarkupLine(outcome);

        if (OpenPage)
        {
            _logger.Debug("Opening web page");
            
            Utils.OpenUrl("https://scrap.tf/raffles/won");
        }
        else
        {
            AnsiConsole.MarkupLine("You can execute this command with the [bold #06b6d4]-o[/] option to automatically open your browser to the won raffles page.");
        }
        
        Console.WriteLine();
    }
}