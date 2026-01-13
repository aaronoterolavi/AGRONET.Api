using AGRONET.Auth.Application.Common.Results;
using AGRONET.Auth.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace AGRONET.Auth.Application.Services
{
    public sealed class AuthService : IAuthService
    {
        private readonly IAdAuthService _ad;
        private readonly IUsersRepository _users;
        private readonly ITokenService _tokens;
        private readonly IRefreshTokenRepository _refreshRepo;
        private readonly IAuditService? _audit;

        public AuthService(
            IAdAuthService ad,
            IUsersRepository users,
            ITokenService tokens,
            IRefreshTokenRepository refreshRepo,
            IAuditService? audit = null)
        {
            _ad = ad;
            _users = users;
            _tokens = tokens;
            _refreshRepo = refreshRepo;
            _audit = audit;
        }

        public async Task<AuthResult> LoginAsync(string username, string password, string? ip, string? userAgent, CancellationToken ct = default)
        {
            username = (username ?? "").Trim().ToLowerInvariant();

            var adOk = await _ad.ValidateCredentialsAsync(username, password, ct);
            if (!adOk)
            {
                if (_audit != null) await _audit.LogAsync(username, null, "INVALID_AD", "Credenciales AD inválidas", ip, userAgent, ct);
                return AuthResult.Fail("Usuario o contraseña incorrectos.");
            }

            var user = await _users.GetByUsernameAsync(username, ct);

            if (user is null)
            {
                var regToken = _tokens.CreateRegistrationToken(username);
                if (_audit != null) await _audit.LogAsync(username, null, "REQUIRES_REGISTRATION", "AD OK, falta registro en AGRONET", ip, userAgent, ct);
                return AuthResult.RequiresRegistration(username, regToken);
            }

            if (!user.Activo)
            {
                if (_audit != null) await _audit.LogAsync(username, user.DniNorm, "BLOCKED", "Usuario inactivo", ip, userAgent, ct);
                return AuthResult.Fail("Usuario inactivo. Contacte al administrador.");
            }

            var access = _tokens.CreateAccessToken(user);
            var refresh = _tokens.CreateRefreshToken();
            var refreshHash = _tokens.HashRefreshToken(refresh);

            var refreshExp = DateTime.UtcNow.AddDays(14); // luego lo leemos de config en TokenService
            await _refreshRepo.InsertAsync(user.IdUsuario, refreshHash, refreshExp, ip, userAgent, ct);
            await _users.UpdateLastLoginAsync(user.IdUsuario, ct);

            if (_audit != null) await _audit.LogAsync(username, user.DniNorm, "OK", "Login OK", ip, userAgent, ct);

            return AuthResult.Ok(user, access, refresh);
        }

        public async Task<AuthResult> CompleteRegistrationAsync(string registrationToken, string dni, string? ip, string? userAgent, CancellationToken ct = default)
        {
            if (!_tokens.TryValidateRegistrationToken(registrationToken, out var username))
                return AuthResult.Fail("Token de registro inválido o expirado. Vuelve a iniciar sesión.");

            username = username.Trim().ToLowerInvariant();

            // Inserta usuario desde personalsql por DNI
            var (codigo, mensaje, nuevoId) = await _users.InsertFromRgpmaByDniAsync(username, dni, ip, userAgent, ct);

            if (codigo == 1)
            {
                var user = await _users.GetByUsernameAsync(username, ct);
                if (user is null)
                    return AuthResult.Fail("Registro creado, pero no se pudo obtener el usuario. Intenta nuevamente.");

                var access = _tokens.CreateAccessToken(user);
                var refresh = _tokens.CreateRefreshToken();
                var refreshHash = _tokens.HashRefreshToken(refresh);

                var refreshExp = DateTime.UtcNow.AddDays(14);
                await _refreshRepo.InsertAsync(user.IdUsuario, refreshHash, refreshExp, ip, userAgent, ct);
                await _users.UpdateLastLoginAsync(user.IdUsuario, ct);

                if (_audit != null) await _audit.LogAsync(username, user.DniNorm, "REGISTER_OK", "Registro por DNI OK", ip, userAgent, ct);

                return AuthResult.Ok(user, access, refresh);
            }

            if (codigo == -2)
            {
                if (_audit != null) await _audit.LogAsync(username, null, "DNI_NOT_FOUND", mensaje, ip, userAgent, ct);
                return AuthResult.Fail("No se encontró el DNI en la tabla de trabajadores.");
            }

            if (codigo == -1)
            {
                // Caso real: concurrencia/reintento. Si ya existe, lo logueamos.
                var user = await _users.GetByUsernameAsync(username, ct);
                if (user != null && user.Activo)
                {
                    var access = _tokens.CreateAccessToken(user);
                    var refresh = _tokens.CreateRefreshToken();
                    var refreshHash = _tokens.HashRefreshToken(refresh);

                    var refreshExp = DateTime.UtcNow.AddDays(14);
                    await _refreshRepo.InsertAsync(user.IdUsuario, refreshHash, refreshExp, ip, userAgent, ct);
                    await _users.UpdateLastLoginAsync(user.IdUsuario, ct);

                    if (_audit != null) await _audit.LogAsync(username, user.DniNorm, "REGISTER_DUPLICATE_OK", "Duplicado pero ya existe, login OK", ip, userAgent, ct);

                    return AuthResult.Ok(user, access, refresh);
                }

                return AuthResult.Fail("Conflicto al registrar usuario. Intenta nuevamente.");
            }

            if (_audit != null) await _audit.LogAsync(username, null, "REGISTER_ERROR", mensaje, ip, userAgent, ct);
            return AuthResult.Fail("No se pudo completar el registro. " + mensaje);
        }

        public async Task<AuthResult> RefreshAsync(string refreshToken, string? ip, string? userAgent, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
                return AuthResult.Fail("Refresh token requerido.");

            var oldHash = _tokens.HashRefreshToken(refreshToken);

            var old = await _refreshRepo.GetByHashAsync(oldHash, ct);
            if (old is null || old.RevokedAt != null || old.ExpiresAt <= DateTime.UtcNow)
                return AuthResult.Fail("Refresh token inválido o expirado.");

            var user = await _users.GetByIdAsync(old.IdUsuario, ct);
            if (user is null || !user.Activo)
                return AuthResult.Fail("Usuario inválido o inactivo.");

            var newAccess = _tokens.CreateAccessToken(user);
            var newRefresh = _tokens.CreateRefreshToken();
            var newHash = _tokens.HashRefreshToken(newRefresh);

            var newExp = DateTime.UtcNow.AddDays(14);

            var (codigo, mensaje) = await _refreshRepo.RotateAsync(oldHash, newHash, newExp, ip, userAgent, ct);
            if (codigo != 1)
                return AuthResult.Fail("No se pudo refrescar sesión. " + mensaje);

            if (_audit != null) await _audit.LogAsync(user.Username, user.DniNorm, "REFRESH_OK", "Refresh OK", ip, userAgent, ct);

            return AuthResult.Ok(user, newAccess, newRefresh);
        }


        //public async Task LogoutAsync(string refreshToken, CancellationToken ct = default)
        //{
        //    if (string.IsNullOrWhiteSpace(refreshToken)) return;
        //    var hash = _tokens.HashRefreshToken(refreshToken);
        //    await _refreshRepo.RevokeAsync(hash, "LOGOUT", ct);
        //}

        public async Task LogoutAsync(int idUsuario, string refreshToken, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(refreshToken)) return;
            var hash = _tokens.HashRefreshToken(refreshToken);
            await _refreshRepo.RevokeByUserAsync(idUsuario, hash, "LOGOUT", ct);
        }

    }
}
