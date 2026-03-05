using AGRONET.Auth.Application.Contracts.Auth;
using AGRONET.Auth.Application.Interfaces;
using AGRONET.Auth.Infrastructure.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace AGRONET.Auth.Infrastructure.Security
{
    public sealed class TokenService : ITokenService
    {
        private readonly JwtOptions _opt;
        private readonly byte[] _signingKeyBytes;

        public TokenService(IOptions<JwtOptions> opt)
        {
            _opt = opt.Value;
            _signingKeyBytes = Encoding.UTF8.GetBytes(_opt.SigningKey);
            if (_signingKeyBytes.Length < 32)
                throw new InvalidOperationException("Jwt:SigningKey debe tener al menos 32 caracteres.");
        }

        public string CreateAccessToken(UserDto user)
        {
            var claims = new List<Claim>
            {
                // ✅ Estándar JWT
                new(JwtRegisteredClaimNames.Sub, user.IdUsuario.ToString()),

                // ✅ Estándar .NET para identificar usuario
                new(ClaimTypes.NameIdentifier, user.IdUsuario.ToString()),

                new("username", user.Username),
                new("dni", user.DniNorm ?? ""),
                new(ClaimTypes.Role, user.RolCodigo ?? "USUARIO"),
                 // ✅ CLAVE para menús por rol (int)
                new("roleId", user.IdRol.ToString())
            };

            var key = new SymmetricSecurityKey(_signingKeyBytes);
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _opt.Issuer,
                audience: _opt.Audience,
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddMinutes(_opt.AccessTokenMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        public string CreateRefreshToken()
        {
            // 64 bytes -> base64url
            var bytes = RandomNumberGenerator.GetBytes(64);
            return Base64UrlEncoder.Encode(bytes);
        }

        public byte[] HashRefreshToken(string refreshToken)
        {
            // Best practice: HMACSHA256 con SigningKey como secret (puedes separar secret si quieres)
            using var hmac = new HMACSHA256(_signingKeyBytes);
            return hmac.ComputeHash(Encoding.UTF8.GetBytes(refreshToken));
        }

        public string CreateRegistrationToken(string username)
        {
            // Token de propósito: "registration", corto (5 min)
            var claims = new List<Claim>
        {
            new("purpose", "registration"),
            new("username", username)
        };

            var key = new SymmetricSecurityKey(_signingKeyBytes);
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _opt.Issuer,
                audience: _opt.Audience,
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddMinutes(5),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public bool TryValidateRegistrationToken(string token, out string username)
        {
            username = string.Empty;

            var handler = new JwtSecurityTokenHandler();
            var key = new SymmetricSecurityKey(_signingKeyBytes);

            try
            {
                var principal = handler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = _opt.Issuer,
                    ValidateAudience = true,
                    ValidAudience = _opt.Audience,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = key,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromSeconds(30)
                }, out _);

                var purpose = principal.Claims.FirstOrDefault(c => c.Type == "purpose")?.Value;
                if (!string.Equals(purpose, "registration", StringComparison.OrdinalIgnoreCase))
                    return false;

                var u = principal.Claims.FirstOrDefault(c => c.Type == "username")?.Value;
                if (string.IsNullOrWhiteSpace(u)) return false;

                username = u;
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
