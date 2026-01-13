using System;
using System.Collections.Generic;
using System.Text;

namespace AGRONET.Auth.Application.Interfaces
{
    public interface IAdAuthService
    {
        Task<bool> ValidateCredentialsAsync(string username, string password, CancellationToken ct = default);
    }
}
