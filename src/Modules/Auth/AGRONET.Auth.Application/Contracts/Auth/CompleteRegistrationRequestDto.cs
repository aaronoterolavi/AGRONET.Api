using System;
using System.Collections.Generic;
using System.Text;

namespace AGRONET.Auth.Application.Contracts.Auth
{
    public sealed class CompleteRegistrationRequestDto
    {
        public string RegistrationToken { get; set; } = default!;
        public string Dni { get; set; } = default!;
    }
}
