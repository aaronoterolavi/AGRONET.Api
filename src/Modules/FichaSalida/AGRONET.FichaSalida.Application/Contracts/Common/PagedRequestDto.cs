using System;
using System.Collections.Generic;
using System.Text;

namespace AGRONET.FichaSalida.Application.Contracts.Common
{
    public sealed class PagedRequestDto
    {
        public int PageNumber { get; set; } = 0; // 0-based
        public int PageSize { get; set; } = 20;
    }
}
