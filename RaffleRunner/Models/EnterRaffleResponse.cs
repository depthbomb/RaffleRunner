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
// Last edited on 07/25/2022 @ 19:12
#endregion

using System.Text.Json.Serialization;

namespace RaffleRunner.Models;

public class EnterRaffleResponse
{
    [JsonPropertyName("message")]
    public string Message { get; set; }

    [JsonPropertyName("entered_message")]
    public string EnteredMessage { get; set; }

    [JsonPropertyName("can_comment")]
    public bool CanComment { get; set; }

    [JsonPropertyName("success")]
    public bool Success { get; set; }
}