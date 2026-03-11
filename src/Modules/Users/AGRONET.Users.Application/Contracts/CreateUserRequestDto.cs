namespace AGRONET.Users.Application.Contracts
{
    public sealed class CreateUserRequestDto
    {
        public string Username { get; set; } = string.Empty;
        public string? Correo { get; set; }
        public string DniNorm { get; set; } = string.Empty;
        public string? ApePaterno { get; set; }
        public string? ApeMaterno { get; set; }
        public string? Nombres { get; set; }
        public int IdRol { get; set; }
        public string? CodArea { get; set; }
        public string? CodTipoEmpleado { get; set; }
        public string? Flg_sede { get; set; }
        public bool Activo { get; set; } = true;
        public string? descArea { get; set; }
        public string? CodTrabajador { get; set; }
    }
}