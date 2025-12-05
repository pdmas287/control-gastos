export interface Movimiento {
  fecha: Date;
  tipoMovimiento: string;
  fondoMonetario: string;
  descripcion: string;
  monto: number;
  tipoDocumento?: string;
  observaciones?: string;
}

export interface ComparativoPresupuesto {
  tipoGasto: string;
  montoPresupuestado: number;
  montoEjecutado: number;
  diferencia: number;
  porcentajeEjecucion: number;
}
