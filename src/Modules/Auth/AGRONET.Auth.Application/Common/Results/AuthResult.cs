using AGRONET.Auth.Application.Contracts.Auth;
using System;
using System.Collections.Generic;
using System.Text;

namespace AGRONET.Auth.Application.Common.Results
{
    public sealed class AuthResult
    {
        public string Status { get; private set; } = default!;
        public string? Message { get; private set; }

        public string? AccessToken { get; private set; }
        public string? RefreshToken { get; private set; }

        public string? RegistrationToken { get; private set; }
        public UserDto? User { get; private set; }

        public static AuthResult Ok(UserDto user, string accessToken, string refreshToken)
            => new()
            {
                Status = "OK",
                User = user,
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };

        public static AuthResult RequiresRegistration(string username, string registrationToken)
            => new()
            {
                Status = "REQUIRES_REGISTRATION",
                Message = "Usuario validado en AD pero no registrado en AGRONET. Enviar DNI para completar registro.",
                RegistrationToken = registrationToken,
                User = new UserDto { Username = username, DniNorm = "", RolCodigo = "", Activo = true } // opcional
            };

        public static AuthResult Fail(string message)
            => new()
            {
                Status = "ERROR",
                Message = message
            };
    }
}
