namespace ControlGastos.API.DTOs
{
    public class TipoGastoDto
    {
        public int TipoGastoId { get; set; }
        public string Codigo { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public bool Activo { get; set; }
        public int UsuarioId { get; set; }
        public string? NombreUsuario { get; set; }
    }

    public class TipoGastoCreateDto
    {
        public string Descripcion { get; set; } = string.Empty;
        public bool Activo { get; set; } = true;
    }

    public class TipoGastoUpdateDto
    {
        public string Descripcion { get; set; } = string.Empty;
        public bool Activo { get; set; }
    }
}
