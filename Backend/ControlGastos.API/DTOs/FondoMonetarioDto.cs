namespace ControlGastos.API.DTOs
{
    public class FondoMonetarioDto
    {
        public int FondoMonetarioId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string TipoFondo { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public decimal SaldoActual { get; set; }
        public bool Activo { get; set; }
    }

    public class FondoMonetarioCreateDto
    {
        public string Nombre { get; set; } = string.Empty;
        public string TipoFondo { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public decimal SaldoActual { get; set; } = 0;
        public bool Activo { get; set; } = true;
    }

    public class FondoMonetarioUpdateDto
    {
        public string Nombre { get; set; } = string.Empty;
        public string TipoFondo { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public bool Activo { get; set; }
    }
}
