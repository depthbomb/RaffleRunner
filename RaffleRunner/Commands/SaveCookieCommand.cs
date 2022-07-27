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
// Created on     07/26/2022 @ 17:50
// Last edited on 07/26/2022 @ 18:48
#endregion

using System.IO;
using RaffleRunner.Attributes;

namespace RaffleRunner.Commands;

[Command("save-cookie", Description = "Saves your cookie value to the file system")]
public class SaveCookieCommand : RootCommand
{
    [Cookie]
    [Required]
    [Argument(0, Description = "Your scr_session cookie value")]
    public string Cookie { get; set; }
    
    private readonly Logger _logger = LogManager.GetCurrentClassLogger();

    public override async Task ExecuteAsync()
    {
        try
        {
            await File.WriteAllTextAsync(GlobalShared.CookieFilePath, Cookie);
            _logger.Info("Saved cookie");
        }
        catch (Exception ex)
        {
            _logger.Fatal(ex, "Failed to save cookie");
        }
    }
}