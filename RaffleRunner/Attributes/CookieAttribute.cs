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
// Last edited on 07/26/2022 @ 00:01
#endregion

using ValidationResult = System.ComponentModel.DataAnnotations.ValidationResult;

namespace RaffleRunner.Attributes;

[PublicAPI]
public class CookieAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext ctx)
    {
        if (value is not string cookie)
        {
            return new ValidationResult($"The {ctx.DisplayName} option is invalid.");
        }
        
        if (cookie.Length < 250)
        {
            return new ValidationResult($"The {ctx.DisplayName} option is too short.");
        }
            
        if (cookie.Contains("scr_session"))
        {
            return new ValidationResult($"The {ctx.DisplayName} option is invalid. Make sure to only include the value of the cookie.");
        }
            
        return ValidationResult.Success;

    }
}