using System;
using System.Collections.Generic;
using System.Text;

namespace AGRONET.Auth.Application.Interfaces
{
    public interface IRefreshTokenRepository
    {
        Task InsertAsync(int idUsuario, byte[] tokenHash, DateTime expiresAt, string? ip, string? userAgent, CancellationToken ct = default);

        Task<RefreshTokenRecord?> GetByHashAsync(byte[] tokenHash, CancellationToken ct = default);

        Task RevokeAsync(byte[] tokenHash, string? reason, CancellationToken ct = default);

        Task<(int Codigo, string Mensaje)> RotateAsync(
            byte[] oldHash,
            byte[] newHash,
            DateTime newExpiresAt,
            string? ip,
            string? userAgent,
            CancellationToken ct = default);

        Task RevokeByUserAsync(int idUsuario, byte[] tokenHash, string? reason, CancellationToken ct = default);

    }

    public sealed class RefreshTokenRecord
    {
        public long IdRefreshToken { get; set; }
        public int IdUsuario { get; set; }
        public byte[] TokenHash { get; set; } = default!;
        public DateTime CreatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
        public DateTime? RevokedAt { get; set; }
        public byte[]? ReplacedByTokenHash { get; set; }
    }
}
