using AGRONET.FichaSalida.Application.Configuration;
using AGRONET.FichaSalida.Application.Contracts.Common;
using AGRONET.FichaSalida.Application.Contracts.FichaSalida;
using AGRONET.FichaSalida.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
 

namespace AGRONET.FichaSalida.Application.Services
{
    public sealed class FichaSalidaService : IFichaSalidaService
    {
        private readonly IFichaSalidaRepository _repo;

        private readonly IFichaSalidaAdjuntoRepository _adjRepo;
        private readonly StorageOptions _storage;
        private readonly UploadsOptions _uploads;

        public FichaSalidaService(IFichaSalidaRepository repo,
        IFichaSalidaAdjuntoRepository adjRepo,
        IOptions<StorageOptions> storage,
        IOptions<UploadsOptions> uploads)
        {
            _repo = repo;
            _adjRepo = adjRepo;
            _storage = storage.Value;
            _uploads = uploads.Value;
        }

        public Task<IReadOnlyList<FichaSalidaTipoDto>> ListarTiposAsync(CancellationToken ct = default)
            => _repo.ListarTiposAsync(ct);

        public Task<IReadOnlyList<FichaSalidaTipoDetalleDto>> ListarDetallesPorTipoAsync(
            string codFichaSalidaTipo,
            CancellationToken ct = default)
        {
            codFichaSalidaTipo = (codFichaSalidaTipo ?? "").Trim();
            if (string.IsNullOrWhiteSpace(codFichaSalidaTipo))
                return Task.FromResult<IReadOnlyList<FichaSalidaTipoDetalleDto>>([]);

            // tu SP acepta estado, por defecto "1"
            return _repo.ListarDetallesPorTipoAsync(codFichaSalidaTipo, "1", ct);
        }

        public async Task<PagedResultDto<FichaSalidaHistorialDto>> ListarHistorialPorDniAsync(
     string dni,
     string estadoAutorizacion,
     int pageSize,
     int pageNumber,
     CancellationToken ct = default)
        {
            dni = (dni ?? "").Trim();
            estadoAutorizacion = (estadoAutorizacion ?? "").Trim();

            if (pageSize <= 0) pageSize = 20;
            if (pageSize > 200) pageSize = 200;
            if (pageNumber < 0) pageNumber = 0;

            var (items, total) = await _repo.ListarHistorialAsync(dni, estadoAutorizacion, pageSize, pageNumber, ct);

            return new PagedResultDto<FichaSalidaHistorialDto>
            {
                Items = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalRows = total
            };
        }

        public Task<IReadOnlyList<FichaSalidaEstadoDto>> ListarEstadosAsync(CancellationToken ct = default)
       => _repo.ListarEstadosAsync(ct);

        public async Task<FichaSalidaCrearResponseDto> CrearAsync(
        string dni,
        FichaSalidaCrearRequestDto req,
        IFormFile? documento,
        CancellationToken ct = default)
        {
            // 1) Insertar ficha (SP valida y responde Id + Mensaje)
            var (idFicha, msg) = await _repo.InsertarAsync(dni, req, ct);

            // Si SP devolvió error: Id null
            if (idFicha is null)
            {
                return new FichaSalidaCrearResponseDto
                {
                    IdFichaSalida = null,
                    MensajeSalida = string.IsNullOrWhiteSpace(msg) ? "No se pudo registrar." : msg,
                    TieneAdjunto = false
                };
            }

            // 2) Si no hay documento, listo
            if (documento is null || documento.Length == 0)
            {
                return new FichaSalidaCrearResponseDto
                {
                    IdFichaSalida = idFicha,
                    MensajeSalida = msg,
                    TieneAdjunto = false
                };
            }

            // 3) Validar archivo
            var maxBytes = (long)_uploads.MaxSizeMb * 1024 * 1024;
            if (documento.Length > maxBytes)
                return new FichaSalidaCrearResponseDto
                {
                    IdFichaSalida = idFicha,
                    MensajeSalida = $"Archivo excede el máximo permitido ({_uploads.MaxSizeMb} MB).",
                    TieneAdjunto = false
                };

            var ext = Path.GetExtension(documento.FileName)?.ToLowerInvariant() ?? "";
            if (!_uploads.AllowedExtensions.Any(x => string.Equals(x, ext, StringComparison.OrdinalIgnoreCase)))
                return new FichaSalidaCrearResponseDto
                {
                    IdFichaSalida = idFicha,
                    MensajeSalida = "Tipo de archivo no permitido. Solo: pdf, docx, jpg, png.",
                    TieneAdjunto = false
                };

            // 4) Calcular ruta base según modo
            var mode = (_storage.Mode ?? "Local").Trim();
            var basePath = mode.Equals("UNC", StringComparison.OrdinalIgnoreCase)
                ? _storage.UncBasePath
                : _storage.LocalBasePath;

            // Carpeta por ficha
            var folder = Path.Combine(basePath, idFicha.Value.ToString());
            Directory.CreateDirectory(folder);

            // Nombre seguro para guardar
            var safeName = $"{Guid.NewGuid():N}{ext}";
            var fullPath = Path.Combine(folder, safeName);

            // 5) Guardar archivo + sha256
            byte[] sha;
            await using (var fs = new FileStream(fullPath, FileMode.CreateNew, FileAccess.Write, FileShare.None))
            {
                await documento.CopyToAsync(fs, ct);
            }

            await using (var read = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using var sha256 = SHA256.Create();
                sha = await sha256.ComputeHashAsync(read, ct);
            }

            // 6) Insert metadata en BD_AGRONET
            var idAdj = await _adjRepo.InsertarAdjuntoAsync(
                idFichaSalida: idFicha.Value,
                fileName: safeName,
                originalName: documento.FileName,
                contentType: documento.ContentType,
                fileSizeBytes: documento.Length,
                storageMode: mode.Equals("UNC", StringComparison.OrdinalIgnoreCase) ? "UNC" : "Local",
                storagePath: fullPath,
                sha256: sha,
                createdByDni: dni,
                ct: ct);

            return new FichaSalidaCrearResponseDto
            {
                IdFichaSalida = idFicha,
                MensajeSalida = msg,
                TieneAdjunto = true,
                IdAdjunto = idAdj
            };
        }
    }
}
