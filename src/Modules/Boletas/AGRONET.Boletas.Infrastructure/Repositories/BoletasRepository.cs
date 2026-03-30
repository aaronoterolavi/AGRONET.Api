using System.Data;
using AGRONET.Boletas.Application.DTOs;
using AGRONET.Boletas.Application.Interfaces;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace AGRONET.Boletas.Infrastructure.Repositories;

public class BoletasRepository : IBoletasRepository
{
    private readonly string _connectionString;

    public BoletasRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("BD_AGRONET")
            ?? throw new InvalidOperationException("No se encontró la cadena de conexión 'BD_AGRONET'.");
    }

    public async Task<IEnumerable<BoletaListadoDto>> ListarPorDniAnioMesAsync(string dni, string anio, string mes)
    {
        using IDbConnection connection = new SqlConnection(_connectionString);

        var parameters = new
        {
            vDni = dni,
            vAnio = anio,
            vMes = mes
        };

        var result = await connection.QueryAsync<BoletaListadoDto>(
            "dbo.USP_Boletas_ListarPorDniAnioMes",
            parameters,
            commandType: CommandType.StoredProcedure);

        return result;
    }

    public async Task<BoletaArchivoDto?> ObtenerPorIdYDniAsync(int iCodBoleta, string dni)
    {
        using IDbConnection connection = new SqlConnection(_connectionString);

        var parameters = new
        {
            iCodBoleta,
            vDni = dni
        };

        var result = await connection.QueryFirstOrDefaultAsync<BoletaArchivoDto>(
            "dbo.USP_Boletas_ObtenerPorIdYDni",
            parameters,
            commandType: CommandType.StoredProcedure);

        return result;
    }

    public async Task MarcarVistoAsync(int iCodBoleta, string dni)
    {
        using IDbConnection connection = new SqlConnection(_connectionString);

        var parameters = new
        {
            iCodBoleta,
            vDni = dni
        };

        await connection.ExecuteAsync(
            "dbo.USP_Boletas_MarcarVisto",
            parameters,
            commandType: CommandType.StoredProcedure);
    }

    public async Task MarcarDescargadoAsync(int iCodBoleta, string dni)
    {
        using IDbConnection connection = new SqlConnection(_connectionString);

        var parameters = new
        {
            iCodBoleta,
            vDni = dni
        };

        await connection.ExecuteAsync(
            "dbo.USP_Boletas_MarcarDescargado",
            parameters,
            commandType: CommandType.StoredProcedure);
    }
}