using AGRONET.Boletas.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace AGRONET.Boletas.Application.Interfaces
{
    public interface IBoletasService
    {
        Task<IEnumerable<BoletaListadoDto>> ListarPorDniAnioMesAsync(string dni, string anio, string mes);
        Task<BoletaArchivoDto?> ObtenerPorIdYDniAsync(int iCodBoleta, string dni);
        Task MarcarVistoAsync(int iCodBoleta, string dni);
        Task MarcarDescargadoAsync(int iCodBoleta, string dni);
        Task<PlanillaResumenResponseDto> ObtenerPlanillaResumenAsync(string dni, string periodo);
    }
}
