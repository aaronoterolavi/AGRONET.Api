using System;
using System.Collections.Generic;
using System.Text;

namespace AGRONET.Auth.Application.Contracts.Auth
{
    public sealed class LoginResponseDto
    {
        public string Status { get; set; } = default!; // OK | REQUIRES_REGISTRATION
        public string? Message { get; set; }

        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }

        public string? RegistrationToken { get; set; } // cuando falta registro
        public UserDto? User { get; set; }
    }

    public sealed class UserDto
    {
        public int IdUsuario { get; set; }
        public string Username { get; set; } = default!;
        public string DniNorm { get; set; } = default!;
        public string? Nombres { get; set; }
        public string? ApePaterno { get; set; }
        public string? ApeMaterno { get; set; }
        public string? Correo { get; set; }
        public string RolCodigo { get; set; } = default!;
        public bool Activo { get; set; }
    }
}
