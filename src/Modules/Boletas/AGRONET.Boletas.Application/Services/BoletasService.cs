using AGRONET.Boletas.Application.DTOs;
using AGRONET.Boletas.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace AGRONET.Boletas.Application.Services
{
    public class BoletasService : IBoletasService
    {
        private readonly IBoletasRepository _repository;

        public BoletasService(IBoletasRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<BoletaListadoDto>> ListarPorDniAnioMesAsync(string dni, string anio, string mes)
        {
            return await _repository.ListarPorDniAnioMesAsync(dni, anio, mes);
        }

        public async Task<BoletaArchivoDto?> ObtenerPorIdYDniAsync(int iCodBoleta, string dni)
        {
            return await _repository.ObtenerPorIdYDniAsync(iCodBoleta, dni);
        }

        public async Task MarcarVistoAsync(int iCodBoleta, string dni)
        {
            await _repository.MarcarVistoAsync(iCodBoleta, dni);
        }

        public async Task MarcarDescargadoAsync(int iCodBoleta, string dni)
        {
            await _repository.MarcarDescargadoAsync(iCodBoleta, dni);
        }
    }
}
