using System;
using System.Collections.Generic;
using System.Text;

namespace AGRONET.Auth.Infrastructure.Configuration
{
    public sealed class LdapOptions
    {
        public string Host { get; set; } = default!;
        public int Port { get; set; } = 389;
        public bool UseSsl { get; set; } = false;
        public string Domain { get; set; } = default!;         // AGRORURAL
        public string SearchBaseDn { get; set; } = default!;   // DC=agrorural,DC=gob,DC=pe
        public int TimeoutSeconds { get; set; } = 5;
    }
}
