using System;
using System.Collections.Generic;
using System.Text;

namespace AGRONET.FichaSalida.Application.Contracts.Common
{
    public sealed class OperacionResultadoDto
    {
        /// <summary>
        /// Código de resultado (1 = OK, 0 = ERROR)
        /// </summary>
        public int Codigo { get; set; }

        /// <summary>
        /// Mensaje descriptivo del resultado
        /// </summary>
        public string Mensaje { get; set; } = string.Empty;
    }
}
