using AGRONET.Auth.Application.Contracts.Auth;
using System;
using System.Collections.Generic;
using System.Text;

namespace AGRONET.Auth.Application.Interfaces
{
    public interface ITokenService
    {
        string CreateAccessToken(UserDto user);

        /// <summary>
        /// Crea un refresh token aleatorio (string) para entregar al cliente.
        /// </summary>
        string CreateRefreshToken();

        /// <summary>
        /// Hashea el refresh token antes de guardarlo (HMACSHA256 recomendado).
        /// </summary>
        byte[] HashRefreshToken(string refreshToken);

        /// <summary>
        /// Token corto para completar registro (5 min). Debe amarrar el username.
        /// </summary>
        string CreateRegistrationToken(string username);

        bool TryValidateRegistrationToken(string token, out string username);
    }
}
