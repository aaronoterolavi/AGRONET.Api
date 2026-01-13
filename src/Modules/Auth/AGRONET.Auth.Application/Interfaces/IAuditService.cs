using System;
using System.Collections.Generic;
using System.Text;

namespace AGRONET.Auth.Application.Interfaces
{
    public interface IAuditService
    {
        Task LogAsync(string? username, string? dniNorm, string resultado, string? mensaje, string? ip, string? userAgent, CancellationToken ct = default);
    }
}
