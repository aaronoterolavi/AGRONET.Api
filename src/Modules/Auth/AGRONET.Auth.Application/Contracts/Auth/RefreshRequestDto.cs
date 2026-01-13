using System;
using System.Collections.Generic;
using System.Text;

namespace AGRONET.Auth.Application.Contracts.Auth
{
    public sealed class RefreshRequestDto
    {
        public string RefreshToken { get; set; } = default!;
    }
}
