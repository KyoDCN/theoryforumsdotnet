using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace TheoryForums.Shared.Helpers
{
    public static class ExtensionMethods
    {
        /// <summary>
        /// Generates Token for authentication and authorization purposes
        /// </summary>
        /// <param name="claims"></param>
        /// <param name="secretKey">_Config.GetSection("AppSettings:Token").Value</param>
        /// <returns></returns>
        public static string GenerateToken(this JwtSecurityTokenHandler tokenHandler, IEnumerable<Claim> claims, string secretKey, IConfiguration config)
        {
            var key = Encoding.ASCII.GetBytes(secretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = config["AppSettings:Issuer"],
                Audience = config["AppSettings:Audience"],
                IssuedAt = DateTime.UtcNow,
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(3),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512Signature),
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public static string ObscureEmail(this string email)
        {
            char[] obscuredEmail = email.ToCharArray();

            for (int i = 1; i < email.Length-1; i++)
            {
                if (email[i+1] == '@') break;

                obscuredEmail[i] = '*';
            }

            return new string(obscuredEmail);
        }

        public static void AddApplicationError(this HttpResponse response, string message)
        {
            response.Headers.Add("Application-Error", message);
            response.Headers.Add("Access-Control-Expose-Headers", "Application-Error");
            response.Headers.Add("Access-Control-Allow-Origin", "*");
        }
    }
}
