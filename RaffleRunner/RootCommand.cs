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
// Last edited on 07/26/2022 @ 18:57
#endregion

namespace RaffleRunner;

public class RootCommand
{
    [Option("-d|--debug", Description = "Enable debug logging")]
    public bool Debug { get; private set; }
    
    private readonly Logger _logger = LogManager.GetCurrentClassLogger();

    public virtual async Task OnExecuteAsync()
    {
        if (Debug)
        {
            foreach (var rule in LogManager.Configuration.LoggingRules)
            {
                rule.SetLoggingLevels(LogLevel.Trace, LogLevel.Fatal);
            }
            
            LogManager.ReconfigExistingLoggers();
        }
        
        await ExecuteAsync();
    }
    
    public virtual Task ExecuteAsync()
    {
        return Task.CompletedTask;
    }
}