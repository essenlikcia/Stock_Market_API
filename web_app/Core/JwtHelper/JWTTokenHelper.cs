using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using web_app.Migrations;
using web_app.Models;

namespace web_app.Core.Repositories
{
    public interface IJWTTokenHelper
    {
        string GenerateToken(CustomUser user);
    }

    public class JWTTokenHelper : IJWTTokenHelper
    {
        private readonly IConfiguration _Configuration;
        private readonly string _jwtKey;

        public JWTTokenHelper(IConfiguration configuration)
        {
            _Configuration = configuration;
            _jwtKey = _Configuration["AppSettings:JwtKey"];
        }

        public string GenerateToken(CustomUser user)
        {
            var claims = new List<Claim>
            {
                new Claim("email",user.Email),
                new Claim("id",user.Id.ToString()),
            };

            //user?.UserRoles.ForEach(role => new Claim(ClaimTypes.Role, role.Role.Name));

            var key = new SymmetricSecurityKey(Convert.FromBase64String(_jwtKey));

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddYears(1),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            );
            var generatedToken = new JwtSecurityTokenHandler().WriteToken(token);

            return generatedToken;
        }
    }
}