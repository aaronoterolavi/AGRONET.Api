using System.Data;
using System.Data.SqlClient;
using Dapper;
using Microsoft.Extensions.Configuration;
using AGRONET.Marcacion.Application.Abstractions;
using AGRONET.Marcacion.Application.Contracts;

namespace AGRONET.Marcacion.Infrastructure.Persistence;

public sealed class MarcacionRepository : IMarcacionRepository
{
    private readonly string _cs;

    public MarcacionRepository(IConfiguration config)
    {
        _cs = config.GetConnectionString("BD_AGRONET")
              ?? throw new InvalidOperationException("Falta ConnectionStrings:BD_AGRONET");
    }

    public async Task<string> RegistrarAsync(RegistrarMarcacionCommand cmd, CancellationToken ct)
    {
        await using var cn = new SqlConnection(_cs);
        await cn.OpenAsync(ct);

        var p = new DynamicParameters();
        p.Add("@dni", cmd.Dni, DbType.String, size: 8);
        p.Add("@cod_area", cmd.CodArea, DbType.String, size: 8);
        p.Add("@tipoAsistencia", cmd.TipoAsistencia, DbType.String, size: 2);
        p.Add("@Nid", cmd.Nid, DbType.String, size: 30);
        p.Add("@idbarra", cmd.IdBarra, DbType.String, size: 25);
        p.Add("@Cod_empresa", cmd.CodEmpresa, DbType.String, size: 25);
        p.Add("@aud_UsuarioLogin", cmd.AudUsuarioLogin, DbType.String, size: 50);
        p.Add("@aud_ipMarca", cmd.AudIpMarca, DbType.String, size: 25);

        // El SP hace SELECT 'mensaje'
        var msg = await cn.QueryFirstOrDefaultAsync<string>(
            new CommandDefinition(
                commandText: "USP_asistencia_insertar",
                parameters: p,
                commandType: CommandType.StoredProcedure,
                cancellationToken: ct
            )
        );

        return msg ?? "";
    }
}
