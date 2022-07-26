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
// Last edited on 07/26/2022 @ 01:52
#endregion

using System.Reflection;

namespace RaffleRunner;

[PublicAPI]
internal static class GlobalShared
{
    internal static Version      Version          = new("0.2.0.0");
    internal static string       MimicUserAgent   = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/103.0.0.0 Safari/537.36";
    internal static AssemblyName AssemblyName     = Assembly.GetExecutingAssembly().GetName();
    internal static string       FullAssemblyName = AssemblyName.Name!;
    internal static string       ProgramIdentifier => $"{FullAssemblyName} v{Version} ({ModuleVersion})";
    internal static Guid         ModuleVersion     => Assembly.GetExecutingAssembly().ManifestModule.ModuleVersionId;
}