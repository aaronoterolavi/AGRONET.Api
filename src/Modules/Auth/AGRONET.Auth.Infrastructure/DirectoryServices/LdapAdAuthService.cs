using AGRONET.Auth.Application.Interfaces;
using AGRONET.Auth.Infrastructure.Configuration;
using System;
using System.Collections.Generic;
using System.DirectoryServices.Protocols;
using System.Net;
using System.Text;
using Microsoft.Extensions.Options;

namespace AGRONET.Auth.Infrastructure.DirectoryServices
{
    public sealed class LdapAdAuthService : IAdAuthService
    {
        private readonly LdapOptions _opt;

        public LdapAdAuthService(IOptions<LdapOptions> opt)
        {
            _opt = opt.Value;
        }

        public Task<bool> ValidateCredentialsAsync(string username, string password, CancellationToken ct = default)
        {
            username = (username ?? "").Trim();

            try
            {
                var identifier = new LdapDirectoryIdentifier(_opt.Host, _opt.Port);
                using var connection = new LdapConnection(identifier)
                {
                    AuthType = AuthType.Ntlm,
                    Timeout = TimeSpan.FromSeconds(_opt.TimeoutSeconds)
                };

                connection.SessionOptions.ProtocolVersion = 3;
                connection.SessionOptions.SecureSocketLayer = _opt.UseSsl;

                // 1) DOMAIN\user
                var domainUser = $"{_opt.Domain}\\{username}";
                try
                {
                    connection.Bind(new NetworkCredential(domainUser, password));
                    return Task.FromResult(true);
                }
                catch (LdapException)
                {
                    // 2) user@domain (UPN)
                    var upn = $"{username}@agrorural.gob.pe";
                    connection.Bind(new NetworkCredential(upn, password));
                    return Task.FromResult(true);
                }
            }
            catch (LdapException ex)
            {
                Console.WriteLine($"LDAP ERROR: {ex.ErrorCode} - {ex.Message}");
                if (ex.InnerException != null)
                    Console.WriteLine($"INNER: {ex.InnerException.Message}");
                return Task.FromResult(false);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GENERIC LDAP ERROR: {ex.Message}");
                if (ex.InnerException != null)
                    Console.WriteLine($"INNER: {ex.InnerException.Message}");
                return Task.FromResult(false);
            }
        }

    }
}
