namespace AGRONET.Boletas.Application.DTOs;

public class PlanillaResumenResponseDto
{
    public IEnumerable<PlanillaDetalleDto> Ingresos { get; set; } = [];
    public IEnumerable<PlanillaDetalleDto> Descuentos { get; set; } = [];
    public PlanillaTotalDto? Totales { get; set; }
}