using System;
using System.Collections.Generic;
using System.Text;

namespace AGRONET.Auth.Infrastructure.Configuration
{
    public sealed class JwtOptions
    {
        public string Issuer { get; set; } = default!;
        public string Audience { get; set; } = default!;
        public int AccessTokenMinutes { get; set; } = 15;
        public int RefreshTokenDays { get; set; } = 14;
        public string SigningKey { get; set; } = default!;
    }
}
