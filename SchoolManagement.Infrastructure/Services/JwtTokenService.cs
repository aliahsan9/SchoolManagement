using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SchoolManagement.Application.Common.Configuration;
using SchoolManagement.Application.Interfaces;
using SchoolManagement.Domain.Constants;
using SchoolManagement.Domain.Entities;

namespace SchoolManagement.Infrastructure.Services
{
    public sealed class JwtTokenService(IOptions<JwtSettings> options) : IJwtTokenService
    {
        private readonly JwtSettings _settings = options.Value;

        public (string Token, DateTime ExpiresAtUtc) CreateAccessToken(
            User user,
            IReadOnlyCollection<string> roleNames,
            Guid? schoolId,
            string? schoolSubdomain)
        {
            var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(ClaimTypes.Name, $"{user.FirstName} {user.LastName}".Trim())
        };

            foreach (var role in roleNames)
                claims.Add(new Claim(ClaimTypes.Role, role));

            if (schoolId is Guid sid)
                claims.Add(new Claim(ClaimNames.SchoolId, sid.ToString()));
            if (!string.IsNullOrWhiteSpace(schoolSubdomain))
                claims.Add(new Claim(ClaimNames.SchoolSubdomain, schoolSubdomain));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.SigningKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.UtcNow.AddMinutes(_settings.AccessTokenMinutes);

            var token = new JwtSecurityToken(
                issuer: _settings.Issuer,
                audience: _settings.Audience,
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: expires,
                signingCredentials: creds);

            var written = new JwtSecurityTokenHandler().WriteToken(token);
            return (written, expires);
        }

        public (string PlainToken, DateTime ExpiresAtUtc) CreateRefreshTokenValue()
        {
            var plain = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
            var expires = DateTime.UtcNow.AddDays(_settings.RefreshTokenDays);
            return (plain, expires);
        }

        public string HashRefreshToken(string plainToken)
        {
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(plainToken));
            return Convert.ToHexString(bytes);
        }
    }
}