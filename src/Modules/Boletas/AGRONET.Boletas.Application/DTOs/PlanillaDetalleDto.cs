namespace AGRONET.Boletas.Application.DTOs;

public class PlanillaDetalleDto
{
    public string? Cod_Ano { get; set; }
    public string? Cod_Mes { get; set; }
    public string? Num_Documento { get; set; }
    public string? Tip_Planilla { get; set; }
    public string? Dsc_Planilla { get; set; }
    public string? SubNum_Planilla { get; set; }
    public string? Tip_Rem_Dscto { get; set; }
    public string? Dsc_Rem_Dscto { get; set; }
    public decimal Imp_Rem_Dscto { get; set; }
}