using AGRONET.Auth.Infrastructure.Data;
using AGRONET.FichaSalida.Application.Contracts.Common;
using AGRONET.FichaSalida.Application.Interfaces;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace AGRONET.FichaSalida.Infrastructure.Data.Repositories
{
    public sealed class FichaSalidaAdjuntoRepository : IFichaSalidaAdjuntoRepository
    {
        private readonly ISqlConnectionFactory _factory;

        public FichaSalidaAdjuntoRepository(ISqlConnectionFactory factory)
        {
            _factory = factory;
        }

        public async Task<long> InsertarAdjuntoAsync(
            int idFichaSalida,
            string fileName,
            string? originalName,
            string? contentType,
            long fileSizeBytes,
            string storageMode,
            string storagePath,
            byte[]? sha256,
            string? createdByDni,
            CancellationToken ct = default)
        {
            using var con = _factory.CreateBdAgronetConnection();

            var p = new DynamicParameters();
            p.Add("@IdFichaSalida", idFichaSalida, DbType.Int32);
            p.Add("@FileName", fileName, DbType.String);
            p.Add("@OriginalName", originalName, DbType.String);
            p.Add("@ContentType", contentType, DbType.String);
            p.Add("@FileSizeBytes", fileSizeBytes, DbType.Int64);
            p.Add("@StorageMode", storageMode, DbType.String);
            p.Add("@StoragePath", storagePath, DbType.String);
            p.Add("@Sha256", sha256, DbType.Binary, size: 32);
            p.Add("@CreatedByDni", createdByDni, DbType.String);

            return await con.ExecuteScalarAsync<long>(
                "dbo.USP_FichaSalidaAdjunto_Insertar",
                p,
                commandType: CommandType.StoredProcedure);
        }

        //public async Task<OperacionResultadoDto> AnularAsync(int id, CancellationToken ct = default)
        //{
        //    using var con = _factory.CreateBdAgronetConnection();

        //    var p = new DynamicParameters();
        //    p.Add("@id", id, DbType.Int32);

        //    return await con.QueryFirstAsync<OperacionResultadoDto>(
        //        "dbo.USP_tbl_FichaSalida_Anular",
        //        p,
        //        commandType: CommandType.StoredProcedure);
        //}

        //public async Task<OperacionResultadoDto> ActualizarEstadoAutorizacionAsync(
        //    int id,
        //    string estadoAutorizacion,
        //    CancellationToken ct = default)
        //{
        //    using var con = _factory.CreateBdAgronetConnection();

        //    var p = new DynamicParameters();
        //    p.Add("@id", id, DbType.Int32);
        //    p.Add("@estadoAutorizacion", estadoAutorizacion, DbType.String);

        //    return await con.QueryFirstAsync<OperacionResultadoDto>(
        //        "dbo.USP_tbl_FichaSalida_ActualizarEstadoAutorizacion",
        //        p,
        //        commandType: CommandType.StoredProcedure);
        //}
    }
}
