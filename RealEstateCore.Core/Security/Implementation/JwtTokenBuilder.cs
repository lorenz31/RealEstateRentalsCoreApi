using System;
using System.Collections.Generic;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using System.Text;

namespace RealEstateCore.Core.Security
{
    public class JwtTokenBuilder
    {
        private Dictionary<string, string> _claims = new Dictionary<string, string>();
        private int _expiryInMinutes = 10;
        private IConfiguration _config;

        public JwtTokenBuilder(IConfiguration config)
        {
            _config = config;
        }

        public string GenerateToken()
        {
            var claims = new List<Claim>
            {
              new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("JwtSettings:SecurityKey").Value));

            var token = new JwtSecurityToken(
                issuer: _config.GetSection("AppConfiguration:Issuer").Value,
                audience: _config.GetSection("AppConfiguration:Audience").Value,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_expiryInMinutes),
                signingCredentials: new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}