namespace ControlGastos.API.DTOs
{
    public class MovimientoDto
    {
        public DateTime Fecha { get; set; }
        public string TipoMovimiento { get; set; } = string.Empty; // "Gasto" o "Depósito"
        public string FondoMonetario { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public decimal Monto { get; set; }
        public string? TipoDocumento { get; set; }
        public string? Observaciones { get; set; }
    }

    public class ComparativoPresupuestoDto
    {
        public string TipoGasto { get; set; } = string.Empty;
        public decimal MontoPresupuestado { get; set; }
        public decimal MontoEjecutado { get; set; }
        public decimal Diferencia { get; set; }
        public decimal PorcentajeEjecucion { get; set; }
    }
}
