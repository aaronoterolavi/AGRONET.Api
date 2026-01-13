using System;
using System.Collections.Generic;
using System.Text;

namespace AGRONET.Auth.Application.Contracts.Auth
{
    public sealed class LoginRequestDto
    {
        public string Username { get; set; } = default!; // ej: aotero
        public string Password { get; set; } = default!;
    }
}
