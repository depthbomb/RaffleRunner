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
// Created on     07/25/2022 @ 19:15
// Last edited on 07/25/2022 @ 23:29
#endregion

using RaffleRunner.Commands;

namespace RaffleRunner;

[Subcommand(typeof(CheckWonCommand))]
[Subcommand(typeof(JoinRafflesCommand))]
[Subcommand(typeof(CheckUpdatesCommand))]
internal class Program
{
    private static async Task<int> Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;
        
        AnsiConsole.Write(new Rule($"[#06b6d4]{GlobalShared.ProgramIdentifier}[/] by depthbomb").LeftAligned());

        int exitCode = await CommandLineApplication.ExecuteAsync<Program>(args);
        
        AnsiConsole.Write(new Rule());
        
        return exitCode;
    }

    public async Task OnExecuteAsync(CommandLineApplication app) => app.ShowHelp();
}