using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.IdentityModel.Tokens;
using Warehouse.Api.Common;
using Warehouse.Core.DTO.Users;
using Warehouse.Core.Settings;

namespace Warehouse.Api.Extensions
{
    public static class CommandExtensions
    {
        public static string ManufacturerFolderPath =>
            Directory.GetCurrentDirectory() + @"\..\Warehouse.Api\wwwroot\Manufacturers\";
        public static string CustomerFolderPath =>
            Directory.GetCurrentDirectory() + @"\..\Warehouse.Api\wwwroot\Customers\";
        public static string ProductFolderPath =>
            Directory.GetCurrentDirectory() + @"\..\Warehouse.Api\wwwroot\Products\";
        public static string UserFolderPath =>
            Directory.GetCurrentDirectory() + @"\..\Warehouse.Api\wwwroot\Users\";

        public static JsonSerializerOptions JsonSerializerOptions =>
            new() {PropertyNamingPolicy = JsonNamingPolicy.CamelCase};
        
        public static void CheckForNull<T>(this T item)
        {
            if (item == null)
            {
                var typeName = typeof(T).Name;
                throw Result<T>.Failure("id", $"{typeName} doesn't exists", HttpStatusCode.BadRequest);
            }
        }
        
        public static string GenerateJwtToken(UserDto user, JwtTokenConfiguration tokenConfiguration)
        {
            List<Claim> claims = new()
            {
                new("Id", user.Id),
                new("UserName", user.UserName),
                new("Email", user.Email),
                new("SessionId", user.SessionId)
            };
            claims.AddRange(user.Roles.Select(r => new Claim(ClaimTypes.Role, r)));

            var jwtToken = new JwtSecurityToken(
                tokenConfiguration.Issuer,
                tokenConfiguration.Audience,
                claims,
                expires: DateTime.UtcNow.AddMinutes(tokenConfiguration.AccessTokenExpirationMinutes),
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenConfiguration.Secret)),
                    SecurityAlgorithms.HmacSha256Signature));

            return new JwtSecurityTokenHandler().WriteToken(jwtToken);
        }

        public static string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }
}