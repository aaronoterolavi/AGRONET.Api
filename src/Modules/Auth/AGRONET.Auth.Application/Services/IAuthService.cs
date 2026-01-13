using AGRONET.Auth.Application.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace AGRONET.Auth.Application.Services
{
    public interface IAuthService
    {
        Task<AuthResult> LoginAsync(string username, string password, string? ip, string? userAgent, CancellationToken ct = default);

        Task<AuthResult> CompleteRegistrationAsync(string registrationToken, string dni, string? ip, string? userAgent, CancellationToken ct = default);

        Task<AuthResult> RefreshAsync(string refreshToken, string? ip, string? userAgent, CancellationToken ct = default);

        //Task LogoutAsync(string refreshToken, CancellationToken ct = default);

        Task LogoutAsync(int idUsuario, string refreshToken, CancellationToken ct = default);

    }
}
