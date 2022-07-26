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
// Last edited on 07/25/2022 @ 23:58
#endregion

namespace RaffleRunner;

public enum Level
{
    Debug,
    Info,
    Warn,
    Error,
    Fatal,
    Success // Contextual
}

public static class Logger
{
    public static  bool   LogDebug   { get; set; }
    private static string DatePrefix => $"[{DateTime.Now}]".PadRight(21, ' ');

    public static void Debug(string format, params object[] args)
    {
        if (LogDebug)
        {
            PrintDatePrefix();
            PrintLevel(Level.Debug);
            Console.WriteLine(format, args);
        }
    }
    
    public static void Info(string format, params object[] args)
    {
        PrintDatePrefix();
        PrintLevel(Level.Info);
        Console.WriteLine(format, args);
    }
    
    public static void Warn(string format, params object[] args)
    {
        PrintDatePrefix();
        PrintLevel(Level.Warn);
        Console.WriteLine(format, args);
    }
    
    public static void Error(string format, params object[] args)
    {
        PrintDatePrefix();
        PrintLevel(Level.Error);
        Console.WriteLine(format, args);
    }
    
    public static void Fatal(string format, params object[] args)
    {
        PrintDatePrefix();
        PrintLevel(Level.Fatal);
        Console.WriteLine(format, args);
        
        Environment.Exit(1);
    }
    
    public static void Success(string format, params object[] args)
    {
        PrintDatePrefix();
        PrintLevel(Level.Success);
        Console.WriteLine(format, args);
    }

    private static void PrintLevel(Level level)
    {
        string color;
        switch (level)
        {
            default:
            case Level.Debug:
                color = "#64748b";
                break;
            case Level.Info:
                color = "#14b8a6";
                break;
            case Level.Warn:
                color = "#f97316";
                break;
            case Level.Error:
                color = "#dc2626";
                break;
            case Level.Fatal:
                color = "#e11d48";
                break;
            case Level.Success:
                color = "#84cc16";
                break;
        }

        string prefix = level.ToString().ToUpper().PadRight(7, ' ');
        AnsiConsole.MarkupInterpolated($"[bold {color}]{prefix}[/] ");
    }

    private static void PrintDatePrefix()
    {
        AnsiConsole.MarkupInterpolated($"[dim]{DatePrefix}[/] ");
    }
}