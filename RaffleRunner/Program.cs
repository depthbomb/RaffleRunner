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
// Last edited on 07/26/2022 @ 19:36
#endregion

using System.IO;
using NLog.Config;
using NLog.Targets;
using RaffleRunner.Commands;

namespace RaffleRunner;

[Subcommand(typeof(CheckWonCommand))]
[Subcommand(typeof(SaveCookieCommand))]
[Subcommand(typeof(JoinRafflesCommand))]
[Subcommand(typeof(CheckUpdatesCommand))]
internal class Program
{
    private static async Task<int> Main(string[] args)
    {
        Console.OutputEncoding =  Encoding.UTF8;
        Console.CancelKeyPress += (_, _) => ShutDown();

        if (!Path.Exists(GlobalShared.AppDataPath))
        {
            Directory.CreateDirectory(GlobalShared.AppDataPath);
        }

        InitializeLogger();

        var logger = LogManager.GetCurrentClassLogger();

        AnsiConsole.Write(new Rule($"[#06b6d4]{GlobalShared.ProgramIdentifier}[/] by depthbomb").LeftAligned());

        int exitCode;
        try
        {
            exitCode = await CommandLineApplication.ExecuteAsync<Program>(args);
        }
        catch (Exception ex)
        {
            logger.Fatal(ex, "Execution failed");
            exitCode = 1;
        }
        finally
        {
            ShutDown();
        }

        return exitCode;
    }

    public async Task OnExecuteAsync(CommandLineApplication app) => app.ShowHelp();

    private static void ShutDown()
    {
        AnsiConsole.Write(new Rule());

        LogManager.Shutdown();
    }
    
    private static void InitializeLogger()
    {
        var config = new LoggingConfiguration();
        var fileTarget = new FileTarget
        {
            Layout                       = @"${longdate} | ${pad:padding=5:inner=${level:uppercase=true}} | ${logger:shortName=true} | ${message}${exception}",
            ArchiveEvery                 = FileArchivePeriod.Month,
            ArchiveFileName              = "backup.{#}.zip",
            ArchiveNumbering             = ArchiveNumberingMode.Date,
            ArchiveDateFormat            = "yyyyMMddHHmm",
            EnableArchiveFileCompression = true,
            FileName                     = Path.Combine(GlobalShared.AppLogsPath, "${date:format=yyyy-MM}.log"),
            CreateDirs                   = true,
            MaxArchiveFiles              = 5,
        };
        var consoleTarget = new ColoredConsoleTarget
        {
            Layout           = @"${date:format=yyyy-MM-dd HH\:mm\:ss} | ${pad:padding=5:inner=${level:uppercase=true}} | ${pad:padding=20:inner=${logger:shortName=true}} | ${message}${exception}",
            EnableAnsiOutput = true,
        };
        consoleTarget.WordHighlightingRules.Add(
            new ConsoleWordHighlightingRule
            {
                Regex           = @"\d{4}(-\d{2}){2}\s(\d{2}:?){3}",
                ForegroundColor = ConsoleOutputColor.DarkGray
            }
        );
        consoleTarget.WordHighlightingRules.Add(
            new ConsoleWordHighlightingRule
            {
                Regex           = @"\w*Command",
                ForegroundColor = ConsoleOutputColor.Yellow
            }
        );
        consoleTarget.WordHighlightingRules.Add(
            new ConsoleWordHighlightingRule
            {
                Regex           = @""".*""",
                ForegroundColor = ConsoleOutputColor.Magenta
            }
        );
        consoleTarget.WordHighlightingRules.Add(
            new ConsoleWordHighlightingRule("DEBUG", ConsoleOutputColor.DarkGray, ConsoleOutputColor.NoChange)
        );
        consoleTarget.WordHighlightingRules.Add(
            new ConsoleWordHighlightingRule("INFO", ConsoleOutputColor.DarkCyan, ConsoleOutputColor.NoChange)
        );
        consoleTarget.WordHighlightingRules.Add(
            new ConsoleWordHighlightingRule("WARN", ConsoleOutputColor.DarkYellow, ConsoleOutputColor.NoChange)
        );
        consoleTarget.WordHighlightingRules.Add(
            new ConsoleWordHighlightingRule("ERROR", ConsoleOutputColor.Red, ConsoleOutputColor.NoChange)
        );
        consoleTarget.WordHighlightingRules.Add(
            new ConsoleWordHighlightingRule("FATAL", ConsoleOutputColor.White, ConsoleOutputColor.Red)
        );

        config.AddTarget("File", fileTarget);
        config.AddTarget("Console", consoleTarget);
        config.LoggingRules.Add(new LoggingRule("*", LogLevel.Debug, fileTarget));
        config.LoggingRules.Add(new LoggingRule("*", LogLevel.Info, consoleTarget));

        LogManager.Configuration = config;
    }
}